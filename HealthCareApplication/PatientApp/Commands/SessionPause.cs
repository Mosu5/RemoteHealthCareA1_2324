using PatientApp.DeviceConnection;
using System;

namespace PatientApp.Commands
{
    internal class SessionPause : ISessionCommand
    {
        private readonly EventHandler<Statistic> _onReceiveData;

        public SessionPause(EventHandler<Statistic> onReceiveData)
        {
            _onReceiveData = onReceiveData;
        }

        /// <summary>
        /// Unsubscribes from data receive event
        /// </summary>
        public void Execute()
        {
            DeviceManager.OnReceiveData -= _onReceiveData;
            Console.WriteLine("======= Session paused");
        }
    }
}
