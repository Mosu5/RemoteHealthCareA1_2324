using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ServerApp.States
{
    internal class SessionIdle : IState
    {

        private readonly ServerContext _context;
        public SessionIdle(ServerContext context)
        {
            _context = context;
        }

        public IState Handle(JsonObject packet)
        {
            // Get the command of the sent message
            string command = JsonUtil.GetValueFromPacket(packet, "command").ToString();

            if (command == "session/start")
            {
                // Mark user as active in session
                _context.GetUserAccount().HasActiveSession = true;
                _context.IsSessionActive = true;

                _context.ResponseToPatient = ResponseClientData.GenerateResponse("session/start", null, "ok");
                //_context.ResponseToDoctor = ResponseClientData.GenerateDoctorResponse("session/start", null, _context.GetUserAccount().GetUserName());
                return new SessionActiveState(_context);
            }
            else if (command == "stats/summary")
            {
                // TODO: Retrieve this data from the userstats buffer instead of the userstats file
                //_context.ResponseToPatient = ResponseClientData.GenerateSummaryRequest(_context.UserStatsBuffer);

                List<UserStat> allUserStats = _context.GetUserAccount().GetUserStats();

                SummaryStats sumStats = new SummaryStats(allUserStats);

                JsonObject myObj = System.Text.Json.JsonSerializer.Deserialize<JsonObject>(System.Text.Json.JsonSerializer.Serialize(sumStats));

                //JsonObject jsonUserStats = (JsonObject)JsonSerializer.Serialize<List<UserStat>>(allUserStats);
                //string jsonString = System.Text.Json.JsonSerializer.Serialize(allUserStats);

                //JsonObject jsonUserStats = JsonConvert.DeserializeObject<JsonObject>(jsonString);                

                //JsonObject summaryForDoc = JsonSerializer.Deserialize<JsonObject>(JsonSerializer.Serialize(allUserStats));

                //= ResponseClientData.GenerateSummaryRequest(jsonUserStats);
                _context.ResponseToPatient = null;
                _context.ResponseToDoctor = ResponseClientData.GenerateSummaryRequest(myObj);


            }
            else if (command == "chats/send")
            {
                string message = JsonUtil.GetValueFromPacket(packet, "data", "message").ToString();

                _context.ResponseToDoctor = ResponseClientData.DoctorChatSendResponse(message, _context.GetUserAccount().GetUserName());
            }
            return this;
        }
    }
}
