using System.Text.Json.Nodes;

namespace PatientApp.Commands
{
    public class LoginResponse: ISessionCommand
    {
        public bool Execute(JsonObject data)
        {
            throw new System.NotImplementedException();
        }
    }
}