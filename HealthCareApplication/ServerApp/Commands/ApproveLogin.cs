using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.Commands
{
    internal class ApproveLogin : ISessionCommand
    {
        private ServerConn _serverConn;

        public ApproveLogin(ServerConn serverConn) 
        {
            this._serverConn = serverConn;
        }


        public void Execute(JsonObject data)
        {
            
            // Current structure of command usage is not applicable with our application
            // TODO: Refactor this command/state structure: See LoginState.cs for context!


        }



    }
}

