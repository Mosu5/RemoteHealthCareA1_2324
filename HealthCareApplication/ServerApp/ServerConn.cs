using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ServerApp
{
    internal class ServerConn
    {
        private static readonly Encoding _encoding = Encoding.ASCII;

        private TcpClient _tcpClient;
        private NetworkStream _stream;

        public ServerConn(TcpClient tcpClient)
        { 
            this._tcpClient = tcpClient;
            this._stream = tcpClient.GetStream();
        }

        /// <summary>
        ///     Closes the TcpClient and NetworkStream.
        /// </summary>
        public void CloseConnection()
        {
            _tcpClient?.Close();
            _stream?.Close();
        }

        /// <summary>
        ///     Sends a JSON message to the VR server asynchronously.
        /// </summary>
        /// <param name="payload">An object whose structure will be converted to JSON and sent to the VR server.</param>
        /// <exception cref="CommunicationException">When something goes wrong while sending data to the VR server.</exception>
        public async Task SendJson(object payload)
        {

            // Turn payload object into a JSON string
            string jsonString = JsonSerializer.Serialize(payload);

            // Encode JSON string as a byte array
            byte[] payloadAsBytes = _encoding.GetBytes(jsonString);

            // Encode length of the payload as bytes
            byte[] lengthData = BitConverter.GetBytes((uint)payloadAsBytes.Length);

            // Concatenate payload to length data for the final message
            byte[] message = lengthData.Concat(payloadAsBytes).ToArray();

            await _stream.WriteAsync(message, 0, message.Length);
        }

        /// <summary>
        ///     Receives a JSON message from the VR server asynchronously
        /// </summary>
        /// <returns>The message which the server sent, as a JsonObject.</returns>
        /// <exception cref="CommunicationException">When something goes wrong while receiving data from the VR server.</exception>
        public async Task<JsonObject> ReceiveJson()
        {

            byte[] lengthArray = new byte[4];

            // Read the first four bytes and put it in lengthArray
            await _stream.ReadAsync(lengthArray, 0, lengthArray.Length);

            // Convert length to uint
            uint length = BitConverter.ToUInt32(lengthArray, 0);

            byte[] payloadBuffer = new byte[length];
            int totalBytesRead = 0;

            // Read until amount of bytes read exceeds the length of the length variable
            while (totalBytesRead < length)
            {
                // Read the bytes that have just been received
                int bytesRead = await _stream.ReadAsync(payloadBuffer, totalBytesRead, payloadBuffer.Length - totalBytesRead);
                totalBytesRead += bytesRead;
            }

            // Deserialize message
            var messageAsString = _encoding.GetString(payloadBuffer, 0, totalBytesRead);
            JsonObject deserializedMessage = JsonSerializer.Deserialize<JsonObject>(messageAsString)?.AsObject();

            return deserializedMessage;
        }
    }
}
