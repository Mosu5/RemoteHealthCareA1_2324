using ServerApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class SessionPausedState : IState
    {
        private ServerContext context;

        public SessionPausedState(ServerContext context)
        {
            this.context = context;
        }

        public IState Handle(JsonObject packet)
        {
            //Getting needed values from the packet
            string command = (string) JsonUtil.GetValueFromPacket(packet, "command");
            string username = (string)JsonUtil.GetValueFromPacket(packet, "data", "username");

            //checking if the command equals session/pause
            if (command == "session/pause")
            {
                //extracting the username after check;
                foreach (UserAccount account in Server.users)
                {
                    if (username.Equals(account.GetUserName()))
                    {
                        account.isPaused = true;
                        context.ResponseToClient = TriggerClientPause();
                        return this;
                    }
                    else
                        throw new Exception("User account not found");
                    
                }
            }
            else if (command == "session/resume")
            {
                foreach (UserAccount account in Server.users)
                {
                    if (username.Equals(account.GetUserName()))
                    {
                        account.isPaused = false;
                        context.ResponseToClient = TriggerClientPause();
                        return this;
                    }
                    else
                        throw new Exception("User account not found");
                    
                }

                return new SessionActiveState(context);
            }
            return this;
        }

        private JsonObject TriggerClientPause()
        {
            return new JsonObject
            {
                {"command", "session/pause" }
            };
        }
    }
}
