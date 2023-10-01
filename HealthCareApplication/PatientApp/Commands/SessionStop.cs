using PatientApp.DeviceConnection;
using System;
using System.Text.Json.Nodes;
using Utilities.Communication;

namespace PatientApp.Commands
{
    public class SessionStop : ISessionCommand
    {
        private EventHandler<Statistic> _onReceiveDataDevMgr;
        private readonly EventHandler<Statistic> _onReceiveData;

        public SessionStop(EventHandler<Statistic> onReceiveDataDevMgr, EventHandler<Statistic> onReceiveData)
        {
            _onReceiveDataDevMgr = onReceiveDataDevMgr;
            _onReceiveData = onReceiveData;
        }

        public bool Execute(JsonObject data, ClientConn conn)
        {
            // TODO This doesn't work yet, investigate this
            _onReceiveDataDevMgr -= _onReceiveData;
            Console.WriteLine("======= Session stopped");
            return true;
        }
    }
}
