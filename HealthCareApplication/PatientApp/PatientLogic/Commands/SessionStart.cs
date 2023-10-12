using PatientApp.DeviceConnection;
using PatientApp.PatientLogic;
using PatientApp.PatientLogic.Commands;
using PatientApp.PatientLogic.Helpers;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace PatientApp.PatientLogic.Commands
{
    internal class SessionStart : IPatientCommand
    {
        public async Task<bool> Execute()
        {
            Request request = new Request(PatientFormat.SessionStartMessage());
            JsonObject response = await RequestHandler.GetResponse(request);

            if (!response.ContainsKey("status"))
                throw new CommunicationException("The login message did not contain the JSON key 'status'");

            if (!response["status"].ToString().Equals("ok"))
                return false;

            DeviceManager.OnReceiveData += RequestHandler.OnReceiveData;

            return true;
        }
    }
}
