using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class SessionStoppedState : IState
    {
        private ServerContext _context;

        public SessionStoppedState(ServerContext serverContext) 
        {
        this._context = serverContext;
        }


        public void Handle(JsonObject packet)
        {

            if (packet.ContainsKey("data"))
            {
                JsonObject data = packet["data"] as JsonObject;
                Console.WriteLine("Login recieved data: " + data);

                //if (this._context.)
                //{

                //}

                //if (data.ContainsKey("username") && data.ContainsKey("password"))
                //{
                //    if (username == _username && password == _password)
                //    {
                //        // Update response to the client so the server can retrieve response and send to client
                //        context.ResponseToClient = ApproveLogin();
                //        context.SetNextState(new SessionActiveState(context));
                //    }
                //    else
                //    {
                //        context.ResponseToClient = RefuseLogin();
                //    }


                //}
                //else
                //{
                //    throw new FormatException("Converting data field to JsonObject failed");
                //}

            }
            else
            {
                throw new FormatException("Json packet format corrupted!");
            }
        }
    }
}
