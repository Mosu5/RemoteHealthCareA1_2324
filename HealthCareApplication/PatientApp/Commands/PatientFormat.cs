using System.Text.Json.Nodes;

namespace PatientApp.Commands
{
    internal class PatientFormat
    {
        public static JsonObject BaseMessage(string command, JsonObject data)
        {
            return new JsonObject
            {
                { "command", command },
                { "data", data }
            };
        }

        public static JsonObject LoginMessage(string username, string password)
        {
            return BaseMessage("login", new JsonObject
            {
                { "username", username },
                { "password", password }
            });
        }

        public static JsonObject SendStatsMessage(double speed, int distance, int heartRate)
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
