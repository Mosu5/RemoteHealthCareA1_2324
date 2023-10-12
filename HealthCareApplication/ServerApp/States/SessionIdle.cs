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
                context.GetUserAccount().hasActiveSession = true;
                context.isSessionActive = true;
                context.ResponseToClient = ResponseClientData.GenerateResponse("session/start", null, "ok"); 
                return new SessionActiveState(context);
            }
            if (command == "stats/summary")
            {
                context.ResponseToClient = ResponseClientData.GenerateSummaryRequest(context.userStatsBuffer); ;
            }
            return this;

        }

    }
}
