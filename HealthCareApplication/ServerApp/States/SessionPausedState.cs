using System;
using System.Text.Json.Nodes;

namespace ServerApp.States
{
    internal class SessionPausedState : IState
    {
        private readonly ServerContext _context;

        public SessionPausedState(ServerContext context)
        {
            _context = context;
        }

        public IState Handle(JsonObject packet)
        {
            //Getting needed values from the packet
            string command = JsonUtil.GetValueFromPacket(packet, "command").ToString();

            //checking if the command equals session/pause
            if (command == "session/resume")
            {
                _context.GetUserAccount().IsPaused = false;
                _context.GetUserAccount().HasActiveSession = true;
                _context.ResponseToPatient = ResponseClientData.GenerateResponse("session/resume", null, "ok");
                _context.IsSessionActive = true;
                return new SessionActiveState(_context);
            }
            return this;
        }
    }
}
