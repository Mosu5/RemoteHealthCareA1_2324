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
        private ServerContext _context;

        public SessionPausedState(ServerContext context)
        {
            this._context = context;
        }

        public IState Handle(JsonObject packet)
        {
            //Getting needed values from the packet
            string command = (string)JsonUtil.GetValueFromPacket(packet, "command");
            string username = (string)JsonUtil.GetValueFromPacket(packet, "data", "username");

            //checking if the command equals session/pause
            if (command == "session/pause")
            {
                foreach (UserAccount account in Server.users)
                {
                    if (username.Equals(account.GetUserName()))
                    {
                        Console.WriteLine("session/pause for account");
                        account.isPaused = true;
                        _context.ResponseToClient = ResponseClientData.GenerateResponse("session/pause", null, "ok"); ;
                        return this;
                    }
                    else
                    {
                        throw new Exception("User account not found");
                    }
                }
            }
            else if (command == "session/resume")
            {
                foreach (UserAccount account in Server.users)
                {
                    if (username.Equals(account.GetUserName()))
                    {
                        account.isPaused = false;
                        _context.ResponseToClient = ResponseClientData.GenerateResponse("session/resume", null, "ok"); ;
                        return this;
                    }
                    else
                    {
                        throw new Exception("User account not found");
                        
                    }
                }

                return new SessionActiveState(_context);
            }
            return this;
        }



    }
}
