using DoctorApp.Communication;
using DoctorApp.Helpers;
using RHSandbox.Communication;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace DoctorApp.Commands
{
    public class Login : IDoctorCommand
    {
        private readonly string _username;
        private readonly string _password;

        public Login(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public async Task<bool> Execute()
        {
            Request request = new Request(DoctorFormat.LoginMessage(_username, _password));
            JsonObject response = await DoctorProxy.GetResponse(request);

            if (!response.ContainsKey("status"))
                throw new CommunicationException("The login message did not contain the JSON key 'status'");

            if (response["status"].ToString().Equals("ok"))
                return true;

            return false;
        }
    }
}
