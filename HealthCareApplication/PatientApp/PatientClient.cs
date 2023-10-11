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
}
