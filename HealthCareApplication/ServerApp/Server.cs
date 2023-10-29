using ServerApp.States;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApp
{
    public class Server
    {
        // All user accounts
        public static List<UserAccount> Users = new List<UserAccount>()
        {
            new UserAccount("bob", "bob"),
            new UserAccount("jan", "jan"),
            new UserAccount("eren", "eren"),
            new UserAccount("abc", "abc")
        };

        // TODO add possibility of multiple doctors connected at the same time
        private static TcpClient _doctorClient;

        public static async Task Main(string[] args)
        {
            // Proof of concept, uitbreinding/refining later mogelijk
            UserAccount doctor = new UserAccount("dokter", "simon");
            doctor.IsDoctor = true;
            Users.Add(doctor);

            // Start up listener for incoming connections
            ServerConn.StartListener("127.0.0.1", 8888);

            // Runs every time a device (client) connects to the server listener
            while (ServerConn.AcceptClient() is var client)
            {
                await Console.Out.WriteLineAsync("A client has connected");

                // Start a client handler specific to the client that just connected on a new thread
                Thread clientThread = new Thread(async() => await HandlePatientAsync(client));
                clientThread.Start();
            }
        }

        /// <summary>
        /// Handles communication with a client by responding to it and sending messages to it.
        /// </summary>
        /// <param name="client">Object containing the concrete connection to the client</param>
        public static async Task HandlePatientAsync(TcpClient client)
        {
            ServerContext serverContext = new ServerContext(client);

            while (client.Connected)
            {
                await Console.Out.WriteLineAsync($"Set server state to {serverContext.GetState()}. Looking for data...");

                // Block until the client sends a message to the server
                JsonObject data = await ServerConn.ReceiveJson(client);

                await Console.Out.WriteLineAsync($"Client ({client.Client.RemoteEndPoint}) sent: {data}");

                // Update the server context, it might transition to a new state.
                serverContext.Update(data);
               
                // Check if user is doctor according to account class
                if(serverContext.GetUserAccount() != null && serverContext.GetUserAccount().IsDoctor)
                {
                    // Create a new thread for handling the doctor
                    Thread doctorThread = new Thread(HandleDoctorAsync);
                    doctorThread.Start(new object[] { serverContext, client });

                    // Send the response of the client
                    // TODO is this correct? Not .ResponseToDoctor?
                    await ServerConn.SendJson(client, serverContext.ResponseToPatient);

                    return; // Close this thread since the doctor has another thread
                }

                // Check if there was a message for the doctor. If so, send it.
                if(_doctorClient != null && serverContext.ResponseToDoctor != null)
                {
                    await Console.Out.WriteLineAsync("Response to Doctor: " + serverContext.ResponseToDoctor.ToString());
                    await ServerConn.SendJson(_doctorClient, serverContext.ResponseToDoctor);
                }

                // Send response according to updated server context
                if (serverContext.ResponseToPatient != null)
                    await ServerConn.SendJson(client, serverContext.ResponseToPatient);
            }
        }

        /// <summary>
        /// Handle Doctor connection. Pass in the existing servercontext because server knows the useraccount information
        /// </summary>
        public static async void HandleDoctorAsync(object doctorParams)
        {
            object[] parameterArray = (object[])doctorParams;
            TcpClient tcpClient = parameterArray[1] as TcpClient;
            _doctorClient = tcpClient;

            // TODO maybe make static
            DoctorHandler doctorHandler = new DoctorHandler(); 

            while (tcpClient.Connected) 
            {
                // Listen to messages from doctor
                JsonObject data = await ServerConn.ReceiveJson(tcpClient);

                // Determine the data for the patient by the doctor handler.
                // If the data was malformed, go to the next iteration of the while loop.
                if (!doctorHandler.Handle(data)) continue;

                // Get the user account to which the doctor is communicating
                UserAccount receivingPatient = GetUserByName(doctorHandler.PatientToRespondTo);

                // Get the receiving patient's TcpClient
                TcpClient patientToRespondTo = receivingPatient.UserClient;

                // If so, then the doctor sent a request to a patient that is not logged in/does not exist.
                if (patientToRespondTo == null)
                {
                    // TODO better handle this edge case, the doctor still assumes that everything went well
                    return;
                }

                // The message to send to the patient
                JsonObject msgPayload = doctorHandler.ResponseValue;

                // Send command from the DoctorWPFApp through server to the correct patient
                await ServerConn.SendJson(patientToRespondTo, msgPayload);
            }
        }

        /// <summary>
        /// Goes through the list of user accounts and gets the user account with the corresponding name, or if absent null.
        /// </summary>
        private static UserAccount GetUserByName(string name) 
        {
            foreach (UserAccount user in Users)
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
    