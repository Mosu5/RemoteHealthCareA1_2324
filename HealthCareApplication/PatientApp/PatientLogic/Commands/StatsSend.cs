using PatientApp.DeviceConnection;
using PatientApp.PatientLogic.Helpers;
using System;
using System.Threading.Tasks;
using Utilities.Communication;

namespace PatientApp.PatientLogic.Commands
{
    internal class StatsSend : IPatientCommand
    {
        private Statistic _statistic;
        private ClientConn _clientConn;

        public StatsSend(Statistic statistic, ClientConn clientConn)
        {
            _statistic = statistic;
            // TODO round speed somewhere else in codebase
            _statistic.Speed = Math.Round(statistic.Speed, 2);
            _clientConn = clientConn;
        }

        /// <summary>
        /// Send the patients health statistics to the server
        /// </summary>
        public async Task<bool> Execute()
        {
            await Console.Out.WriteLineAsync("Sending stats: " + PatientFormat.StatsSendMessage(_statistic));
            await _clientConn.SendJson(PatientFormat.StatsSendMessage(_statistic));

            // Expect no response from the server

            return true;
        }
    }
}
