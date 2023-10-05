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
            //checks if the packet is contains the correct data
            if (packet.ContainsKey("data"))
            {
                //extracting the needed part of the JsonObject
                JsonObject data = packet["data"] as JsonObject;
                Console.WriteLine("Login recieved data: " + data);

                //extracting username and password
                string username = (string)data["username"];
                string password = (string)data["password"];

                //checks if the packet is contains the correct data
                if (data.ContainsKey("username") && data.ContainsKey("password"))
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
                            context.ResponseToClient = RefuseLogin();
                            //context.SetNextState(new SessionActiveState(context));
                            return this;
                        }
                    }
                }
                else
                {
                    throw new FormatException("Converting data field to JsonObject failed");
                }
            }
            else
            {
                throw new FormatException("Json packet format corrupted!");
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
