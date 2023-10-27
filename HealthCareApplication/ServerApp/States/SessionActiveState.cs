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
            this._context = context;
        }

        public IState Handle(JsonObject packet)
        {
            string command = JsonUtil.GetValueFromPacket(packet, "command").ToString();
         
            if (command == "stats/send")
            {
                double speed = Double.Parse(JsonUtil.GetValueFromPacket(packet, "data", "speed").ToString());
                int distance = Int32.Parse(JsonUtil.GetValueFromPacket(packet, "data", "distance").ToString());
                byte heartRate = Byte.Parse(JsonUtil.GetValueFromPacket(packet, "data", "heartrate").ToString());

                Console.WriteLine("Buffering data: ");

                UserStat currentStat = new UserStat(speed, distance, heartRate);
                // Save data in server
                BufferUserData(currentStat);


                //JsonObject statsForDoc = (JsonObject)JsonConvert.SerializeObject(currentStat);
                //JsonObject statsForDoc = (JsonObject)JsonSerializer.Serialize<UserStat>(currentStat);
                string statsForDoc = JsonSerializer.Serialize(currentStat);
                // Data has been recieved and saved in the server, time to send it to the doctor
                // The actual sending will be done in the Server class itself
                _context.ResponseToDoctor = ResponseClientData.GenerateDoctorResponse(command, statsForDoc, _context.GetUserAccount().GetUserName());

                return this; // Stay in this state to recieve more data
            }
            else if (command == "session/stop")
            {
                this._context.ResponseToClient = ResponseClientData.GenerateResponse("session/stop", null, "ok");
                return new SessionStoppedState(this._context);
            }
            else if(command == "session/pause")
            {
                this._context.ResponseToClient = ResponseClientData.GenerateResponse("session/pause", null, "ok");
                return new SessionPausedState(this._context);
            }
            
            // To DO:
            // Implement pause and resume into this state.
            return this;
        }

        private void BufferUserData(UserStat userstat)
        {
          
            this._context.userStatsBuffer.Add(userstat);
            //foreach (UserStat stat in _context.userStatsBuffer)
            //{
            //   stat.ToString();
            //}
        }
    }
}
