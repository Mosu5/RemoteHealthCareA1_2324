using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class SessionPausedState : IState
    {
        public IState Handle(JsonObject packet)
        {
            throw new NotImplementedException();
        }
    }
}
