using PatientApp.PatientLogic.Helpers;
using System;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;
using Utilities.Logging;

namespace PatientApp.PatientLogic.Commands
{
    public class StatsSummary : IPatientCommand
    {
        public async Task<bool> Execute()
        {
            Request request = new Request(PatientFormat.StatsSummaryMessage());
            JsonObject response = await RequestHandler.GetResponse(request);

            Logger.Log($"Response was: {response}", LogType.Debug);

            return true;
        }
    }
}
