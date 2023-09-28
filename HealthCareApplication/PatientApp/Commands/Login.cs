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
        public bool Execute(JsonObject data)
        {
            Console.WriteLine(data.ToString());
            DataTransfer.SendJson(data);
            return true;
            //throw new NotImplementedException();
            if (data == null || data["data"] == null) // no response data, so we try logging in
            {
                Task loginTask = SendLoginInfo();
                loginTask.Wait();
                return true;
            }

            // string json = new
            // {
            //     command,
            //     new
            //     {
            //         status
            //         login,
            //         password
            //     }
            // };

            string status = data["data"]?["status"]?.GetValue<string>();

            if (status == "error")
            {
                throw new CommunicationException("Combination of username and password was incorrect");
            }

            return true;
        }

        /// <summary>
        /// Send a json with login data to server
        /// </summary>
        /// <exception cref="CommunicationException"> Exception when data or connection is incomplete or corrupt</exception>
        private async Task SendLoginInfo()
        {
            if (_username == string.Empty || _password == string.Empty)
            {
                throw new CommunicationException("Login data is not filled.");
            }

            ;

            // create object
            object payload = new
            {
                command = "login",
                data = new
                {
                    username = _username,
                    password = _password
                }
            };

            string json = JsonSerializer.Serialize(payload, new JsonSerializerOptions() { WriteIndented = true });
            Console.WriteLine("payload: " + json);

            // await DataTransfer.SendJson(payload);
        }

        private string _username;
        private string _password;

        public string Username
        {
            set => _username = value;
        }

        public string Password
        {
            set => _password = value;
        }

        public Login()
        {
        }

        public Login(string username, string password)
        {
            _username = username;
            _password = password;
        }
    }
}