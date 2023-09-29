using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class SessionActiveState : IState
    {
        private ServerContext context;
        public SessionActiveState(ServerContext context)
        {
            this.context = context;
        }

        public void Handle(JsonObject packet)
        {
            throw new NotImplementedException();
        }
    }
}
