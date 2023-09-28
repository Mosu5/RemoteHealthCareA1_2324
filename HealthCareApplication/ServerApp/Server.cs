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

namespace ServerApp
{
    internal class Server
    {


        public static void Main(string[] args)
        {

            StartServer();
        }
        

        public static void StartServer()
        {
            try
            {

                // Start server socket
                int port = 8888;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                TcpListener server = new TcpListener(localAddr, port);
                server.Start();
                // Accept incoming clients and make new thread
                Socket handler = server.AcceptSocket();

                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(handler);
            }
            catch (Exception e){
                Console.WriteLine(e.StackTrace);
            }
        }

        public static void HandleClient(object socket)
        {
            Socket clientSocket = socket as Socket;
            NetworkStream networkStream = new NetworkStream(clientSocket);
            
            while (clientSocket.Connected)
            {
                byte[] buffer = new byte[1024];
                
                networkStream.Read(buffer, 0, buffer.Length);
                string responseString = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                string correctStr = responseString.Substring(4).Trim(' ');
                Console.WriteLine(correctStr);
                Console.WriteLine((JsonObject)JsonObject.Parse(correctStr));

                Console.WriteLine(JsonSerializer.Deserialize<JsonObject>(correctStr)?.AsObject());
                //JsonObject jsonResponse = JsonSerializer.Deserialize<JsonObject>(responseString);
                //Console.WriteLine(jsonResponse.ToString());



            }
        }
    }
    
}