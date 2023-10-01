using PatientApp.BikeConnection;
using PatientApp.BikeConnection.Receiver;
using System;
using System.Text.Json.Nodes;
using Utilities.Communication;

namespace PatientApp.Commands
{
    internal class SessionStart : ISessionCommand
    {
        private EventHandler<Statistic> _onReceiveDataClient;
        private readonly EventHandler<Statistic> _onReceiveData;

        public SessionStart(EventHandler<Statistic> onReceiveDataClient, EventHandler<Statistic> onReceiveData)
        {
            _onReceiveDataClient = onReceiveDataClient;
            _onReceiveData = onReceiveData;
        }

        public bool Execute(JsonObject data, ClientConn conn)
        {
            // This doesn't work yet, investigate this
            _onReceiveDataClient += _onReceiveData;
            Console.WriteLine("Session started");
            return true;
        }
    }
}
