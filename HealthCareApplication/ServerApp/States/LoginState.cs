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
        private string _username = "bob";
        private string _password= "bob";

        private ServerContext context;
        public LoginState(ServerContext context)
        {
            this.context = context;
        }

        public void Handle(JsonObject packet)
        {
            if (packet.ContainsKey("data"))
            {
                JsonObject data = packet["data"] as JsonObject;
                Console.WriteLine("Login recieved data: " + data);

                string username= (string)data["username"];
                string password = (string)data["password"];
                if(data.ContainsKey("username") && data.ContainsKey("password"))
                {
                    if (username == _username && password == _password)
                    {
                        // Update response to the client so the server can retrieve response and send to client
                        context.ResponseToClient = ApproveLogin();
                        
                        // To Do: Implement creating of UserAccount in a different command
                        UserAccount userAccount = new UserAccount(username, password);
                        context.SetNewUser(userAccount); // Yay
                        context.SetNextState(new SessionActiveState(context));
                    }
                    else
                    {
                        context.ResponseToClient = RefuseLogin();
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
        }

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
