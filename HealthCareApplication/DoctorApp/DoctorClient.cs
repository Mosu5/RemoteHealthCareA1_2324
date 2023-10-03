using DoctorApp.Commands;
using System.Text.Json.Nodes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Communication;
using Utilities.Logging;

namespace DoctorApp
{
    internal class DoctorClient
    {
        private static readonly string _serverIpAddress = "127.0.0.1";
        private static readonly int _port = 8888;
        public static readonly ClientConn _clientConn = new ClientConn(_serverIpAddress, _port);

        public static async Task Main(string[] args)
        {
            // In the final build, probably uncomment this line. This makes sure no testing logs are written to the console
            //Logger.SetTypesToLogFor(LogType.Warning, LogType.Error, LogType.CommunicationExceptionInfo);

            if (!await _clientConn.ConnectToServer())
            {
                Logger.Log("Could not connect to server", LogType.Error);
                return;
            }

            try
            {
                await Task.Run(ReceiveConsoleInput);

                // Below is an example of how communication might work.

                //// Attempt logging in to the server
                //var username = "Joel Beukers";
                //if (await new Login(username, "POMPEEEEE").Execute(_clientConn))
                //    Logger.Log($"User {username} has logged on to the server.", LogType.GeneralInfo);
                //else
                //{
                //    Logger.Log($"User {username} has entered incorrect credentials.", LogType.Error);
                //    return;
                //}

                //// Attempt starting the session
                //if (await new SessionStart().Execute(_clientConn))
                //    Logger.Log($"A new session has started.", LogType.GeneralInfo);
                //else
                //{
                //    Logger.Log("A new session could not be started.", LogType.Error);
                //    return;
                //}

                //Thread.Sleep(5000);

                //// Attempt retrieving a summary
                //var summaryCommand = new StatsSummary();
                //if (await summaryCommand.Execute(_clientConn))
                //    Logger.Log($"Summary:\n{summaryCommand.Summary}", LogType.GeneralInfo);
                //else
                //{
                //    Logger.Log("A summary of this session could not be retrieved.", LogType.Error);
                //    return;
                //}

                //// Attempt stopping the session
                //if (await new SessionStop().Execute(_clientConn))
                //    Logger.Log($"The current session has been stopped.", LogType.GeneralInfo);
                //else
                //{
                //    Logger.Log("The current session could not be stopped.", LogType.Error);
                //    return;
                //}

                //// Send a chat
                //await new ChatsSend("GA NOU EENS NAAR DIE GYM JONGE").Execute(_clientConn);
            }
            catch (CommunicationException ex)
            {
                Logger.Log($"CommunicationException: {ex.Message}\n{ex.StackTrace}", LogType.CommunicationExceptionInfo);
            }
        }

        /// <summary>
        /// Receives console input. FOR TESTING PURPOSES, so does not necessarily need to be reorganised.
        /// </summary>
        private static async Task ReceiveConsoleInput()
        {
            Logger.Log("Enter commands in the console to execute them.", LogType.GeneralInfo);

            while (true)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    case "login":
                        // Receive username and password
                        Console.WriteLine("Username:");
                        string username = Console.ReadLine();
                        Console.WriteLine("Password:");
                        string password = Console.ReadLine();

                        // Attempt logging in to the server
                        if (await new Login(username, password).Execute(_clientConn))
                            Logger.Log($"User {username} has logged on to the server.", LogType.GeneralInfo);
                        else
                        {
                            Logger.Log($"User {username} has entered incorrect credentials.", LogType.Error);
                            return;
                        }
                        break;
                    case "logout":
                        // Attempt logging out of the server
                        if (await new Logout().Execute(_clientConn))
                            Logger.Log($"The user has logged out of the server.", LogType.GeneralInfo);
                        else
                        {
                            Logger.Log($"The user could not be logged out.", LogType.Error);
                            return;
                        }
                        break;
                    case "stats/summary":
                        // Attempt retrieving a summary
                        var summaryCommand = new StatsSummary();
                        if (await summaryCommand.Execute(_clientConn))
                            Logger.Log($"Summary:\n{summaryCommand.Summary}", LogType.GeneralInfo);
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
                        await new ChatsSend(chatMessage).Execute(_clientConn);
                        break;
                    case "session/start":
                        // Attempt starting the session
                        if (await new SessionStart().Execute(_clientConn))
                            Logger.Log($"A new session has started.", LogType.GeneralInfo);
                        else
                        {
                            Logger.Log("A new session could not be started.", LogType.Error);
                            return;
                        }
                        break;
                    case "session/stop":
                        // Attempt stopping the session
                        if (await new SessionStop().Execute(_clientConn))
                            Logger.Log($"The current session has been stopped.", LogType.GeneralInfo);
                        else
                        {
                            Logger.Log("The current session could not be stopped.", LogType.Error);
                            return;
                        }
                        break;
                    case "session/pause":
                        // Attempt stopping the session
                        if (await new SessionPause().Execute(_clientConn))
                            Logger.Log($"The current session has been paused.", LogType.GeneralInfo);
                        else
                        {
                            Logger.Log("The current session could not be paused.", LogType.Error);
                            return;
                        }
                        break;
                    case "session/resume":
                        // Attempt stopping the session
                        if (await new SessionResume().Execute(_clientConn))
                            Logger.Log($"The current session has been resumed.", LogType.GeneralInfo);
                        else
                        {
                            Logger.Log("The current session could not be resumed.", LogType.Error);
                            return;
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown command: " + input);
                        break;
                }
            }
        }
    }
}