using PatientApp.DeviceConnection;
using System;

namespace PatientApp.Commands
{
    internal class SessionResume : ISessionCommand
    {
        private readonly EventHandler<Statistic> _onReceiveData;

        public SessionResume(EventHandler<Statistic> onReceiveData)
        {
            _onReceiveData = onReceiveData;
        }

        /// <summary>
        /// Subscribes to data receive event
        /// </summary>
        public void Execute()
        {
            DeviceManager.OnReceiveData += _onReceiveData;
            Console.WriteLine("======= Session resumed");
        }
    }
}
