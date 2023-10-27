using ServerApp.States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Communication;
using Utilities.Logging;

namespace ServerApp
{
    internal class Server
    {
        private static ServerConn serverConn = new ServerConn("127.0.0.1", 8888);
        public static List<UserAccount> users = new List<UserAccount>();//List of users
        private static TcpClient doctorClient;

        public static async Task Main(string[] args)
        {
            users.Add(new UserAccount("bob","bob"));
            users.Add(new UserAccount("jan", "jan"));
            users.Add(new UserAccount("eren", "eren"));
            users.Add(new UserAccount("abc", "abc"));

            // Proof of concept, uitbreinding/refining later mogelijk
            UserAccount doctor = new UserAccount("simon", "dokter");
            doctor.isDoctor = true;

            users.Add(doctor);
            serverConn.StartListener();
            

            while (serverConn.AcceptClient() is var client)
            {
                Console.Out.WriteLineAsync("A client has connected");

                Thread clientThread = new Thread(HandleClientAsync);
                clientThread.Start(client);
            }
        }

        // Session of an active user
        public static async void HandleClientAsync(object connectingClient)
        {
            TcpClient client = connectingClient as TcpClient;
            ServerContext serverContext = new ServerContext(serverConn);
            while (client.Connected)
            {
                await Console.Out.WriteLineAsync(serverContext.GetState().ToString());
                Console.Out.WriteLineAsync("Looking for data: ");
                JsonObject data = await serverConn.ReceiveJson(client);
                await Console.Out.WriteLineAsync("received " + data.ToString());
                serverContext.Update(data);

                if(serverContext.GetUserAccount() != null && serverContext.GetUserAccount().IsDoctor())
                {
                    Thread doctorThread = new Thread(HandleDoctorAsync);
                    doctorThread.Start(new object[] { serverContext, client });
                }
                if(doctorClient != null)
                {

                    if (serverContext.ResponseToDoctor != null)
                    {
                        await Console.Out.WriteLineAsync("Response to Doctor:" + serverContext.ResponseToDoctor.ToString());
                        await serverConn.SendJson(doctorClient, serverContext.ResponseToDoctor);
                    }

                }
                await serverConn.SendJson(client, serverContext.ResponseToClient);
            }
        }

        // Handle Doctor connection. Pass in the existing servercontext because server knows the useraccount information
        public static async void HandleDoctorAsync(object doctorParams)
        {
            object[] parameterArray = (object[])doctorParams;
            ServerContext serverContext = parameterArray[0] as ServerContext;
            TcpClient tcpClient = parameterArray[1] as TcpClient;
            doctorClient = tcpClient;

            while (tcpClient.Connected) 
            {

                

            }

        }
    }   
}
    