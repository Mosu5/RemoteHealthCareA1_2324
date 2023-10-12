using PatientApp.DeviceConnection;
using PatientApp.PatientLogic;
using PatientApp.PatientLogic.Commands;
using PatientApp.VrLogic;
using System;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Communication;
using Utilities.Logging;

namespace PatientApp
{
    public class PatientClient
    {
        static async Task Main(string[] args)
        {
            // Logger will log if LogType is present
            Logger.SetTypesToLogFor(
                LogType.GeneralInfo,
                //LogType.DeviceInfo,
                LogType.VrInfo,
                LogType.CommunicationExceptionInfo,
                LogType.Warning,
                LogType.Error,
                LogType.Debug
            );

            try
            {
                // Initialize BLE connection
                await DeviceManager.Initialize();

                // Initialize VR environment
                await Task.Run(VrProgram.Initialize);

                // Run commands for the healthcare server which you don't want to manually type
                Task.Run(AutoExecuteCommands);

                // Initialize console commands, but don't wait for completion
                Task.Run(ReceiveConsoleInput);

                // Listen for requests
                await RequestHandler.Listen();
            }
            catch (CommunicationException ex)
            {
                Logger.Log($"CommunicationException: {ex.Message}\n{ex.StackTrace}", LogType.CommunicationExceptionInfo);
            }
        }

        /// <summary>
        /// Any commands that you would want to send to the server but don't want to manually type, go here!
        /// </summary>
        private static async Task AutoExecuteCommands()
        {
            // To make sure the request listener had time to listen for responses
            await Task.Delay(1000);

            //await new Login("bob", "bob").Execute();
            //await new SessionStart().Execute();
        }

        /// <summary>
        /// Read console commands to be sent to the healthcare server.
        /// </summary>
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
                            Logger.Log($"User {username} has entered incorrect credentials.", LogType.Error);
                        break;
                    case "logout":
                        // Attempt logging out of the server
                        if (await new Logout().Execute())
                            Logger.Log($"The user has logged out of the server.", LogType.GeneralInfo);
                        else
                            Logger.Log($"The user could not be logged out.", LogType.Error);
                        break;
                    case "stats/summary":
                        var summaryCommand = new StatsSummary();
                        if (await summaryCommand.Execute())
                            Logger.Log($"Received summary response", LogType.GeneralInfo);
                        else
                            Logger.Log("A summary of this session could not be retrieved.", LogType.Error);
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
                        if (await new SessionStart().Execute())
                            Logger.Log($"A new session has started.", LogType.GeneralInfo);
                        else
                            Logger.Log("A new session could not be started.", LogType.Error);
                        break;
                    case "session/stop":
                        // Attempt stopping the session
                        if (await new SessionStop().Execute())
                            Logger.Log($"The current session has been stopped.", LogType.GeneralInfo);
                        else
                            Logger.Log("The current session could not be stopped.", LogType.Error);
                        break;
                    case "session/pause":
                        // Attempt pausing the session
                        if (await new SessionPause().Execute())
                            Logger.Log($"The current session has been paused.", LogType.GeneralInfo);
                        else
                            Logger.Log("The current session could not be paused.", LogType.Error);
                        break;
                    case "session/resume":
                        // Attempt resuming the session
                        if (await new SessionResume().Execute())
                            Logger.Log($"The current session has been resumed.", LogType.GeneralInfo);
                        else
                            Logger.Log("The current session could not be resumed.", LogType.Error);
                        break;
                    default:
                        Logger.Log($"Unknown command: {input}", LogType.Warning);
                        break;
                }
            }
        }
    }
}
