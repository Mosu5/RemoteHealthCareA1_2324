using PatientApp.DeviceConnection;
using System;

namespace PatientApp.Commands
{
    internal class SessionPause : ISessionCommand
    {
        private EventHandler<Statistic> _onReceiveDataDevMgr;
        private readonly EventHandler<Statistic> _onReceiveData;

        public SessionPause(EventHandler<Statistic> onReceiveDataDevMgr, EventHandler<Statistic> onReceiveData)
        {
            _onReceiveDataDevMgr = onReceiveDataDevMgr;
            _onReceiveData = onReceiveData;
        }

        /// <summary>
        /// Unsubscribes from data receive event
        /// </summary>
        public void Execute()
        {
            _onReceiveDataDevMgr -= _onReceiveData;
            Console.WriteLine("======= Session paused");
        }
    }
}
