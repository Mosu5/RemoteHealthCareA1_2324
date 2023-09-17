using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Threading.Tasks;

namespace VRConnection
{
    internal class VrSession
    {
        private readonly string _ipAddress;
        private readonly int _port;
        private readonly Encoding _encoding = Encoding.ASCII;
        private TcpClient? _tcpClient;
        private NetworkStream? _stream;

        private bool _isSessionActive = false;

        public VrSession(string serverIpAddress, int serverPort)
        {
            _ipAddress = serverIpAddress;
            _port = serverPort;
        }

        public async Task<bool> Initialize()
        {
            if (!await ConnectToServer())
            {
                await Console.Out.WriteLineAsync($"Could not connect to address {_ipAddress} on port {_port}. Maybe there is an already active session?");
                return false;
            }

            await SendAsJson(Formatting.SessionListGet());

            JsonObject? sessionListResponse = await ReceiveAsJsonObject();
            string? sessionId = Formatting.ValidateAndGetSessionId(sessionListResponse);
            if (sessionId == null) return false;

            await SendAsJson(Formatting.TunnelAdd(sessionId));

            JsonObject? tunnelCreateResponse = await ReceiveAsJsonObject();
            string? tunnelId = Formatting.ValidateAndGetTunnelId(tunnelCreateResponse);
            if (tunnelId == null) return false;

            _isSessionActive = true;
            return true;
        }

        public void Close()
        {
            _tcpClient?.Close();
            _stream?.Close();
        }

        private async Task<bool> ConnectToServer()
        {
            if (_isSessionActive) return false;

            _tcpClient = new TcpClient(_ipAddress, _port);
            await _tcpClient.ConnectAsync(_ipAddress, _port);

            if (_tcpClient.Connected) _stream = _tcpClient.GetStream();
            return _tcpClient.Connected;
        }

        private async Task SendAsJson(object payload)
        {
            byte[] payloadAsBytes = _encoding.GetBytes(JsonSerializer.Serialize(payload));
            byte[] lengthData = BitConverter.GetBytes((uint)payloadAsBytes.Length);
            byte[] message = lengthData.Concat(payloadAsBytes).ToArray();

            await _stream.WriteAsync(message, 0, message.Length);
        }

        private async Task<JsonObject?> ReceiveAsJsonObject()
        {
            byte[] lengthArray = new byte[4];

            await _stream.ReadAsync(lengthArray, 0, lengthArray.Length);
            uint length = BitConverter.ToUInt32(lengthArray, 0);

            byte[] payloadBuffer = new byte[length];
            int totalBytesRead = 0;

            while (totalBytesRead < length)
            {
                int bytesRead = await _stream.ReadAsync(payloadBuffer, totalBytesRead, payloadBuffer.Length - totalBytesRead);
                totalBytesRead += bytesRead;
            }

            string messageAsString = _encoding.GetString(payloadBuffer, 0, totalBytesRead);
            return JsonSerializer.Deserialize<JsonObject>(messageAsString)?.AsObject();
        }
    }
}
