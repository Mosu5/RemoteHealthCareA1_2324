using System.Text.Json.Nodes;
using Utilities.Communication;

namespace PatientApp.Commands
{
    internal class Login : ISessionCommand
    {
        private readonly JsonObject _dataObject;
        private readonly ClientConn _connection;

        public Login(JsonObject dataObject, ClientConn connection)
        {

            Console.WriteLine(data.ToString());
            conn.SendJson(data);

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
            Console.WriteLine(status);

            if (status == "error")
            {
                throw new CommunicationException("Combination of username and password was incorrect");
            }
            _dataObject = dataObject;
            _connection = connection;
        }

        /// <summary>
        /// Handle the login from client to server
        /// </summary>
        /// <exception cref="CommunicationException">If the response was not correctly formatted with username and password.</exception>
        public void Execute()
        {
            if (!_dataObject.ContainsKey("username"))
                throw new CommunicationException("The message did not contain the JSON key 'username'");

            if (!_dataObject.ContainsKey("password"))
                throw new CommunicationException("The message did not contain the JSON key 'password'");

            string username = _dataObject["username"].ToString();
            string password = _dataObject["password"].ToString();

            _connection.SendJson(PatientFormat.LoginMessage(username, password)).Wait();
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