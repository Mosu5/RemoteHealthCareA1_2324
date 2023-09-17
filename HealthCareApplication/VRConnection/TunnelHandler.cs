using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VRConnection
{
    /// <summary>
    /// Handles all communication between application and VR server
    /// </summary>
    public class TunnelHandler
    {
        public string TunnelId;
        private NetworkStream _networkStream;

        public TunnelHandler(NetworkStream networkStream)
        {
            _networkStream = networkStream;
        }

        /// <summary>
        /// Create tunnel for VR communication
        /// </summary>
        public void CreateTunnel()
        {
            var sessionId = GetSessionId();

            var tunnelCommand = Formatting.TunnelAdd(sessionId); // create command

            SendMessage(tunnelCommand); // request tunnel connection

            var tunnelConfirmation = ReadJsonObject(); // get confirmation from server
            
            TunnelId = tunnelConfirmation?["data"]?["id"]?.ToString() ?? string.Empty; // get tunnelId or set to empty if no id is found
        }
        
        /// <summary>
        /// Filter and return sessionId from VR session list
        /// </summary>
        /// <returns> sessionId as string </returns>
        public string GetSessionId()
        {
            // get the sessionlist
            var sessionListCommand = Formatting.SessionListGet();
            SendMessage(sessionListCommand);

            // find pc running the sim/vr 
            var sessionListData = ReadJsonObject();
            var sessions = sessionListData["data"]?.AsArray();
            var sessionId = "";

            foreach (var s in sessions)
            {
                var hostName = s?["clientinfo"]?["host"]?.ToString();
                // Console.WriteLine(hostName);
                if (hostName == null)
                {
                    continue;
                }

                if (!hostName.Equals(Environment.MachineName, StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                sessionId = s["id"].ToString();
                Console.WriteLine(sessionId);
            }

            return sessionId;
        }

        /// <summary>
        /// Read message from VR server as object
        /// </summary>
        /// <returns> message as JsonObject </returns>
        public JsonObject? ReadJsonObject()
        {
            byte[] lengthArray = new byte[4];

            _networkStream.Read(lengthArray, 0, 4);
            uint length = BitConverter.ToUInt32(lengthArray, 0);

            Console.WriteLine(length);

            byte[] buffer = new byte[length];
            int totalRead = 0;

            // Read bytes untill no more

            while (totalRead < length)
            {
                int read = _networkStream.Read(buffer, totalRead, buffer.Length - totalRead);
                totalRead += read;
                Console.WriteLine("ReadMessage: " + read);
            }

            var data = Encoding.ASCII.GetString(buffer, 0, totalRead);


            var jsonData = JsonSerializer.Deserialize<JsonObject>(data)?.AsObject();

            return jsonData;
        }

        /// <summary>
        /// Read message from VR server as text
        /// </summary>
        /// <returns> message as string </returns>
        public string ReadString()
        {
            byte[] lenghtArray = new byte[4];

            _networkStream.Read(lenghtArray, 0, 4);
            uint lenght = BitConverter.ToUInt32(lenghtArray, 0);

            Console.WriteLine(lenght);

            byte[] buffer = new byte[lenght];
            int totalRead = 0;

            // Read bytes untill no more

            while (totalRead < lenght)
            {
                int read = _networkStream.Read(buffer, totalRead, buffer.Length - totalRead);
                totalRead += read;
                Console.WriteLine("ReadMessage: " + read);
            }

            return Encoding.ASCII.GetString(buffer, 0, totalRead);
        }


        /// <summary>
        /// Serialize and send a message
        /// </summary>
        /// <param name="message"> message to be send </param>
        public void SendMessage(object message)
        {
            var jsonString = JsonSerializer.Serialize(message);

            byte[] bytes = Encoding.ASCII.GetBytes(jsonString);
            byte[] length = BitConverter.GetBytes((uint)jsonString.Length);
            bytes = length.Concat(bytes).ToArray();

            _networkStream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Serialize and send a message
        /// </summary>
        /// <param name="message"> message to be send </param>
        public void SendMessage(string message)
        {
            //make sure the other end decodes with the same format!
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            byte[] length = BitConverter.GetBytes((uint)message.Length); // Getting the length of the payload
            Console.WriteLine("Lenghts = {0}", message.Length);

            // Takes the first four bytes that indicate lenght and adds it to the total load
            bytes = length.Concat(bytes).ToArray();

            _networkStream.Write(bytes, 0, bytes.Length);
        }
    }
}