using Newtonsoft.Json.Linq;
using PatientApp.DeviceConnection;
using Utilities;
using System;
using Utilities.Communication;

namespace PatientWPFApp.PatientLogic
{
    internal class PatientFormat
    {
        public static JToken GetKey(JObject dataObject, string key)
        {
            if (dataObject.ContainsKey(key))
                return dataObject[key];
            else
                throw new CommunicationException($"The message did not contain the JSON key '{key}'.");
        }

        public static JObject GetDataObject(JObject receivedMessage, string expectedCommand)
        {
            // Check if the message has a command field
            if (!receivedMessage.ContainsKey("command"))
                throw new CommunicationException("The message did not contain the JSON key 'command'.");

            // Check if the message has a data field
            if (!receivedMessage.ContainsKey("data"))
                throw new CommunicationException("The message did not contain the JSON key 'data'.");

            // Validate command field
            string receivedCommand = receivedMessage["command"].ToString();

            if (!expectedCommand.Equals("") && !receivedCommand.Equals(expectedCommand))
                throw new CommunicationException($"Expected command '{expectedCommand}' but received '{receivedCommand}'.");

            JObject dataObject = (JObject)receivedMessage["data"];

            return dataObject;
        }

        // BASE MESSAGES
        public static JObject BaseMessageEmpty(string command)
        {
            return BaseMessage(command, new JObject());
        }

        public static JObject BaseMessage(string command, JObject data)
        {
            return new JObject
            {
                { "command", command },
                { "data", data }
            };
        }

        // OTHER MESSAGES
        public static JObject LoginMessage(string username, string password)
        {
            return BaseMessage("login", new JObject
            {
                { "username", username },
                { "password", password }
            });
        }

        public static JObject LogoutMessage()
        {
            throw new NotImplementedException();
        }

        public static JObject SessionStartMessage()
        {
            return BaseMessageEmpty("session/start");
        }

        public static JObject SessionStopMessage()
        {
            return BaseMessageEmpty("session/stop");
        }

        public static JObject SessionPauseMessage()
        {
            return BaseMessageEmpty("session/pause");
        }

        public static JObject SessionResumeMessage()
        {
            return BaseMessageEmpty("session/resume");
        }

        public static JObject StatsSendMessage(Statistic statistic)
        {
            return BaseMessage("stats/send", new JObject()
            {
                { "speed", statistic.Speed },
                { "distance", statistic.AccumulatedDistance },
                { "heartrate", statistic.HeartRate },
                { "rrInterval", statistic.RrIntervals.ToString() }
            });
        }

        public static JObject StatsSummaryMessage()
        {
            return BaseMessageEmpty("stats/summary");
        }

        public static JObject ChatsSendMessage(string chatMessage)
        {
            return BaseMessage("chats/send", new JObject
            {
                { "message", chatMessage }
            });
        }
    }
}
