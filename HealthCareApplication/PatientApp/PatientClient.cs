using PatientApp.DeviceConnection;
using System;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Communication;

namespace PatientApp
{
    public class PatientClient
    {
        // Helper for sending and receiving network traffic
        private readonly ClientConn _clientConn;

        // Manages commands
        private readonly CommandHandler _commandHandler;

        // Connection to the trainer and hrm
        private readonly DeviceManager _deviceManager;

        public PatientClient()
        {
            _clientConn = new ClientConn("127.0.0.1", 8888);
            _deviceManager = new DeviceManager();
            _commandHandler = new CommandHandler(_clientConn, _deviceManager.OnReceiveData);
        }

        static async Task Main(string[] args)
        {
            // Create a new instance of this class to access the non-static method Initialize().
            await new PatientClient().Initialize();
        }

        public async Task Initialize()
        {
            if (await _clientConn.ConnectToServer())
            {
                // Catch any CommunicationExceptions that could be thrown here
                try { await Run(); }
                catch (CommunicationException ex)
                {
                    await Console.Out.WriteLineAsync($"CommunicationException: {ex.Message}\n{ex.StackTrace}");
                }
            }
            else
            {
                await Console.Out.WriteLineAsync("Could not connect to server");
                Console.ReadLine();
            }
        }

        private async Task Run()
        {
            Thread t = new Thread(ReceiveConsoleInput);
            t.Start();

            await _commandHandler.Listen();
        }

        /// <summary>
        /// Receives string commands from the console input and applies the command if valid.
        /// </summary>
        private void ReceiveConsoleInput()
        {
            while (true)
            {
                JsonObject dataObject;

                string input = Console.ReadLine();

                switch (input)
                {
                    case "login":
                        // Receive username and password
                        Console.Write("Username: ");
                        string username = Console.ReadLine();
                        Console.Write("Password: ");
                        string password = Console.ReadLine();
                        dataObject = new JsonObject()
                        {
                            { "username", username },
                            { "password", password }
                        };

                        _commandHandler.ExecuteCommandToSend("login", dataObject);
                        break;
                    case "stats/send":
                        // Receive speed, distance and heart rate
                        Console.Write("Speed: ");
                        int speed = Convert.ToInt32(Console.ReadLine());

                        Console.Write("Distance: ");
                        int distance = Convert.ToInt32(Console.ReadLine());

                        Console.Write("Heart rate: ");
                        int heartRate = Convert.ToInt32(Console.ReadLine());

                        dataObject = new JsonObject()
                        {
                            { "speed", speed },
                            { "distance", distance },
                            { "heartrate", heartRate }
                        };

                        _commandHandler.ExecuteCommandToSend("stats/send", dataObject);
                        break;
                    default:
                        Console.WriteLine("Unknown command: " + input);
                        break;
                }
            }
        }
    }
}
