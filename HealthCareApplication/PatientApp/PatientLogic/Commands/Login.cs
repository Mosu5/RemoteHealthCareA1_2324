using PatientApp.PatientLogic;
using PatientApp.PatientLogic.Commands;
using PatientApp.PatientLogic.Helpers;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace PatientApp.PatientLogic.Commands
{
    internal class Login : IPatientCommand
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
            Request request = new Request(PatientFormat.LoginMessage(_username, _password));
            JsonObject response = await RequestHandler.GetResponse(request);

            if (!response.ContainsKey("status"))
                throw new CommunicationException("The login message did not contain the JSON key 'status'");

            if (!response["status"].ToString().Equals("ok"))
                return false;

            return true;
        }
    }
}