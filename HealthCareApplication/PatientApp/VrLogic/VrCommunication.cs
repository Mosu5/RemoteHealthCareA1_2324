using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;
using Utilities.Communication;

namespace PatientApp.VrLogic
{
    internal class VrCommunication
    {
        private static readonly Encoding _encoding = Encoding.ASCII;

        private static TcpClient _tcpClient;
        private static NetworkStream _stream;

        private static bool _isConnected;

        /// <summary>
        ///     Attempts to asynchronously connect to the VR server and establishes a NetworkStream that listens
        ///     to incoming messages.
        /// </summary>
        /// <returns>Wether the connection attempt was successful.</returns>
        public static async Task<bool> ConnectToServer(string ipAddress, int port)
        {
            if (_isConnected) return false;

            _tcpClient = new TcpClient(ipAddress, port);
            if (_tcpClient.Connected)
            {
                _stream = _tcpClient.GetStream();
                _isConnected = true;
                return true;
            }

            await _tcpClient.ConnectAsync(ipAddress, port);

            if (_tcpClient.Connected) _stream = _tcpClient.GetStream();
            _isConnected = _tcpClient.Connected;

            return _isConnected;
        }

        /// <summary>
        ///     Closes the TcpClient and NetworkStream.
        /// </summary>
        public static void CloseConnection()
        {
            if (!_isConnected) return;
            _isConnected = false;
            _tcpClient?.Close();
            _stream?.Close();
        }

        /// <summary>
        ///     Sends a JSON message to the VR server asynchronously.
        /// </summary>
        /// <param name="payload">An object whose structure will be converted to JSON and sent to the VR server.</param>
        /// <exception cref="CommunicationException">When something goes wrong while sending data to the VR server.</exception>
        public static async Task SendAsJson(object payload)
        {
            if (!_isConnected)
                throw new CommunicationException(
                    "There is no active communication between the application and the VR server.");

            var payloadAsBytes = _encoding.GetBytes(JsonSerializer.Serialize(payload));
            var lengthData = BitConverter.GetBytes((uint)payloadAsBytes.Length);
            var message = lengthData.Concat(payloadAsBytes).ToArray();

            await _stream.WriteAsync(message, 0, message.Length);
        }

        /// <summary>
        ///     Receives a JSON message from the VR server asynchronously
        /// </summary>
        /// <returns>The message which the server sent, as a JsonObject.</returns>
        /// <exception cref="CommunicationException">When something goes wrong while receiving data from the VR server.</exception>
        public static async Task<JsonObject> ReceiveJsonObject()
        {
            if (!_isConnected)
                throw new CommunicationException(
                    "There is no active communication between the application and the VR server.");
            var lengthArray = new byte[4];

            await _stream.ReadAsync(lengthArray, 0, lengthArray.Length);
            var length = BitConverter.ToUInt32(lengthArray, 0);

            var payloadBuffer = new byte[length];
            var totalBytesRead = 0;

            while (totalBytesRead < length)
            {
                var bytesRead =
                    await _stream.ReadAsync(payloadBuffer, totalBytesRead, payloadBuffer.Length - totalBytesRead);
                totalBytesRead += bytesRead;
            }

            var messageAsString = _encoding.GetString(payloadBuffer, 0, totalBytesRead);
            var deserializedMessage = JsonSerializer.Deserialize<JsonObject>(messageAsString)?.AsObject();

            if (deserializedMessage != null) return deserializedMessage;

            throw new CommunicationException("Something went wrong while receiving a message from the VR server.");
        }
    }
}
