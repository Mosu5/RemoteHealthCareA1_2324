using DoctorApp.Communication;
using DoctorApp.Helpers;
using RHSandbox.Communication;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;
using Utilities.Logging;

namespace DoctorApp.Commands
{
    internal class SessionStart : IDoctorCommand
    {
        private readonly string _patientUsername;

        public SessionStart(string patientUsername)
        {
            _patientUsername = patientUsername;
        }

        public async Task<bool> Execute()
        {
            Request request = new Request(DoctorFormat.SessionStartMessage(_patientUsername));
            JsonObject response = await DoctorProxy.GetResponse(request);

            if (!response.ContainsKey("status"))
                throw new CommunicationException("The login message did not contain the JSON key 'status'");

            if (!response["status"].ToString().Equals("ok"))
                return false;

            return true;
        }
    }
}
