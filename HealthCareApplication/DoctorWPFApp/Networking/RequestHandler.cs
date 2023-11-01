using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Utilities.Logging;
using Utilities.Communication;

namespace DoctorWPFApp.Networking
{
    public class RequestHandler
    {
        // Doctor ViewModel events
        public static event EventHandler<bool>? LoggedIn;
        public static event EventHandler<string[]>? ReceivedPatients;
        public static event EventHandler<Statistic>? ReceivedStat;
        public static event EventHandler<string>? ReceivedChat;
        public static event EventHandler<string>? ReceivedSummary;
        public static event EventHandler<string> SessionStarted;
        public static event EventHandler<string> SessionStopped;

        public static async Task Listen()
        {
            if (!await ClientConn.ConnectToServer("127.0.0.1", 8888))
                throw new CommunicationException("Could not connect to the server.");

            while (await ClientConn.ReceiveJson() is var message) // listening for message
            {
                if (message == null) return;

                Logger.Log($"Received: {message}", LogType.Debug);

                // Get command and data field from message
                (string command, JsonObject dataObject) = GetCommandAndData(message);

                Logger.Log($"Was not response, but was {command}", LogType.GeneralInfo);

                switch (command)
                {
                    case "login":
                        bool loggedIn = DoctorFormat.GetKey(dataObject, "status").ToString().Equals("ok");
                        LoggedIn?.Invoke(nameof(RequestHandler), loggedIn);
                        break;
                    case "session/getUsers":
                        JsonObject receivedPatients = DoctorFormat.GetKey(dataObject, "patients").AsObject();
                        string usernamesJson = DoctorFormat.GetKey(receivedPatients, "usernames").ToString();

                        // Deserialize the JSON string into a string array
                        string[] usernames = JsonConvert.DeserializeObject<string[]>(usernamesJson)!;

                        ReceivedPatients?.Invoke(nameof(RequestHandler), usernames);
                        break;
                    case "chats/send":
                        ReceivedChat?.Invoke(nameof(DoctorFormat), DoctorFormat.GetKey(dataObject, "message").ToString());
                        break;
                    case "stats/send":
                        JsonObject statObject = DoctorFormat.GetKey(dataObject, "stats").AsObject();

                        JsonDocument jsonDoc = JsonDocument.Parse(statObject.ToString());

                        // Get the decimal value from the JsonObject
                        decimal originalValue = jsonDoc.RootElement.GetProperty("speed").GetDecimal();

                        string username = DoctorFormat.GetKey(dataObject, "username").ToString();
                        // Round the decimal value to two decimal places
                        decimal roundedValue = Math.Round(originalValue / 3.6M, 2);

                        ReceivedStat?.Invoke(nameof(DoctorFormat), new Statistic
                        (
                            (double)roundedValue,
                            int.Parse(DoctorFormat.GetKey(statObject, "distance").ToString()),
                            int.Parse(DoctorFormat.GetKey(statObject, "heartrate").ToString()),
                            username
                        ));
                        break;
                    case "stats/summary":
                        ReceivedSummary?.Invoke(nameof(DoctorFormat), DoctorFormat.GetKey(dataObject, "stats").ToString());
                        break;
                    case "session/start":
                        SessionStarted?.Invoke(nameof(RequestHandler), DoctorFormat.GetKey(dataObject, "username").ToString());
                        break;
                    case "session/stop":
                        SessionStopped?.Invoke(nameof(RequestHandler), DoctorFormat.GetKey(dataObject, "username").ToString());                  
                        break;
                    default:
                        Logger.Log($"Cannot process command '{command}'.", LogType.Warning);
                        break;
                }
            }
        }

        /// <summary>
        /// Retrieves the command field (string) and the data field (JsonObject) from
        /// a message.
        /// </summary>
        /// <exception cref="CommunicationException">If the JSON message did not have the required fields</exception>
        public static (string, JsonObject) GetCommandAndData(JsonObject message)
        {
            // Check if the message has a command field
            if (!message.ContainsKey("command"))
                throw new CommunicationException("The message did not contain the JSON key 'command'");

            // Check if the message has a data field
            if (!message.ContainsKey("data"))
                throw new CommunicationException("The message did not contain the JSON key 'data'");

            // Get command and data field
            string command = message["command"].ToString();
            JsonObject dataObject = message["data"].AsObject();

            // Return a TjOePEl
            return (command, dataObject);
        }
    }
}
