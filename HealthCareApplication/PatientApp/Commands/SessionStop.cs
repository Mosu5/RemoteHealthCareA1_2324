using System;
using System.Text.Json.Nodes;

namespace PatientApp.Commands
{
    internal class SessionStop : ISessionCommand
    {
        public bool Execute(JsonObject data)
        {
            throw new NotImplementedException();
        }
    }
}
