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
    public class Server
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
            UserAccount doctor = new UserAccount("dokter", "simon");
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
            ServerContext serverContext = new ServerContext(serverConn, client);
            while (client.Connected)
            {
                await Console.Out.WriteLineAsync(serverContext.GetState().ToString());
                Console.Out.WriteLineAsync("Looking for data: ");
                JsonObject data = await serverConn.ReceiveJson(client);
                await Console.Out.WriteLineAsync("received " + data.ToString());
                serverContext.Update(data);
               
                // Check if user is doctor according to account class
                if(serverContext.GetUserAccount() != null && serverContext.GetUserAccount().IsDoctor())
                {
                    Thread doctorThread = new Thread(HandleDoctorAsync);
                    doctorThread.Start(new object[] { serverContext, client });
                    await serverConn.SendJson(client, serverContext.ResponseToClient);
                    return; // Close this thread since the doctor has another thread
                }
                if(doctorClient != null)
                {
                    if (serverContext.ResponseToDoctor != null)
                    {
                        await Console.Out.WriteLineAsync("Response to Doctor:" + serverContext.ResponseToDoctor.ToString());
                        await serverConn.SendJson(doctorClient, serverContext.ResponseToDoctor);
                    }
                }

                // Send response according to updated server context
                await serverConn.SendJson(client, serverContext.ResponseToClient);

                // TODO TESTING
                //if (data["command"].ToString() == "chats/send")
                //{
                //    serverConn.SendJson(client, new JsonObject()
                //    {
                //        { "command", "chats/send" },
                //        { "data", new JsonObject()
                //        {
                //            { "message", "server message" }
                //        } }
                //    }).Wait();
                //}
            }
        }

        // Handle Doctor connection. Pass in the existing servercontext because server knows the useraccount information
        public static async void HandleDoctorAsync(object doctorParams)
        {
            DoctorHandler doctorHandler = new DoctorHandler(); 

            object[] parameterArray = (object[])doctorParams;
            ServerContext serverContext = parameterArray[0] as ServerContext;
            TcpClient tcpClient = parameterArray[1] as TcpClient;
            doctorClient = tcpClient;

            while (tcpClient.Connected) 
            {
                
                // Listen to messages from doctor client
                JsonObject data = await serverConn.ReceiveJson(tcpClient);

                // Determine the data for the patient by the doctor handler
                doctorHandler.Handle(data);

                UserAccount receivingUser = getUserByName(doctorHandler.userToRespondTo);
                TcpClient clientToRespond = receivingUser.userClient;
                JsonObject msgPayload = doctorHandler.responseValue;

                // Send command from the doctorWPFclient through server to the correct patient
                await serverConn.SendJson(clientToRespond, msgPayload);

            }

        }

        private static UserAccount getUserByName(string name) 
        {

            foreach (UserAccount user in Server.users)
            {
                if (name.Equals(user.GetUserName()))
                {
                    return user;
                }
            }

            return null;

        }
    }   
}
    