using RHSandbox.Commands;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace PatientApp
{
    internal class Program
    {
        private static ISessionCommand _command;

        static async Task Main(string[] args)
        {
            try
            {
                if (await DataTransfer.ConnectToServer("127.0.0.1", 6969))
                    await Run();
            }
            catch (CommunicationException ex)
            {
                await Console.Out.WriteLineAsync($"CommunicationException: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static async Task Run()
        {
            // Initialize command to be login
            _command = new Login();
            _command.Execute(null);

            // Declare a variable inside while condition which listens for inbound JSON messages.
            // Loop blocks until a new message is received.
            while (await DataTransfer.ReceiveJson() is var message)
            {
                // Check if the message has a command field and that field has the type of object
                if (!message.ContainsKey("command") || !(message["command"] is JsonObject)) continue;

                // Check if the message has a data field
                if (!message.ContainsKey("data")) continue;

                string commandField = message["command"].ToString();
                JsonObject dataField = message["data"].AsObject();

                // Set and execute command based on commandField
                switch (commandField)
                {
                    case "login":
                        _command = new Login();
                        break;
                    case "session/start":
                        _command = new SessionStart();
                        break;
                    case "stats/send":
                        _command = new SendStats();
                        break;
                    default:
                        _command = null;
                        throw new CommunicationException("Unknown command received: " + commandField);
                }
                _command?.Execute(dataField);
            }
        }
    }
}