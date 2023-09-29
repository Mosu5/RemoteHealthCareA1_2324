using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using PatientApp.Commands;
using Utilities.Communication;

namespace PatientApp
{
    public class MessageHandler
    {
        private Dictionary<String, ISessionCommand> _commands;

        public MessageHandler()
        {
            // todo add all commands to dictionary
            // add client to constructor, create client in main
            _commands = new Dictionary<string, ISessionCommand>()
            {
                { "login", new LoginResponse() },
                // { "session/start", new SessionStart() },
                { "stats/send", new SendStats() }
            };
        }

        public void Handle(JsonObject message)
        {
            // Check if the message has a command field and that field has the type of object
            if (!message.ContainsKey("command") || !(message["command"] is JsonObject)) return;

            // Check if the message has a data field
            if (!message.ContainsKey("data")) return;

            string command = message["command"].ToString();
            JsonObject data = message["data"].AsObject();
            
            // Check if the command exists in the dictionary and execute it
            _commands[command].Execute(data);
        }
    }
}