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
                _context.ResponseToDoctor = ResponseClientData.GenerateDoctorResponse("session/start", null, _context.GetUserAccount().GetUserName());
                return new SessionActiveState(_context);
            }
            else if (command == "stats/summary")
            {
                // TODO: Retrieve this data from the userstats buffer instead of the userstats file
                _context.ResponseToPatient = ResponseClientData.GenerateSummaryRequest(_context.UserStatsBuffer);
                // Reset userbuffer for next session
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
