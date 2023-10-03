using DoctorApp.Helpers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace DoctorApp.Commands
{
    internal class ChatsSend : IDoctorCommand
    {
        private readonly string _chatMessage;

        public ChatsSend(string chatMessage)
        {
            _chatMessage = chatMessage;
        }

        public async Task<bool> Execute(ClientConn clientConn)
        {
            // Send out the request
            JsonObject request = DoctorFormat.ChatsSendMessage(_chatMessage);
            await clientConn.SendJson(request);

            // We currently don't care if the message arrived intact on the server.
            return true;
        }
    }
}
