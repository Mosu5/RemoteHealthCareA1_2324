using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class SessionIdle : IState
    {
        public IState Handle(JsonObject packet)
        {

            JsonObject data = packet.GetValue("data");
            if (data.ContainsKey("stats/summary"))
            {
                // Give summary


            }
        }
    }
}
