using PatientApp.DeviceConnection;
using System;

namespace PatientApp.Commands
{
    internal class SessionStart : ISessionCommand
    {
        private readonly EventHandler<Statistic> _onReceiveData;

        public SessionStart(EventHandler<Statistic> onReceiveData)
        {
            
            _onReceiveData = onReceiveData;
        }

        /// <summary>
        /// Subscribes to data receive event
        /// </summary>
        public void Execute()
        {
            DeviceManager.OnReceiveData += _onReceiveData;
            Console.WriteLine("======= Session started");
        }
    }
}
