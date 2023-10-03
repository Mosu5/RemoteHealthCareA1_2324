using DoctorApp.Helpers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace DoctorApp.Commands
{
    internal class StatsSummary : IDoctorCommand
    {
        public JsonArray Summary { get; private set; }

        public async Task<bool> Execute(ClientConn clientConn)
        {
            string command = "summary";

            // Send out the request
            JsonObject request = DoctorFormat.BaseMessage(command);
            await clientConn.SendJson(request);

            // Receive the response, and look for the JSON keypair 'status'
            JsonObject response = await clientConn.ReceiveJson();
            JsonNode[] nodeKeys = DoctorFormat.GetKeys(response, command, "statistics");
            Summary = nodeKeys[0].AsArray();

            return true;
        }
    }
}
