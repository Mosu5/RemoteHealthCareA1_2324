using PatientApp.BikeConnection.Receiver;
using System;
using System.Text.Json.Nodes;
using Utilities.Communication;

namespace PatientApp.Commands
{
    internal class SessionPause : ISessionCommand
    {
        private readonly IReceiver receiver;
        private readonly EventHandler<object> _onReceiveData;

        public SessionPause(IReceiver receiver, EventHandler<object> onReceiveData)
        {
            this.receiver = receiver;
            _onReceiveData = onReceiveData;
        }

        public bool Execute(JsonObject data, ClientConn conn)
        {
            //// Subscribe to all bike and HRM events.
            //receiver.ReceivedSpeed -= _onReceiveData;
            //receiver.ReceivedDistance -= _onReceiveData;
            //receiver.ReceivedHeartRate -= _onReceiveData;
            //receiver.ReceivedRrIntervals -= _onReceiveData;
            return true;
        }
    }
}
