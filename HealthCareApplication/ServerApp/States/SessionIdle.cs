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
            string command = JsonUtil.GetValueFromPacket(packet, "command").ToString();

            if (command == "session/start")
            {
                // Mark user as active in session
                context.GetUserAccount().hasActiveSession = true;
                context.isSessionActive = true;
                context.ResponseToClient = SessionStartOk(); 
                return new SessionActiveState(context);
            }
            if (command == "stats/summary")
            {
                context.ResponseToClient = SendSummary();
            }
            return this;

        }

        JsonObject SessionStartOk()
        {
            return new JsonObject
            {
                 {"command", "session/start" },
                {"data", new JsonObject{
                    { "status", "ok" }
                }
                }
            };
        }

        JsonObject SendSummary()
        {
            return new JsonObject
            {
                 {"command", "stats/summary" },
                {"data", new JsonArray{
                    context.userStats
                }
                }
            };
        }
    }
}
