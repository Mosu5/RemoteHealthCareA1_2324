using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace PatientApp.Commands
{
    /// <summary>
    /// Handle the login between client and server
    /// When no response is submitted in Execute(), send login data to server
    /// Otherwise handle response
    /// </summary>
    internal class Login : ISessionCommand
    {
        // Attempt login when data is null 
        // Otherwise check for errors in response

        public bool Execute(JsonObject data, ClientConn conn)
        {
            Console.WriteLine(data.ToString());

            Task sendJsonTask = conn.SendJson(data);
            sendJsonTask.Wait();

            return true;
        }
    }
}