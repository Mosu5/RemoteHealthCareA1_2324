using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;
using Utilities.Communication;

namespace ServerApp
{
    internal class ServerConn
    {
        private static readonly Encoding _encoding = Encoding.ASCII;
        private static TcpListener _tcpListener;

        public static void StartListener(string ipAddress, int port)
        {
            _tcpListener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _tcpListener.Start();
        }

        public static TcpClient AcceptClient()
        {
            return _tcpListener.AcceptTcpClient();
        }

        /// <summary>
        ///     Sends a JSON message to the VR server asynchronously.
        /// </summary>
        /// <param name="payload">An object whose structure will be converted to JSON and sent to the VR server.</param>
        /// <exception cref="CommunicationException">When something goes wrong while sending data to the VR server.</exception>
        public static async Task SendJson(TcpClient client, JsonObject payload)
        {
            // Encode JSON string as a byte array
            byte[] payloadAsBytes = _encoding.GetBytes(payload.ToString());

            // Encode length of the payload as bytes
            byte[] lengthData = BitConverter.GetBytes((uint)payloadAsBytes.Length);

            // Concatenate payload to length data for the final message
            byte[] message = lengthData.Concat(payloadAsBytes).ToArray();

            await client.GetStream().WriteAsync(message, 0, message.Length);
        }

        /// <summary>
        ///     Receives a JSON message from the VR server asynchronously
        /// </summary>
        /// <returns>The message which the server sent, as a JsonObject.</returns>
        /// <exception cref="CommunicationException">When something goes wrong while receiving data from the VR server.</exception>
        public static async Task<JsonObject> ReceiveJson(TcpClient client)
        {
            NetworkStream clientStream = client.GetStream();
            byte[] lengthArray = new byte[4];

            // Read the first four bytes and put it in lengthArray
            await clientStream.ReadAsync(lengthArray, 0, lengthArray.Length);

            // Convert length to uint
            uint length = BitConverter.ToUInt32(lengthArray, 0);

            byte[] payloadBuffer = new byte[length];
            int totalBytesRead = 0;

            // Read until amount of bytes read exceeds the length of the length variable
            while (totalBytesRead < length)
            {
                // Read the bytes that have just been received
                int bytesRead = await clientStream.ReadAsync(payloadBuffer, totalBytesRead, payloadBuffer.Length - totalBytesRead);
                totalBytesRead += bytesRead;
            }

            // Deserialize message
            var messageAsString = _encoding.GetString(payloadBuffer, 0, totalBytesRead);
            JsonObject deserializedMessage = JsonSerializer.Deserialize<JsonObject>(messageAsString)?.AsObject();

            return deserializedMessage;
        }
    }
}
