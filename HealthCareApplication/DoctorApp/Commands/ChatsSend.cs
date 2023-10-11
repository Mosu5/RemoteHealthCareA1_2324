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

        public ChatsSend(string patientUsername, string chatMessage)
        {
            _patientUsername = patientUsername;
            _chatMessage = chatMessage;
        }

        public async Task<bool> Execute()
        {
            Request request = new Request(DoctorFormat.ChatsSendMessage(_patientUsername, _chatMessage));
            JsonObject response = await RequestHandler.GetResponse(request);

            // Expect no response from the server

            return true;
        }
    }
}
