using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class CreateAccountState : IState
    {
        private ServerContext context;
        private Server server;

        public CreateAccountState(ServerContext context)
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
                Console.WriteLine("Create account recieved data: " + data);
                
                //extracting username and password
                string username = (string)data["username"];
                string password = (string)data["password"];

                //checks if the packet is contains the correct data
                if (data.ContainsKey("username") && data.ContainsKey("password"))
                {
                    foreach (UserAccount account in Server.users)
                    {
                        if (account.GetUserName() == username)
                        {
                            context.ResponseToClient = AccCreationFailed();
                            return new LoginState(context);
                        }else 
                        {
                            context.ResponseToClient = AccSuccesfullCreated();
                            Server.users.Add(new UserAccount(username,password));
                            return new SessionActiveState(context);
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
            //Account Creation Failed so it stays in CreateAccountState
            return this;
        }

        /// <summary>
        /// Method returns status of account creation.
        /// </summary>
        /// <returns></returns>
        private JsonObject AccSuccesfullCreated()
        {
            return new JsonObject
            {
                {"command", "create account" },
                {"data", new JsonObject
                    {
                        {"status", "ok"}
                    }
                }
            };
        }

        /// <summary>
        /// Method returns status of account creation.
        /// </summary>
        /// <returns></returns>
        private JsonObject AccCreationFailed()
        {
            return new JsonObject
            {
                {"command", "create account" },
                {"data", new JsonObject
                    {
                        {"status", "error"},
                        {"description", "username and/or password are not correct bozozozo" }
                    }
                }
            };
        }
    }
}
