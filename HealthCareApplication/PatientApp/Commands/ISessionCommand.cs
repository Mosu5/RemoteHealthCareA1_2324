using System.Text.Json.Nodes;

namespace PatientApp.Commands
{
    internal interface ISessionCommand
    {
        bool Execute(JsonObject data);
    }
}
