using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ServerApp.States
{
    internal class SessionIdle : IState
    {

        private ServerContext _context;
        public SessionIdle(ServerContext context)
        {
            this._context = context;
        }

        public IState Handle(JsonObject packet)
        {
            string command = JsonUtil.GetValueFromPacket(packet, "command").ToString();

            if (command == "session/start")
            {
                // Mark user as active in session
                _context.GetUserAccount().hasActiveSession = true;
                _context.isSessionActive = true;
                _context.ResponseToClient = ResponseClientData.GenerateResponse("session/start", null, "ok"); 
                return new SessionActiveState(_context);
            }
            else if (command == "stats/summary")
            {
                // To do: Retrieve this data from the userstats buffer instead of the userstats file
                List<UserStat> allUserStats = _context.GetUserAccount().GetUserStats();

                string json = System.Text.Json.JsonSerializer.Serialize(allUserStats);

                _context.ResponseToClient = ResponseClientData.GenerateSummaryRequest(json);
                // Reset userbuffer for next session
            }
            else if(command == "stats/history"){
                //List<List<UserStat>> history = _context.GetUserAccount().GetUserStats();
                //JsonObject json = (JsonObject)JsonSerializer.Serialize(history);
                //_context.ResponseToClient = ResponseClientData.GenerateSummaryRequest(json);
            }
            return this;

        }

    }
}
