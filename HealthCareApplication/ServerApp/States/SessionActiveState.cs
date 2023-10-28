using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class SessionActiveState : IState
    {
        private ServerContext _context;
        public SessionActiveState(ServerContext context)
        {
            _context = context;
        }

        public IState Handle(JsonObject packet)
        {
            string command = JsonUtil.GetValueFromPacket(packet, "command").ToString();
         
            if (command == "stats/send")
            {
                double speed = double.Parse(JsonUtil.GetValueFromPacket(packet, "data", "speed").ToString());
                int distance = int.Parse(JsonUtil.GetValueFromPacket(packet, "data", "distance").ToString());
                byte heartRate = byte.Parse(JsonUtil.GetValueFromPacket(packet, "data", "heartrate").ToString());

                Console.WriteLine("Buffering data: ");

                UserStat currentStat = new UserStat(speed, distance, heartRate);

                // Save data in server
                BufferUserData(currentStat);

                JsonObject statsForDoc = JsonSerializer.Deserialize<JsonObject>(JsonSerializer.Serialize(currentStat));

                // Data has been recieved and saved in the server, time to send it to the doctor
                // The actual sending will be done in the Server class itself
                _context.ResponseToDoctor = ResponseClientData.GenerateDoctorResponse(command, statsForDoc, _context.GetUserAccount().GetUserName());

                return this; // Stay in this state to recieve more data
            }
            else if (command == "session/stop")
            {
                _context.ResponseToPatient = ResponseClientData.GenerateResponse("session/stop", null, "ok");
                return new SessionStoppedState(_context);
            }
            else if(command == "session/pause")
            {
                _context.ResponseToPatient = ResponseClientData.GenerateResponse("session/pause", null, "ok");
                return new SessionPausedState(_context);
            }
            
            // TODO:
            // Implement pause and resume into this state.
            return this;
        }

        private void BufferUserData(UserStat userstat)
        {
            _context.UserStatsBuffer.Add(userstat);
            //foreach (UserStat stat in _context.userStatsBuffer)
            //{
            //   stat.ToString();
            //}
        }
    }
}
