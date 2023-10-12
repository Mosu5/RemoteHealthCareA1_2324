﻿using System;
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

            string command = JsonUtil.GetValueFromPacket(packet, "command").ToString();
         
            if (command == "stats/send")
            {
                double speed = (double)JsonUtil.GetValueFromPacket(packet, "data", "speed");
                int distance = (int)JsonUtil.GetValueFromPacket(packet, "data", "distance");
                byte heartRate = (byte)JsonUtil.GetValueFromPacket(packet, "data", "heartrate");

                // Save data in server
                BufferUserData(speed, distance, heartRate);
                return this; // Stay in this state to recieve more data

            }
            else if (packet.ContainsKey("session/stop"))
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
