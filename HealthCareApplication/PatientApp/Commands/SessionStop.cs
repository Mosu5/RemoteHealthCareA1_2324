using PatientApp.DeviceConnection;
using System;

namespace PatientApp.Commands
{
    public class SessionStop : ISessionCommand
    {
        private EventHandler<Statistic> _onReceiveDataDevMgr;
        private readonly EventHandler<Statistic> _onReceiveData;

        public SessionStop(EventHandler<Statistic> onReceiveDataDevMgr, EventHandler<Statistic> onReceiveData)
        {
            _onReceiveDataDevMgr = DeviceManager.OnReceiveData;
            _onReceiveData = onReceiveData;
        }

        /// <summary>
        /// Unsubscribes from data receive event
        /// </summary>
        public void Execute()
        {
            _onReceiveDataDevMgr -= _onReceiveData;
            Console.WriteLine("======= Session stopped");
        }
    }
}
