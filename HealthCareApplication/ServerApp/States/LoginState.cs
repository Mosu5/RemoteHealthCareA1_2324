using Microsoft.SqlServer.Server;
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
        private string _password= "bob123";

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
                        context.SetNextState(new SessionActiveState(context));
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
    }
}
