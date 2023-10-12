using PatientApp.DeviceConnection;
using PatientApp.PatientLogic.Commands;
using PatientApp.PatientLogic.Helpers;
using PatientApp.VrLogic;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;
using Utilities.Logging;

namespace PatientApp.PatientLogic
{
    internal class RequestHandler
    {
        // List for holding all the requests that have been sent to the server,
        // but which the server has not yet responded to.
        private static readonly List<Request> _pendingRequests = new List<Request>();

        public static readonly ClientConn ClientConn = new ClientConn("127.0.0.1", 8888);

        /// <summary>
        /// Listen for messages from the server and checks if each message is a response or not.
        /// </summary>
        public static async Task Listen()
        {
            // Handle healthcare server connectivity
            if (!await ClientConn.ConnectToServer())
                throw new CommunicationException("Could not connect to the server.");

            while (await ClientConn.ReceiveJson() is var message) // listening for message
            {
                Logger.Log($"Patient Received: {message}", LogType.Debug);

                // Get command and data field from message
                (string command, JsonObject dataObject) = GetCommandAndData(message);

                // Thread safe access to _pendingRequests, because another thread may use this variable too.
                // TODO see if this is still necessary in the final product with UI and stuff.
                lock (_pendingRequests)
                {
                    // Check if this message was a response to a request we sent earlier
                    Request possibleRequest = GetRequestWithCommand(command);
                    if (possibleRequest != null)
                    {
                        // Handle the message as a response to something sent earlier
                        possibleRequest.SetResponse(dataObject);
                        if (!_pendingRequests.Remove(possibleRequest))
                            throw new CommunicationException("Could not remove request from list.");
                        continue;
                    }
                }

                // TODO here goes code for anything that was not a response, e.g. the chat listener
                Logger.Log($"Was not response, but was {command}", LogType.GeneralInfo);

                switch (command)
                {
                    case "chats/send":
                        string patientUsername = PatientFormat.GetKey(dataObject, "clientUsername").ToString();
                        string chatMessage = PatientFormat.GetKey(dataObject, "message").ToString();
                        Logger.Log($"<{patientUsername}> :\t{chatMessage}", LogType.GeneralInfo);
                        break;
                    // If any session/... has been sent and the code reaches this place,
                    // then assume that the doctor did not send the command, but that the patient did.
                    case "session/start":
                        Logger.Log("Doctor has started the session", LogType.GeneralInfo);
                        break;
                    case "session/stop":
                        Logger.Log("Doctor has stopped the session", LogType.GeneralInfo);
                        break;
                    case "session/pause":
                        Logger.Log("Doctor has paused the session", LogType.GeneralInfo);
                        break;
                    case "session/resume":
                        Logger.Log("Doctor has resumed the session", LogType.GeneralInfo);
                        break;
                    default:
                        Logger.Log($"Cannot process command '{command}'.", LogType.Warning);
                        break;
                }
            }
        }

        /// <summary>
        /// Retrieves the command field (string) and the data field (JsonObject) from
        /// a message.
        /// </summary>
        /// <exception cref="CommunicationException">If the JSON message did not have the required fields</exception>
        public static (string, JsonObject) GetCommandAndData(JsonObject message)
        {
            // Check if the message has a command field
            if (!message.ContainsKey("command"))
                throw new CommunicationException("The message did not contain the JSON key 'command'");

            // Check if the message has a data field
            if (!message.ContainsKey("data"))
                throw new CommunicationException("The message did not contain the JSON key 'data'");

            // Get command and data field
            string command = message["command"].ToString();
            JsonObject dataObject = message["data"].AsObject();

            // Return a TjOePEl
            return (command, dataObject);
        }

        /// <summary>
        /// Sends the request to the server and waits for the response
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The data field of the response sent by the server</returns>
        public static async Task<JsonObject> GetResponse(Request request)
        {
            await ClientConn.SendJson(request.Message);

            lock (_pendingRequests)
                _pendingRequests.Add(request);

            // Wait for response of request
            return await request.AwaitResponse();
        }

        /// <summary>
        /// Handles the received trainer data and sends it to the server
        /// </summary>
        public static void OnReceiveData(object _, Statistic stat)
        {
            new StatsSend(stat, ClientConn).Execute().Wait();

            VrProgram.UpdateBikeSpeed(stat.GetSpeed()).Wait();
        }

        /// <summary>
        /// Checks if there is a request with a given command inside the _pendingRequests list.
        /// </summary>
        /// <returns>The request with the specified command value, or else null.</returns>
        private static Request GetRequestWithCommand(string command)
        {
            foreach (var request in _pendingRequests)
            {
                if (command.Equals(request.Command))
                    return request;
            }
            return null;
        }
    }
}
