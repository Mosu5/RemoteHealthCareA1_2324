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
    }
}