using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;
using Utilities.Logging;

namespace PatientWPFApp.PatientLogic
{
    internal class RequestHandler
    {
        // Patient ViewModel events
        public static event EventHandler<bool>? LoggedIn;
        public static event EventHandler<string>? ReceivedChat;

        public static async Task Listen()
        {
            if (!await ClientConn.ConnectToServer("127.0.0.1", 8888))
                throw new CommunicationException("Could not connect to the server.");

            while (await ClientConn.ReceiveJson() is var message) // listening for message
            {
                Logger.Log($"Received: {message}", LogType.Debug);

                // Get command and data field from message
                (string command, JsonObject dataObject) = GetCommandAndData(message);

                Logger.Log($"Was not response, but was {command}", LogType.GeneralInfo);

                switch (command)
                {
                    // TODO fix server side issue that sometimes sends login after the patient sent for example chats/send
                    case "login":
                        bool loggedIn = PatientFormat.GetKey(dataObject, "status").ToString().Equals("ok");
                        LoggedIn?.Invoke(nameof(RequestHandler), loggedIn);
                        break;
                    case "chats/send":
                        ReceivedChat?.Invoke(nameof(RequestHandler), PatientFormat.GetKey(dataObject, "message").ToString());
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
