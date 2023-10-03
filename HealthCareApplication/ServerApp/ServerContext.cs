using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    /// <summary>
    /// Class to handle the states of the server with incomming commando's
    /// </summary>
    internal class ServerContext
    {

        private UserAccount _userAccount { get; set; }
        private IState currentState {  get; set; }
        private IState nextState { get; set;}
        private ServerConn _serverConn { get; set; }
        public JsonObject ResponseToClient { get; set; }
        public ServerContext(ServerConn serverConn) 
        {
            this._serverConn = serverConn;
            currentState = new LoginState(this);
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
            if (receivedData.ContainsKey("command"))
            {
                string commando = (string)receivedData["commando"];
                Console.WriteLine("Recieved command from client: " + commando);

                switch (commando)
                {
                    case "login":
                        this.nextState = new LoginState(this);
                        break;
                    case "session stop":
                        this.nextState = new SessionStoppedState(this);
                        break;
                    case "session pause":
                        this.nextState = new SessionPausedState();
                        break;
                    case "session start":
                        this.nextState = new SessionActiveState(this);
                        break;
                }

            }

            ApplyNewState();
            currentState.Handle(receivedData);
        }

        public void SetNewUser(UserAccount user)
        {
            this._userAccount = user;
        }

        public UserAccount GetUserAccount() { return this._userAccount; }

        
        //public void Run()
        //{
        //    while (this.stream.DataAvailable)
        //    {
        //        StreamReader streamReader = new StreamReader(stream);
        //        JsonObject receivedData = (JsonObject)streamReader.Read();

        //        if (receivedData != null)
        //        {
        //            throw new NullReferenceException("Error while receiving JsonObject");
        //        }
        //        if (receivedData.ContainsKey("command"))
        //        {
        //            string commando = (string)receivedData["commando"];
        //            Console.WriteLine("Recieved command from client: " + commando);

        //            switch (commando)
        //            {
        //                case "login":
        //                    this.nextState = new LoginState(this);
        //                    break;
        //                case "session stop":
        //                    this.nextState = new SessionStoppedState();
        //                    break;
        //                case "session pause":
        //                    this.nextState = new SessionPausedState();
        //                    break;
        //                case "session start":
        //                    this.nextState = new SessionActiveState(this);
        //                    break;
        //            }
                        
        //        }

        //        ApplyNewState();
        //        currentState.Handle(receivedData);
        //    }
        //}

        private void ApplyNewState()
        {
            if (this.nextState != null)
            {
                this.currentState = this.nextState;
            }
            this.nextState = null;
        }
    }
}
