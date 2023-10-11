using Microsoft.SqlServer.Server;
using ServerApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    /// <summary>
    /// State in when a client has connected with the server. 
    /// </summary>
    internal class LoginState : IState
    {
        //TODO current username and password are for testing
        private Server server;

        private ServerContext context;
        public LoginState(ServerContext context)
        {
            this.context = context;
        }

        public IState Handle(JsonObject packet)
        {
            //extracting the needed values from packet
            string username = JsonUtil.GetValueFromPacket(packet, "data", "username") as string;
            string password = JsonUtil.GetValueFromPacket(packet, "data", "password") as string;


            Console.WriteLine("Login recieved data: " + username + "    " + password);

            if (!server.users.Any())
            {
                foreach (UserAccount account in server.users)
                {
                    if (account.GetUserName() == username && account.GetPassword() == password)
                    {
                        context.ResponseToClient = ApproveLogin();
                        //context.SetNextState(new SessionActiveState(context));
                        return new SessionActiveState(context);
                    }
                    else
                    {
                        Console.WriteLine("IM INSIDE ELSE BLOCK");
                        context.ResponseToClient = RefuseLogin();
                        //context.SetNextState(new SessionActiveState(context));
                        return new CreateAccountState(context);
                    }
                }
            }
            else
            {
                return new CreateAccountState(context);
            }
            //Login Failed so it stays in LoginState
            return this;
        }

        /// <summary>
        /// Method to send status of login
        /// </summary>
        /// <returns></returns>
        private JsonObject RefuseLogin()
        {
            return new JsonObject
            {
                {"command", "login" },
                {"data", new JsonObject
                    {
                        {"status", "error"}
                    }
                }
            };
        }

        /// <summary>
        /// Mehtod to send status login
        /// </summary>
        /// <returns></returns>
        private JsonObject ApproveLogin()
        {
            return new JsonObject
            {
                {"command", "login" },
                {"data", new JsonObject
                    {
                        {"status", "ok"}
                    }
                }
            };
        }
    }
}
