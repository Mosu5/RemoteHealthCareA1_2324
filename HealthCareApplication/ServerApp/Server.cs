using Utilities.Encryption;
using ServerApp.States;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
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
            new UserAccount("bob", Encryption.ComputeSha256Hash("bob")),
            new UserAccount("jan", Encryption.ComputeSha256Hash("jan")),
            new UserAccount("eren", Encryption.ComputeSha256Hash("eren")),
            new UserAccount("abc", Encryption.ComputeSha256Hash("abc"))
        };

        // TODO add possibility of multiple doctors connected at the same time
        private static SslStream _doctorSslStream;

        public static async Task Main(string[] args)
        {
            // Proof of concept, uitbreinding/refining later mogelijk
            UserAccount doctor = new UserAccount("dokter", Encryption.ComputeSha256Hash("simon"));
            doctor.IsDoctor = true;
            Users.Add(doctor);

            foreach (var user in Users) 
            {
                    user.GetPassword();
                
            }

           

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


        private static SslStream CreateSslStream(TcpClient client)
        {
            // Load the server's SSL certificate
            X509Certificate2 serverCertificate = new X509Certificate2(ServerConn._certificatePath, ServerConn._certificatePassword, X509KeyStorageFlags.UserKeySet);

            // Create an SSL stream and authenticate the client
            SslStream sslStream = new SslStream(client.GetStream(), false);
            sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.None, true);

            return sslStream;
        }

        /// <summary>
        /// Handles communication with a client by responding to it and sending messages to it.
        /// </summary>
        /// <param name="client">Object containing the concrete connection to the client</param>
        public static async Task HandlePatientAsync(TcpClient client)
        {

            SslStream sslStream = CreateSslStream(client);
            ServerContext serverContext = new ServerContext(client, sslStream);

            while (client.Connected)
            {
                await Console.Out.WriteLineAsync($"Set server state to {serverContext.GetState()}. Looking for data...");

                // Block until the client sends a message to the server
                JsonObject data = await ServerConn.ReceiveJson(sslStream);

                // Dispose this thread when data is null, aka when the patient has disconnected.
                if (data == null) return;

                await Console.Out.WriteLineAsync($"Client ({client.Client.RemoteEndPoint}) sent: {data}");

                // Update the server context, it might transition to a new state.
                serverContext.Update(data);
               
                // Check if user is doctor according to account class
                if(serverContext.GetUserAccount() != null && serverContext.GetUserAccount().IsDoctor)
                {
                    // Create a new thread for handling the doctor
                    Thread doctorThread = new Thread(HandleDoctorAsync);
                    doctorThread.Start(new object[] { serverContext, client, sslStream });

                    // Send the response of the client
                    // TODO is this correct? Not .ResponseToDoctor?
                    await ServerConn.SendJson(sslStream, serverContext.ResponseToPatient);

                    return; // Close this thread since the doctor has another thread
                }

                // Check if there was a message for the doctor. If so, send it.
                if(_doctorSslStream != null && serverContext.ResponseToDoctor != null)
                {
                    await Console.Out.WriteLineAsync("Response to Doctor: " + serverContext.ResponseToDoctor.ToString());
                    await ServerConn.SendJson(_doctorSslStream, serverContext.ResponseToDoctor);
                }

                // Send response according to updated server context
                if (serverContext.ResponseToPatient != null)
                    await ServerConn.SendJson(sslStream, serverContext.ResponseToPatient);
            }
        }

        /// <summary>
        /// Handle Doctor connection. Pass in the existing servercontext because server knows the useraccount information
        /// </summary>
        public static async void HandleDoctorAsync(object doctorParams)
        {
            object[] parameterArray = (object[])doctorParams;
            TcpClient tcpClient = parameterArray[1] as TcpClient;
            SslStream doctorSslStream = parameterArray[2] as SslStream;
            _doctorSslStream = doctorSslStream;

            // Doctor needs a list of patients in the server send this before handling any commands
            await ServerConn.SendJson(_doctorSslStream, ResponseClientData.GenerateUsersInfo(Users));

            // TODO maybe make static
            DoctorHandler doctorHandler = new DoctorHandler(); 

            while (tcpClient.Connected) 
            {
                // Listen to messages from doctor
                JsonObject data = await ServerConn.ReceiveJson(doctorSslStream);

                // Dispose this thread when data is null, aka when the doctor has disconnected.
                if (data == null) return;

                // Determine the data for the patient by the doctor handler.
                // If the data was malformed, go to the next iteration of the while loop.
                if (!doctorHandler.Handle(data)) continue;

                // Get the user account to which the doctor is communicating
                UserAccount receivingPatient = GetUserByName(doctorHandler.PatientToRespondTo);

                // Get the receiving patient's TcpClient
                SslStream patientSslStream = receivingPatient.SslStream;

                // If so, then the doctor sent a request to a patient that is not logged in/does not exist.
                if (patientSslStream == null)
                {
                    // TODO better handle this edge case, the doctor still assumes that everything went well
                    return;
                }

                // The message to send to the patient
                JsonObject msgPayload = doctorHandler.ResponseValue;

                // Send command from the DoctorWPFApp through server to the correct patient
                await ServerConn.SendJson(patientSslStream, msgPayload);

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
    