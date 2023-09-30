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

        private IState currentState {  get; set; }
        private IState nextState { get; set;}
        private NetworkStream stream { get; set; }
        public ServerContext(NetworkStream stream) 
        {
            this.stream = stream;
            currentState = new LoginState(this);
        }

        public void SetNextState(IState newState)
        {
            nextState = newState;
        }
        
        public void Run()
        {
            while (this.stream.DataAvailable)
            {
                StreamReader streamReader = new StreamReader(stream);
                JsonObject receivedData = (JsonObject)streamReader.Read();

                if (receivedData != null)
                {
                    throw new NullReferenceException("Error while receiving JsonObject");
                }
                if (receivedData.ContainsKey("command"))
                {
                    string commando = (string)receivedData["commando"];

                    switch (commando)
                    {
                        case "login":
                            this.nextState = new LoginState(this);
                            break;
                        case "session stop":
                            this.nextState = new SessionStoppedState();
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
        }

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
