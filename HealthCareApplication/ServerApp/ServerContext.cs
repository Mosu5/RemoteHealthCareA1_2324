using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ServerApp.States
{
    /// <summary>
    /// Class to handle the states of the server with incomming commando's
    /// </summary>
    internal class ServerContext
    {
        // Fields for the response of client/doctor.
        // Used by Server.cs to send the JsonObject over the network
        // to a patient or doctor
        public JsonObject ResponseToPatient { get; set; }
        public JsonObject ResponseToDoctor { get; set; }
        public bool IsSessionActive { get; set; }
        public TcpClient tcpClient { get; set; }
        public List<UserStat> UserStatsBuffer;
        public SslStream UserSslStream { get; set; }

        private IState currentState;
        private UserAccount _userAccount;

        public ServerContext(TcpClient connectingClient, SslStream sslStream)
        {
            // Set the current state to login and set session active to false
            currentState = new LoginState(this);
            IsSessionActive = false;

            UserStatsBuffer = new List<UserStat>();
            tcpClient = connectingClient; // Save connecting client so the user can be associated with this client
            UserSslStream = sslStream;
        }

        /// <summary>
        /// Update the state of the client, enables transitioning to other states.
        /// </summary>
        /// <param name="receivedData">Data that was received over the network parsed as JSON</param>
        /// <exception cref="NullReferenceException">If the received data is null</exception>
        public void Update(JsonObject receivedData)
        {
            if (receivedData == null)
                throw new NullReferenceException("Expected a JsonObject, but got null");

            // Set new state
            currentState = currentState.Handle(receivedData);


            //// Temporary bugfix, search for better alternative later
            //// Dont blame me, there is not much time left
            //if (currentState.GetType() == typeof(SessionStoppedState))
            //    currentState = currentState.Handle(receivedData);


        }

        /// <summary>
        /// Save the buffer of userstats into the user account.
        /// </summary>
        public void SaveUserData()
        {

            // Get previously saved data so current data can be added to the file. (Prevent overwriting)
            List<UserStat> allUserStats = this.GetUserAccount().GetUserStats();//Previous Buffer
            if (allUserStats != null)//if the previous stats are not empty
            {
                // Add new data to old data
                foreach (var item in UserStatsBuffer)
                {
                    allUserStats.Add(item);
                }

                //var allData = previousData.Concat(userStatsBuffer);
                string userData = JsonSerializer.Serialize(allUserStats);
                this._userAccount.SaveUserStats(userData);
            }
            else
            {
                string userData = JsonSerializer.Serialize(UserStatsBuffer);
                this._userAccount.SaveUserStats(userData);
            }

        }

        /// <summary>
        /// Set the user account of the client. For when the client logs in with the credentials
        /// of their user account.
        /// </summary>
        public void SetNewUser(UserAccount user)
        {
            _userAccount = user;
        }

        public UserAccount GetUserAccount() { return _userAccount; }

        // Gets the userstats from the current session
        public List<UserStat> GetUserStatsBuffer()
        {
            return UserStatsBuffer;
        }
        public void ResetUserStatsBuffer()
        {
            UserStatsBuffer.Clear();
        }

        /// <summary>
        /// Gets the current state as a string.
        /// </summary>
        internal string GetState()
        {
            return currentState.ToString();
        }
    }
}
