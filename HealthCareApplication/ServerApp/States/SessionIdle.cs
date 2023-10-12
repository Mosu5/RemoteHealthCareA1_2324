using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

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
                _context.ResponseToClient = ResponseDataForClient.GenerateResponse("session/start",null,"ok");
                return new SessionActiveState(_context);
            }
            if (command == "stats/summary")
            {
                _context.ResponseToClient = ResponseDataForClient.GenerateSummaryRequest(_context.userStatsBuffer);
            }
            return this;

        }
    }
}
