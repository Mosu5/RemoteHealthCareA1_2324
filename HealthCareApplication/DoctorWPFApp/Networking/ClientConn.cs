using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace DoctorWPFApp.Networking
{
    public class ClientConn
    {
        private static readonly Encoding _encoding = Encoding.ASCII;

        private static TcpClient? _tcpClient;
        private static NetworkStream? _stream;

        /// <summary>
        ///     Attempts to asynchronously connect to the server and establishes a NetworkStream that listens
        ///     to incoming messages.
        /// </summary>
        /// <returns>Wether the connection attempt was successful.</returns>
        public static async Task<bool> ConnectToServer(string ipAddress, int port)
        {
            // Return if already connected
            if (_tcpClient != null && _tcpClient.Connected) return false;

            // Create new TcpClient and await the connection process to the server
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(ipAddress, port);

            if (_tcpClient.Connected) _stream = _tcpClient.GetStream();

            return _tcpClient.Connected;
        }

        /// <summary>
        ///     Closes the TcpClient and NetworkStream.
        /// </summary>
        public static void CloseConnection()
        {
            // Return if not connected
            if (_tcpClient == null || !_tcpClient.Connected) return;
            _tcpClient?.Close();
            _stream?.Close();
        }

        /// <summary>
        ///     Sends a JSON message to the server asynchronously.
        /// </summary>
        /// <param name="payload">An object whose structure will be converted to JSON and sent to the server.</param>
        /// <exception cref="CommunicationException">When something goes wrong while sending data to the server.</exception>
        public static async Task SendJson(JsonObject payload)
        {
            if (_tcpClient == null || !_tcpClient.Connected)
                throw new CommunicationException(
                    "There is no active communication between the client application and the server.");

            // Encode JSON string as a byte array
            byte[] payloadAsBytes = _encoding.GetBytes(payload.ToString());

            // Encode length of the payload as bytes
            byte[] lengthData = BitConverter.GetBytes((uint)payloadAsBytes.Length);

            // Concatenate payload to length data for the final message
            byte[] message = lengthData.Concat(payloadAsBytes).ToArray();

            await _stream.WriteAsync(message, 0, message.Length);
        }

        /// <summary>
        ///     Receives a JSON message from the server asynchronously
        /// </summary>
        /// <returns>The message which the server sent, as a JsonObject.</returns>
        /// <exception cref="CommunicationException">When something goes wrong while receiving data from the server.</exception>
        public static async Task<JsonObject> ReceiveJson()
        {
            try
            {
                if (_tcpClient == null || !_tcpClient.Connected)
                    throw new CommunicationException(
                        "There is no active communication between the client application and the server.");

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
                JsonObject? deserializedMessage = JsonSerializer.Deserialize<JsonObject>(messageAsString)?.AsObject();

                if (deserializedMessage == null)
                    throw new CommunicationException("Something went wrong while receiving a message from the server.");

                return deserializedMessage;
            }
            catch (ObjectDisposedException)
            {
                return null;
            }
            catch (IOException)
            {
                return null;
            }
        }
    }
}
