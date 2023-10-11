using DoctorApp.Communication;
using DoctorApp.Helpers;
using RHSandbox.Communication;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;
using Utilities.Logging;

namespace DoctorApp.Commands
{
    internal class StatsSummary : IDoctorCommand
    {
        private readonly string _clientUsername;

        public StatsSummary(string clientUsername)
        {
            _clientUsername = clientUsername;
        }

        public async Task<bool> Execute()
        {
            Request request = new Request(DoctorFormat.StatsSummaryMessage(_clientUsername));
            JsonObject response = await DoctorProxy.GetResponse(request);

            Logger.Log($"Response was: {response}", LogType.Debug);

            return true;
        }
    }
}
