using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class SessionIdle : IState
    {

        private ServerContext context;
        public SessionIdle(ServerContext context)
        {
            this.context = context;
        }

        public IState Handle(JsonObject packet)
        {
            string command = (string)JsonUtil.GetValueFromPacket(packet, "command");

            if (command == "session/start")
            {
                // Mark user as active in session
                context.GetUserAccount().hasActiveSession = true;
                context.isSessionActive = true;
                return new SessionActiveState(context);
            }
            if (command == "stats/summary")
            {
                
            }
            return this;

        }

        JsonObject GetSummary()
        {
            return new JsonObject
            {
                // {"command", "stats/summary" },
                //{"data", n}
            };
        }
    }
}
