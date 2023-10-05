using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class CreateAccountState : IState
    {
        private ServerContext context;
        public CreateAccountState(ServerContext context)
        {

        }

        public IState Handle(JsonObject packet)
        {
            throw new NotImplementedException();
            return this;
        }
    }
}
