using System.Text.Json.Nodes;
using Utilities.Communication;

namespace PatientApp.Commands
{
    public interface ISessionCommand
    {
        bool Execute(JsonObject data, ClientConn conn);
    }
}
