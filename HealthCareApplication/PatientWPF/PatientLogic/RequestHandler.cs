using Newtonsoft.Json.Linq;
using PatientWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Logging;

namespace PatientWPFApp.PatientLogic
{
    internal class RequestHandler
    {
        // Patient ViewModel events
        public static event EventHandler<bool> LoggedIn;
        public static event EventHandler<string> ReceivedChat;
        public static event EventHandler<bool> SessionStarted;
        public static event EventHandler<bool> SessionStopped;
        public static event EventHandler<bool> SummaryRequested;

        public static async Task Listen()
        {
            if (!await ClientConn.ConnectToServer("127.0.0.1", 8888))
                throw new CommunicationException("Could not connect to the server.");

            while (await ClientConn.ReceiveJson() is var message) // listening for message
            {
                Logger.Log($"Received: {message}", LogType.Debug);

                // Get command and data field from message
                (string command, JObject dataObject) = GetCommandAndData(message);

                Logger.Log($"Was not response, but was {command}", LogType.GeneralInfo);

                switch (command)
                {
                    // TODO fix server side issue that sometimes sends login after the patient sent for example chats/send
                    case "login":
                        bool loggedIn = ((string)PatientFormat.GetKey(dataObject, "status")).Equals("ok");
                        LoggedIn.Invoke(nameof(RequestHandler), loggedIn);
                        break;
                    case "chats/send":
                        ReceivedChat.Invoke(nameof(RequestHandler), $"Doctor: {(string)PatientFormat.GetKey(dataObject, "message")}");
                        break;
                    case "session/start":
                        SessionStarted?.Invoke(nameof(RequestHandler), true);
                        break;
                    case "session/stop":
                        SessionStopped?.Invoke(nameof(RequestHandler), true);
                        break;
                    case "stats/summary":
                        SummaryRequested?.Invoke(nameof(RequestHandler), true);
                        break;
                    default:
                        Logger.Log($"Cannot process command '{command}'.", LogType.Warning);
                        break;
                }
            }
        }

        /// <summary>
        /// Retrieves the command field (string) and the data field (JObject) from
        /// a message.
        /// </summary>
        /// <exception cref="CommunicationException">If the JSON message did not have the required fields</exception>
        public static (string, JObject) GetCommandAndData(JObject message)
        {
            // Check if the message has a command field
            if (!message.ContainsKey("command"))
                throw new CommunicationException("The message did not contain the JSON key 'command'");

            // Check if the message has a data field
            if (!message.ContainsKey("data"))
                throw new CommunicationException("The message did not contain the JSON key 'data'");

            // Get command and data field
            string command = message["command"].ToString();
            JObject dataObject = (JObject)message["data"];

            // Return a TjOePEl
            return (command, dataObject);
        }
    }
}
