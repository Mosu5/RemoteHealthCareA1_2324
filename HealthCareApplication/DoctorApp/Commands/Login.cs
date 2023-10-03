using DoctorApp.Helpers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace DoctorApp.Commands
{
    internal class Login : IDoctorCommand
    {
        private readonly string _username;
        private readonly string _password;

        public Login(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public async Task<bool> Execute(ClientConn clientConn)
        {
            // Send out the request
            JsonObject request = DoctorFormat.LoginMessage(_username, _password);
            await clientConn.SendJson(request);

            // Receive the response, and look for the JSON keypair 'status'
            JsonObject response = await clientConn.ReceiveJson();
            JsonNode[] nodeKeys = DoctorFormat.GetKeys(response, "login", "status");
            string statusCode = nodeKeys[0].ToString();

            return statusCode.Equals("ok");
        }
    }
}
