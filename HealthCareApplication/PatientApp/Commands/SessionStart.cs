using PatientApp.DeviceConnection;
using PatientApp.DeviceConnection.Receiver;
using System;
using System.Text.Json.Nodes;
using Utilities.Communication;

namespace PatientApp.Commands
{
    internal class SessionStart : ISessionCommand
    {
        private EventHandler<Statistic> _onReceiveDataDevMgr;
        private readonly EventHandler<Statistic> _onReceiveData;

        public SessionStart(EventHandler<Statistic> onReceiveDataDevMgr, EventHandler<Statistic> onReceiveData)
        {
            _onReceiveDataDevMgr = onReceiveDataDevMgr;
            _onReceiveData = onReceiveData;
        }

        public bool Execute(JsonObject data, ClientConn conn)
        {
            _onReceiveDataDevMgr += _onReceiveData;
            Console.WriteLine("======= Session started");
            return true;
        }
    }
}
