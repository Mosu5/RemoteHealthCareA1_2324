using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PatientApp.Commands
{
    internal class ClientFormatting
    {
        private static JsonObject BaseMessage(string command, JsonObject data)
        {
            return new JsonObject
            {
                { "command", command },
                { "data", data }
            };
        }

        public static JsonObject Login(string username, string password)
        {
            return BaseMessage("login", new JsonObject
            {
                { "username", username },
                { "password", password }
            });
        }

        public static JsonObject SendStats(double speed, int distance, int heartRate)
        {
            return BaseMessage("stats/send", new JsonObject
            {
                { "speed", speed },
                { "distance", distance },
                { "heartrate", heartRate }
            });
        }
    }
}
