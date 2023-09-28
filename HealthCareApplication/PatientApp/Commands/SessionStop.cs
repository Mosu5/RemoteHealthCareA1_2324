using System;
using System.Text.Json.Nodes;
using Utilities.Communication;

namespace PatientApp.Commands
{
    public class SessionStop : ISessionCommand
    {
        public bool Execute(JsonObject data, ClientConn conn)
        {
            throw new NotImplementedException();
        }
    }
}
