using System;
using System.Text.Json.Nodes;

namespace PatientApp.Commands
{
    internal class SessionStart : ISessionCommand
    {
        //private readonly IReceiver receiver;
        //private readonly EventHandler<object> _onReceiveData;

        //public SessionStart(IReceiver receiver, EventHandler<object> onReceiveData)
        //{
        //    this.receiver = receiver;
        //    _onReceiveData = onReceiveData;
        //}

        public bool Execute(JsonObject data)
        {
            //// Subscribe to all bike and HRM events.
            //receiver.ReceivedSpeed += _onReceiveData;
            //receiver.ReceivedDistance += _onReceiveData;
            //receiver.ReceivedHeartRate += _onReceiveData;
            //receiver.ReceivedRrIntervals += _onReceiveData;
            return true;
        }
    }
}
