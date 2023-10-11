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
        private Server server;

        public SessionPausedState(ServerContext context)
        {
            this.context = context;
        }

        public IState Handle(JsonObject packet)
        {

            //checking if the packet has a valid format
            if (!packet.ContainsKey("command") || !packet.ContainsKey("data"))
            {
                throw new FormatException("Json packet format corrupted!");
            }

            //extracting the needed part of the JsonObject from the packet
            JsonObject command = packet["command"] as JsonObject;
            JsonObject data = packet["data"] as JsonObject;

            Console.WriteLine("Pause data received: " + data + "\n" + command);

            //extracting the command from the command JsonObject
            string _command = (string)command["command"];

            //checking if the command equals session/pause en extra checking for the format of the data JsonObject
            if (_command == "session/pause" && data.ContainsKey("username"))
            {
                //extracting the username after check;
                string username = (string)data["username"];

                foreach (UserAccount account in server.users)
                {
                    if (username.Equals(account.GetUserName()))
                    {
                        account.isPaused = true;
                        context.ResponseToClient = TriggerClientPause();
                        return this;
                    }
                    else
                    {
                        throw new Exception("User account not found");
                    }
                }
            }
            else if (_command == "session/resume")
            {
                foreach (UserAccount account in server.users)
                {

                    string username = (string)data["username"];

                    if (username.Equals(account.GetUserName()))
                    {
                        account.isPaused = false;
                        context.ResponseToClient = TriggerClientPause();
                        return this;
                    }
                    else
                    {
                        throw new Exception("User account not found");
                    }
                }

                return new SessionActiveState(context);
            }
            else
            {
                throw new FormatException("");
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
