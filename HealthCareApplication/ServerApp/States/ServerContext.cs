using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public ServerContext() 
        {
            currentState = new SessionStarting();
        }

        public void SetNextState(IState newState)
        {
            nextState = newState;
            //byte[] bytes = "kokok".
        }

        

    }
}
