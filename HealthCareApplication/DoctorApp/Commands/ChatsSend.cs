using DoctorApp.Communication;
using DoctorApp.Helpers;
using RHSandbox.Communication;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;
using Utilities.Logging;

namespace DoctorApp.Commands
{
    internal class ChatsSend : IDoctorCommand
    {
        private readonly string _patientUsername;
        private readonly string _chatMessage;
        private readonly ClientConn _clientConn;

        public ChatsSend(string patientUsername, string chatMessage, ClientConn clientConn)
        {
            _patientUsername = patientUsername;
            _chatMessage = chatMessage;
            _clientConn = clientConn;
        }

        public async Task<bool> Execute()
        {
            await _clientConn.SendJson(DoctorFormat.ChatsSendMessage(_patientUsername, _chatMessage));

            // Expect no response from the server

            return true;
        }
    }
}
