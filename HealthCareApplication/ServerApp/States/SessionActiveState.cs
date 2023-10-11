using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class SessionActiveState : IState
    {
        private ServerContext context;
        public SessionActiveState(ServerContext context)
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

            //extracting the command from the command JsonObject
            string _command = (string)command["command"];

            if (packet.ContainsKey("session/start")) 
            {
                // Mark user as active in session
                context.GetUserAccount().hasActiveSession = true;
                context.isSessionActive = true;
                return this;
            }
            else if (packet.ContainsKey("stats/send"))
            {
                if (packet.ContainsKey("data"))
                {
                    JsonObject data = packet["data"] as JsonObject;
                    Console.WriteLine("Login recieved data: " + data);

                    double speed = (double)data["speed"];
                    int distance = (int)data["distance"];
                    byte heartRate = (byte)data["heartrate"];

                    // Save data in server buffer
                    BufferUserData(speed, distance, heartRate);

                    return this;
                }
            }
            else if(packet.ContainsKey("session/stop"))
            {
                return new SessionStoppedState(this.context);
            }
            //Login Failed so it stays in LoginState
            return this;

        }

        private void BufferUserData(double speed, int distance, byte heartrate)
        {
            this.context.userStats.Add(new UserStat(speed, distance, heartrate));
        }


    }
}
