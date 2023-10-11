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

        public CreateAccountState(ServerContext context)
        {
            this.context = context;
        }

        public IState Handle(JsonObject packet)
        {
            //extracting username and password
            string username = JsonUtil.GetValueFromPacket(packet, "data", "username").ToString();
            string password = JsonUtil.GetValueFromPacket(packet, "data", "password").ToString();

            //extracting the needed part of the JsonObject
            Console.WriteLine("Create account recieved data: " + username + "   " + password);

            //checks if the packet is contains the correct data

            foreach (UserAccount account in Server.users)
            {
                if (account.GetUserName() == username)
                {
                    context.ResponseToClient = AccCreationFailed("Username is already being used by an account. Please Login or use another username to create an account");
                    return new LoginState(context);
                }
                else
                {
                    context.ResponseToClient = AccSuccesfullCreated();
                    Server.users.Add(new UserAccount(username, password));
                    return new SessionActiveState(context);
                }
            }
            //Account Creation Failed so it stays in CreateAccountState
            context.ResponseToClient = AccCreationFailed("Account creation Failed because");
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
        private JsonObject AccCreationFailed(string message)
        {
            return new JsonObject
            {
                {"command", "create account" },
                {"data", new JsonObject
                    {
                        {"status", "error"},
                        {"description", message}
                    }
                }
            };
        }
    }
}
