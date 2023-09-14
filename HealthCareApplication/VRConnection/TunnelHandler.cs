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
        public static string TunnelId;
        
        /// <summary>
        /// Create tunnel for VR communication
        /// </summary>
        /// <param name="networkStream">data stream where messages are sent through</param>
        public static void CreateTunnel(NetworkStream networkStream)
        {
            var sessionId = GetSessionId(networkStream);

            var tunnelCommand = Formatting.TunnelAdd(sessionId); // create command

            SendMessage(networkStream, tunnelCommand); // request tunnel connection

            var tunnelConfirmation = ReadJsonObject(networkStream); // get confirmation from server
            
            TunnelId = tunnelConfirmation?["data"]?["id"]?.ToString() ?? string.Empty; // get tunnelId or set to empty if no id is found
            Console.WriteLine("Tunnel id: {0}", TunnelId);
        }

        public static string GetSessionId(NetworkStream networkStream)
        {
            // get the sessionlist
            var sessionListCommand = Formatting.GetSessionList();
            SendMessage(networkStream, sessionListCommand);

            // find pc running the sim/vr 
            var sessionListData = ReadJsonObject(networkStream);
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


        public static JsonObject? ReadJsonObject(NetworkStream networkStream)
        {
            byte[] lengthArray = new byte[4];

            networkStream.Read(lengthArray, 0, 4);
            uint lenght = BitConverter.ToUInt32(lengthArray, 0);

            Console.WriteLine(lenght);

            byte[] buffer = new byte[lenght];
            int totalRead = 0;

            // Read bytes untill no more

            while (totalRead < lenght)
            {
                int read = networkStream.Read(buffer, totalRead, buffer.Length - totalRead);
                totalRead += read;
                Console.WriteLine("ReadMessage: " + read);
            }

            var data = Encoding.ASCII.GetString(buffer, 0, totalRead);


            var jsonData = JsonSerializer.Deserialize<JsonObject>(data)?.AsObject();

            return jsonData;
        }


        public static string ReadString(NetworkStream networkStream)
        {
            byte[] lenghtArray = new byte[4];

            networkStream.Read(lenghtArray, 0, 4);
            uint lenght = BitConverter.ToUInt32(lenghtArray, 0);

            Console.WriteLine(lenght);

            byte[] buffer = new byte[lenght];
            int totalRead = 0;

            // Read bytes untill no more

            while (totalRead < lenght)
            {
                int read = networkStream.Read(buffer, totalRead, buffer.Length - totalRead);
                totalRead += read;
                Console.WriteLine("ReadMessage: " + read);
            }

            return Encoding.ASCII.GetString(buffer, 0, totalRead);
        }


        /// <summary>
        /// Serializes and sends a (json) object
        /// </summary>
        /// <param name="networkStream">stream where the data is sent through</param>
        /// <param name="message">message that you want to send</param>
        public static void SendMessage(NetworkStream networkStream, object message)
        {
            var jsonString = JsonSerializer.Serialize(message);

            byte[] bytes = Encoding.ASCII.GetBytes(jsonString);
            byte[] length = BitConverter.GetBytes((uint)jsonString.Length);
            bytes = length.Concat(bytes).ToArray();

            networkStream.Write(bytes, 0, bytes.Length);
        }

        public static void SendMessage(NetworkStream networkStream, string message)
        {
            //make sure the other end decodes with the same format!
            byte[] bytes = Encoding.ASCII.GetBytes(message);
            byte[] length = BitConverter.GetBytes((uint)message.Length); // Getting the length of the payload
            Console.WriteLine("Lenghts = {0}", message.Length);

            // Takes the first four bytes that indicate lenght and adds it to the total load
            bytes = length.Concat(bytes).ToArray();

            networkStream.Write(bytes, 0, bytes.Length);
        }
    }
}