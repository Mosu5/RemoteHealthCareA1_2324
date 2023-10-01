using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using PatientApp.BikeConnection;
using PatientApp.Commands;
using Utilities.Communication;

namespace PatientApp
{
    /// <summary>
    /// Initialize commands in constructor
    /// Handle commands based on incoming messages from server
    /// </summary>
    public class CommandHandler
    {
        private Dictionary<String, ISessionCommand> _commands;
        private ClientConn _conn;

        public CommandHandler(ClientConn conn, EventHandler<Statistic> onReceiveDataClient)
        {
            _conn = conn;

            // todo add all commands to dictionary
            // add client to constructor, create client in main


            _commands = new Dictionary<string, ISessionCommand>()
            {
                 { "login", new LoginResponse() }, // TODO: clean Login() and create LoginResponse()
                 { "session/start", new SessionStart(onReceiveDataClient, OnReceiveData) }, // pass out necessary event handlers
            };
        }

        public void Handle(JsonObject message)
        {
            // Check if the message has a command field and that field has the type of object
            if (!message.ContainsKey("command")) return;

            // Check if the message has a data field
            if (!message.ContainsKey("data")) return;

            string command = message["command"].ToString();
            JsonObject data = message["data"].AsObject();
            
            // Check if the command exists in the dictionary and execute it
            _commands[command].Execute(data, _conn);
        }

        public void OnReceiveData(object sender, Statistic stat) 
        {
            //TODO: fill Command with stat data and send to server with Execute
            var SendStats = new SendStats();
        }
    }
}