using DoctorApp.Helpers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace DoctorApp.Commands
{
    internal class Logout : IDoctorCommand
    {
        public async Task<bool> Execute(ClientConn clientConn)
        {
            string command = "logout";

            // Send out the request
            JsonObject request = DoctorFormat.BaseMessage(command);
            await clientConn.SendJson(request);

            // Receive the response, and look for the JSON keypair 'status'
            JsonObject response = await clientConn.ReceiveJson();
            JsonNode[] nodeKeys = DoctorFormat.GetKeys(response, command, "status");
            string statusCode = nodeKeys[0].ToString();

            return statusCode.Equals("ok");
        }
    }
}
