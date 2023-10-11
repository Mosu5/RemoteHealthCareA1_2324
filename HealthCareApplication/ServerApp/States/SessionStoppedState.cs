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


        public IState Handle(JsonObject packet)
        {

            if (packet.ContainsKey("data"))
            {

                //checking if the packet has a valid format
                if (!packet.ContainsKey("command") || !packet.ContainsKey("data"))
                {
                    throw new FormatException("Json packet format corrupted!");
                }

                //extracting the needed part of the JsonObject from the packet
                JsonObject command = packet["command"] as JsonObject;
                JsonObject data = packet["data"] as JsonObject;

                //extracting the command from the command JsonObject
                string commandString = (string)command["command"];

                if (commandString = "")
                {
                    // Save data to file
                    // Data will be saved so client/doctor can recieve a stats summary later
                    this._context.SaveUserData();
                    return this;
                }
                if(data.ContainsKey("stats/summary"))
                { 
                    return new SessionIdle();
                }

            }
            else
            {
                throw new FormatException("Json packet format corrupted!");
            }
            return new SessionIdle();
        }





    }
}
