using ServerApp.States;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Communication;

namespace ServerApp
{
    internal class Server
    {
        private static ServerConn serverConn = new ServerConn("127.0.0.1", 8888);

        public static async Task Main(string[] args)
        {
            serverConn.StartListener();

            while (serverConn.AcceptClient() is var client)
            {
                await Console.Out.WriteLineAsync("A client has connected");
                Thread clientThread = new Thread(HandleClientAsync);
                clientThread.Start(client);
            }
        }

        public static async void HandleClientAsync(object connectingClient)
        {
            TcpClient client = connectingClient as TcpClient;

            while (client.Connected)
            {
                JsonObject data = await serverConn.ReceiveJson(client);
                await Console.Out.WriteLineAsync("received " + data.ToString());
            }
        }
    }
    
}