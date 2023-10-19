using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    /// <summary>
    /// Class to handle the states of the server with incomming commando's
    /// </summary>
    internal class ServerContext
    {
        public bool isSessionActive { get; set; }
        private UserAccount _userAccount { get; set; }
        private IState currentState { get; set; }
        private IState nextState { get; set; }
        private ServerConn _serverConn { get; set; }
        public JsonObject ResponseToClient { get; set; }
        public List<UserStat> userStatsBuffer;

        public ServerContext(ServerConn serverConn)
        {
            this._serverConn = serverConn;
            this.isSessionActive = false;
            currentState = new LoginState(this);
            this.userStatsBuffer = new List<UserStat>();
            
        }

        public void SetNextState(IState newState)
        {
            nextState = newState;
        }

        public void Update(JsonObject receivedData)
        {
            if (receivedData == null)
            {
                throw new NullReferenceException("Error while receiving JsonObject");
            }
 
            currentState = currentState.Handle(receivedData);
            
            
        }

        public void SaveUserData()
        {
            string userData = JsonSerializer.Serialize(userStatsBuffer);

            this._userAccount.SaveUserStats(userData);
        }

        public void SetNewUser(UserAccount user)
        {
            this._userAccount = user;
        }

        public UserAccount GetUserAccount() { return this._userAccount; }

        // Gets the userstats from the current session
        public List<UserStat> GetUserStatsBuffer()
        {
            return this.userStatsBuffer;
        }
        public void ResetUserStatsBuffer()
        {
            this.userStatsBuffer.Clear();
        }

        internal string GetState()
        {
            return this.currentState.ToString();
        }
    }
}
