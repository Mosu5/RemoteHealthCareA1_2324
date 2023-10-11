using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.Json.Nodes;
using Utilities.Communication;

namespace DoctorApp.Helpers
{
    internal class DoctorFormat
    {
        // BASE MESSAGES
        public static JsonObject BaseMessageEmpty(string command)
        {
            return BaseMessage(command, new JsonObject());
        }

        public static JsonObject BaseMessage(string command, JsonObject data)
        {
            return new JsonObject
            {
                { "command", command },
                { "data", data }
            };
        }

        public static JsonObject BaseMessageWithUsername(string command, string clientUsername)
        {
            return BaseMessage(command, new JsonObject()
            {
                { "username", clientUsername }
            });
        }

        // OTHER MESSAGES
        public static JsonObject LoginMessage(string username, string password)
        {
            return BaseMessage("login", new JsonObject
            {
                { "username", username },
                { "password", password }
            });
        }

        public static JsonObject LogoutMessage()
        {
            throw new NotImplementedException();
        }

        public static JsonObject SessionStartMessage(string clientUsername)
        {
            return BaseMessageWithUsername("session/start", clientUsername);
        }

        public static JsonObject SessionStopMessage(string clientUsername)
        {
            return BaseMessageWithUsername("session/stop", clientUsername);
        }

        public static JsonObject SessionPauseMessage(string clientUsername)
        {
            return BaseMessageWithUsername("session/pause", clientUsername);
        }

        public static JsonObject SessionResumeMessage(string clientUsername)
        {
            return BaseMessageWithUsername("session/resume", clientUsername);
        }

        public static JsonObject StatsSummaryMessage(string clientUsername)
        {
            return BaseMessageWithUsername("stats/summary", clientUsername);
        }

        public static JsonObject ChatsSendMessage(string patientUsername, string chatMessage)
        {
            return BaseMessage("chats/send", new JsonObject
            {
                { "username", patientUsername },
                { "message", chatMessage }
            });
        }

        public static JsonNode GetKey(JsonObject dataObject, string key)
        {
            if (dataObject.ContainsKey(key))
                return dataObject[key];
            else
                throw new CommunicationException($"The message did not contain the JSON key '{key}'.");
        }

        public static JsonNode[] GetKeys(JsonObject fullMessage, string expectedCommand, params string[] keys)
        {
            JsonObject dataObject = GetDataObject(fullMessage, expectedCommand);
            List<JsonNode> nodes = new List<JsonNode>();

            foreach (var key in keys)
            {
                if (dataObject.ContainsKey(key))
                    nodes.Add(dataObject[key]);
                else
                    throw new CommunicationException($"The message did not contain the JSON key '{key}'.");
            }

            return nodes.ToArray();
        }

        public static JsonObject GetDataObject(JsonObject receivedMessage, string expectedCommand)
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

            JsonObject dataObject = receivedMessage["data"].AsObject();

            return dataObject;
        }
    }
}
