using PatientApp.DeviceConnection;
using PatientApp.Commands;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Communication;

namespace PatientApp
{
    public class PatientClient
    {
        private readonly DeviceManager _deviceManager = new DeviceManager();
        private readonly ClientConn _clientConn = new ClientConn("127.0.0.1", 8888);

        private readonly CommandHandler _commandHandler;

        private ISessionCommand _command = new Login();

        public PatientClient()
        {
            _commandHandler = new CommandHandler(_clientConn, _deviceManager.OnReceiveData);
        }

        static async Task Main(string[] args)
        {
            try
            {
                PatientClient patientClient = new PatientClient();

                if (await patientClient._clientConn.ConnectToServer())
                {
                    await patientClient.Run();
                }
                else
                {
                    await Console.Out.WriteLineAsync("Could not connect to server");
                    Console.ReadLine();
                }
            }
            catch (CommunicationException ex)
            {
                await Console.Out.WriteLineAsync($"CommunicationException: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public async Task Run()
        {
            Thread t = new Thread(ReceiveConsoleInput);
            t.Start();

            // TODO: test message handler

            while (await _clientConn.ReceiveJson() is var message) // listening for message
            {
                await Console.Out.WriteLineAsync($"Received: {message}");
                _commandHandler.Handle(message);
            }
        }


        /// <summary>
        /// Receives string commands from the console input and applies the command if valid.
        /// </summary>
        private void ReceiveConsoleInput()
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

                        //ApplyCommand(login, payload);

                        JsonObject loginData = new JsonObject {
                          { "command", "login" },
                          { "data", new JsonObject {
                              {"username", username},
                              {"password", password}
                          }
                        }
                        };
                        // TO DO: Implement the following wrapper structure to make JSON formatting easier:
                        // CommandWrapper.Login(username, pass);

                        _clientConn.SendJson(loginData);
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
        /// Receives string commands from the console input and applies the command if valid.
        /// </summary>
        private void ApplyCommand(ISessionCommand command, object payload)
        {
            // Lock the _command variable, so no other thread can access it while the program is inside this lock block.
            lock (_command)
            {
                _command = command;
                // Please tell me there's a better way to cast an object to JsonObject
                //JsonObject jsonPayload = payload as JsonObject;
                //_command.Execute(jsonPayload);

                _command.Execute(JsonSerializer.Deserialize<JsonObject>(JsonSerializer.Serialize(payload)), _clientConn);
            }
        }

        public void OnReceiveData(object sender, Statistic stat)
        {

        }
    }
}
