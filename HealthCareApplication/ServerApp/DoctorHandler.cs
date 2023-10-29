using ServerApp.States;
using System;
using System.Runtime.Remoting.Contexts;
using System.Text.Json.Nodes;
using Utilities.Logging;

namespace ServerApp
{

    /// <summary>
    /// Gets and sets data for patients according to the incoming payload values
    /// This class is purely a one-way hatch for data from the Doctor - to - server - to - Patient  
    /// </summary>
    public class DoctorHandler
    {
        // Name of the patient
        public string PatientToRespondTo { get; set; }

        // Whatever the patient should receive
        public JsonObject ResponseValue { get; set; }

        public bool Handle(JsonObject packet)
        {
            // Get command and patient to send it to
            string command = JsonUtil.GetValueFromPacket(packet, "command").ToString();
            string userName = JsonUtil.GetValueFromPacket(packet, "data", "username").ToString();

            // Error handling
            if (command == null || userName == null)
            {
                string warningMsg = "Server recieved message, but command or username was NULL";
                Logger.Log(warningMsg, LogType.Warning);
                return false;
            }

            Console.WriteLine($"Command recieved from doc: {command} destined for patient {userName}");

            PatientToRespondTo = userName;

            // Handle responsevalue according to the incoming command from the doctor client
            switch (command)
            {
                // Note: Data object in responsevalue is empty. Client expects a data field but doesnt expect any values. Donut Wurry Boudda Ting
                case "session/start":
                    ResponseValue = new JsonObject { { "command", "session/start" }, {"data", new JsonObject() } };
                    break;
                case "session/stop":
                    ResponseValue = new JsonObject { { "command", "session/stop" }, { "data", new JsonObject() } };
                    break;
                case "stats/summary":
                    ResponseValue = new JsonObject { { "command", "stats/summary" }, { "data", new JsonObject() } };
                    break;
                case "session/pause":
                    ResponseValue = new JsonObject { { "command", "session/pause" }, { "data", new JsonObject() } };
                    break;
                case "session/resume":
                    ResponseValue = new JsonObject { { "command", "session/resume" }, { "data", new JsonObject() } };
                    break;
                case "chats/send":
                    string message = JsonUtil.GetValueFromPacket(packet, "data", "message").ToString();
                    ResponseValue = new JsonObject() { { "command", "chats/send" }, { "data", new JsonObject() { { "message", message } } } };
                    break;
                case "resistance/set":
                    string value = JsonUtil.GetValueFromPacket(packet, "data", "value").ToString();
                    ResponseValue = ResponseClientData.GenerateResistanceMessage(value);
                    break;
            }
            return true;
        }
    }
}
