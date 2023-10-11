using PatientApp.DeviceConnection;
using PatientApp.PatientLogic;
using PatientApp.PatientLogic.Commands;
using System;
using System.Threading.Tasks;
using Utilities.Communication;
using Utilities.Logging;

namespace PatientApp
{
    public class PatientClient
    {
        static async Task Main(string[] args)
        {
            try
            {
                Task.Run(ReceiveConsoleInput);

                await RequestHandler.Listen();
            }
            catch (CommunicationException ex)
            {
                Logger.Log($"CommunicationException: {ex.Message}\n{ex.StackTrace}", LogType.CommunicationExceptionInfo);
            }
        }

        private static async Task ReceiveConsoleInput()
        {
            Logger.Log("Enter commands in the console to execute them.", LogType.GeneralInfo);

            while (true)
            {
                // Read input
                string input = Console.ReadLine();

                switch (input)
                {
                    case "login":
                        // Receive username and password
                        Logger.Log("Username:", LogType.Debug);
                        string username = Console.ReadLine();
                        Logger.Log("Password:", LogType.Debug);
                        string password = Console.ReadLine();

                        // Attempt logging in to the server
                        if (await new Login(username, password).Execute())
                            Logger.Log($"User {username} has logged on to the server.", LogType.GeneralInfo);
                        else
                        {
                            Logger.Log($"User {username} has entered incorrect credentials.", LogType.Error);
                            return;
                        }
                        break;
                    case "logout":
                        // Attempt logging out of the server
                        if (await new Logout().Execute())
                            Logger.Log($"The user has logged out of the server.", LogType.GeneralInfo);
                        else
                        {
                            Logger.Log($"The user could not be logged out.", LogType.Error);
                            return;
                        }
                        break;
                    case "stats/summary":
                        var summaryCommand = new StatsSummary();
                        if (await summaryCommand.Execute())
                            Logger.Log($"Received summary response", LogType.GeneralInfo);
                        else
                        {
                            Logger.Log("A summary of this session could not be retrieved.", LogType.Error);
                            return;
                        }
                        break;
                    case "chats/send":
                        // Receive chat message
                        Console.WriteLine("Chat message:");
                        string chatMessage = Console.ReadLine();

                        // Send a chat
                        await new ChatsSend(chatMessage, RequestHandler.ClientConn).Execute();
                        break;
                    case "session/start":
                        // Attempt starting the session
                        if (await new SessionStart(RequestHandler.OnReceiveData).Execute())
                            Logger.Log($"A new session has started.", LogType.GeneralInfo);
                        else
                            Logger.Log("A new session could not be started.", LogType.Error);
                        break;
                    case "session/stop":
                        // Attempt stopping the session
                        if (await new SessionStop(RequestHandler.OnReceiveData).Execute())
                            Logger.Log($"The current session has been stopped.", LogType.GeneralInfo);
                        else
                            Logger.Log("The current session could not be stopped.", LogType.Error);
                        break;
                    case "session/pause":
                        // Attempt pausing the session
                        if (await new SessionPause(RequestHandler.OnReceiveData).Execute())
                            Logger.Log($"The current session has been paused.", LogType.GeneralInfo);
                        else
                            Logger.Log("The current session could not be paused.", LogType.Error);
                        break;
                    case "session/resume":
                        // Attempt resuming the session
                        if (await new SessionResume(RequestHandler.OnReceiveData).Execute())
                            Logger.Log($"The current session has been resumed.", LogType.GeneralInfo);
                        else
                            Logger.Log("The current session could not be resumed.", LogType.Error);
                        break;
                    default:
                        Logger.Log($"Unknown command: {input}", LogType.Error);
                        break;
                }
            }
        }
    }
    //public class PatientClient
    //{
    //    // Helper for sending and receiving network traffic
    //    private readonly ClientConn _clientConn;

    //    // Manages commands
    //    private readonly CommandHandler _commandHandler;

    //    // Connection to the trainer and hrm
    //    private readonly DeviceManager _deviceManager;

    //    public PatientClient()
    //    {
    //        _clientConn = new ClientConn("127.0.0.1", 8888);
    //        _deviceManager = new DeviceManager();
    //        _commandHandler = new CommandHandler(_clientConn);
    //    }

    //    static async Task Main(string[] args)
    //    {
    //        // Create a new instance of this class to access the non-static method Initialize().
    //        await new PatientClient().Initialize();
    //    }

    //    public async Task Initialize()
    //    {
    //        if (await _clientConn.ConnectToServer())
    //        {
    //            // Catch any CommunicationExceptions that could be thrown here
    //            try { await Run(); }
    //            catch (CommunicationException ex)
    //            {
    //                await Console.Out.WriteLineAsync($"CommunicationException: {ex.Message}\n{ex.StackTrace}");
    //            }
    //        }
    //        else
    //        {
    //            await Console.Out.WriteLineAsync("Could not connect to server");
    //            Console.ReadLine();
    //        }
    //    }

    //    private async Task Run()
    //    {
    //        Thread t = new Thread(ReceiveConsoleInput);
    //        t.Start();

    //        await _commandHandler.Listen();
    //    }

    //    /// <summary>
    //    /// Receives string commands from the console input and applies the command if valid.
    //    /// </summary>
    //    private void ReceiveConsoleInput()
    //    {
    //        while (true)
    //        {
    //            JsonObject dataObject;

    //            string input = Console.ReadLine();

    //            switch (input)
    //            {
    //                case "login":
    //                    // Receive username and password
    //                    Console.Write("Username: ");
    //                    string username = Console.ReadLine();
    //                    Console.Write("Password: ");
    //                    string password = Console.ReadLine();
    //                    dataObject = new JsonObject()
    //                    {
    //                        { "username", username },
    //                        { "password", password }
    //                    };

    //                    _commandHandler.ExecuteCommandToSend("login", dataObject);
    //                    break;
    //                case "stats/send":
    //                    // Receive speed, distance and heart rate
    //                    Console.Write("Speed: ");
    //                    int speed = Convert.ToInt32(Console.ReadLine());

    //                    Console.Write("Distance: ");
    //                    int distance = Convert.ToInt32(Console.ReadLine());

    //                    Console.Write("Heart rate: ");
    //                    int heartRate = Convert.ToInt32(Console.ReadLine());

    //                    dataObject = new JsonObject()
    //                    {
    //                        { "speed", speed },
    //                        { "distance", distance },
    //                        { "heartrate", heartRate }
    //                    };

    //                    _commandHandler.ExecuteCommandToSend("stats/send", dataObject);
    //                    break;
    //                default:
    //                    Console.WriteLine("Unknown command: " + input);
    //                    break;
    //            }
    //        }
    //    }
    //}
}
