using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using PatientApp.DeviceConnection;
using PatientApp.Commands;
using Utilities.Communication;
using System.Threading.Tasks;

namespace PatientApp
{
    /// <summary>
    /// Initialize commands in constructor
    /// Handle commands based on incoming messages from server
    /// </summary>
    public class CommandHandler
    {
        private readonly ClientConn _clientConn;

        public CommandHandler(ClientConn clientConn)
        {
            _clientConn = clientConn;
        }
        
        /// <summary>
        /// Listens for incoming messages from the server, formats the response and then
        /// executes the matching command.
        /// </summary>
        /// <exception cref="CommunicationException">Anything that went wrong during communication</exception>
        public async Task Listen()
        {
            while (await _clientConn.ReceiveJson() is var message) // listening for message
            {
                await Console.Out.WriteLineAsync($"Received: {message}");

                // Check if the message has a command field
                if (!message.ContainsKey("command"))
                    throw new CommunicationException("The message did not contain the JSON key 'command'");

                // Check if the message has a data field
                if (!message.ContainsKey("data"))
                    throw new CommunicationException("The message did not contain the JSON key 'data'");

                // Get command and data field
                string command = message["command"].ToString();
                JsonObject dataObject = message["data"].AsObject();

                // Check if the command exists in the dictionary and execute it
                ExecuteReceivedCommand(command, dataObject);
            }
        }

        /// <summary>
        /// Handles the received trainer data and sends it to the server
        /// </summary>
        public void OnReceiveData(object sender, Statistic stat) 
        {
            Console.WriteLine("======= OnReceiveData called");
            SendStats sendStats = new SendStats(stat, _clientConn);
            sendStats.Execute();
        }

        /// <summary>
        /// Executes a command which the server sent to the patient.
        /// </summary>
        /// <exception cref="CommunicationException">When the server sent an invalid command</exception>
        private void ExecuteReceivedCommand(string command, JsonObject dataObject)
        {
            try
            {
                var commands = new Dictionary<string, ISessionCommand>()
                {
                    { "login", new LoginResponse(dataObject) },
                    { "stats/summary", new Summary(dataObject) },
                    { "session/start", new SessionStart(OnReceiveData) },
                    { "session/pause", new SessionPause(OnReceiveData) },
                    { "session/resume", new SessionResume(OnReceiveData) },
                    { "session/stop", new SessionStop(OnReceiveData) },
                };

                commands[command].Execute();
            }
            catch (KeyNotFoundException)
            {
                throw new CommunicationException($"No command called {command} has been registered for listening to server messages.");
            }
        }

        /// <summary>
        /// Executes a command which the patient can trigger, for example logging in.
        /// </summary>
        /// <exception cref="CommunicationException">When the patient wants to send an invalid command</exception>
        public void ExecuteCommandToSend(string command, JsonObject dataObject)
        {
            try
            {
                var commands = new Dictionary<string, ISessionCommand>()
                {
                    { "login", new Login(dataObject, _clientConn) },
                    
                };

                commands[command].Execute();
            }
            catch (KeyNotFoundException)
            {
                throw new CommunicationException($"No command called {command} has been registered for sending messages to the server.");
            }
        }
    }
}