using PatientApp.DeviceConnection;
using System;
using Utilities.Communication;

namespace PatientApp.Commands
{
    internal class SendStats : ISessionCommand
    {
        private Statistic _statistic;
        private ClientConn _conn;

        public SendStats(Statistic stat, ClientConn clientConn)
        {
            _statistic = stat;
            _conn = clientConn;
        }

        public void Execute()
        {
            var sendStatsMessage = PatientFormat.SendStatsMessage(_statistic.Speed, _statistic.Distance, _statistic.HeartRate);

            _conn.SendJson(sendStatsMessage).Wait();
        }
    }
}
