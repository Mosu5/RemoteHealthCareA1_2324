using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using PatientApp.BikeConnection;
using PatientApp.BikeConnection.Receiver;
using PatientApp.Commands;
using Utilities.Communication;

namespace PatientApp
{
    internal class Program
    {
        private static ISessionCommand _command = new Login();
        private static Client client = new Client();

        static async Task Main(string[] args)
        {
            try
            {
                //if (await DataTransfer.ConnectToServer("127.0.0.1", 8888))
                //    await Run();
            }
            catch (CommunicationException ex)
            {
                await Console.Out.WriteLineAsync($"CommunicationException: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static async Task Run()
        {
            client.OnReceiveData += OnReceiveData;

            // Run a thread that listens for console input
            Thread t = new Thread(ReceiveConsoleInput);
            t.Start();

            // TODO: add message handler class for handling messages
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
                        _command = new SessionStart(client.OnReceiveData, OnReceiveData);
                        break;
                    case "stats/send":
                        _command = new SendStats();
                        break;
                    default:
                        throw new CommunicationException("Unknown command received: " + commandField);
                }
                _command?.Execute(dataField);
            }
        }

        /// <summary>
        /// Receives string commands from the console input and applies the command if valid.
        /// </summary>
        private static void ReceiveConsoleInput()
        {
            while (true)
            {
                object payload;
                string input = Console.ReadLine();

                switch (input)
                {
                    case "login":
                        // Receive username and password
                        Console.Write("Username: ");
                        string username = Console.ReadLine();
                        Console.Write("Password: ");
                        string password = Console.ReadLine();
                        payload = new { username, password };

                        Login login = new Login();
                        // Login login = new Login(username, password);
                        login.Username = username;
                        login.Password = password;
                        
                        ApplyCommand(login, null);
                        break;
                    case "stats/send":
                        // Receive speed, distance and heart rate
                        Console.Write("Speed: ");
                        int speed = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Distance: ");
                        int distance = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Heart rate: ");
                        int heartrate = Convert.ToInt32(Console.ReadLine());
                        payload = new { speed, distance, heartrate };

                        ApplyCommand(new SendStats(), payload);
                        break;
                    default:
                        Console.WriteLine("Unknown command " + input);
                        break;
                }
            }
        }

        /// <summary>
        /// Accesses the _command variable in a thread-safe manner and executes the given command.
        /// </summary>
        private static void ApplyCommand(ISessionCommand command, object payload)
        {
            // Lock the _command variable, so no other thread can access it while the program is inside this lock block.
            lock (_command)
            {
                _command = command;
                // Please tell me there's a better way to cast an object to JsonObject
                //JsonObject jsonPayload = payload as JsonObject;
                //_command.Execute(jsonPayload);

                _command.Execute(JsonSerializer.Deserialize<JsonObject>(JsonSerializer.Serialize(payload)));
            }
        }

        private static void OnReceiveData(object sender, Statistic stat)
        {
            // new SendStat(...);
        }
    }
}