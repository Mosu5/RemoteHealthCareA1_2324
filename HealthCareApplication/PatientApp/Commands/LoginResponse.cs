using System;
using System.Text.Json.Nodes;
using Utilities.Communication;

namespace PatientApp.Commands
{
    internal class LoginResponse : ISessionCommand
    {
        private readonly JsonObject _message;

        public LoginResponse(JsonObject message)
        {
            _message = message;
        }

        /// <summary>
        /// Confirms if login was successful
        /// </summary>
        /// <exception cref="CommunicationException">If the response was not correctly formatted or credentials are incorrect.</exception>
        public void Execute()
        {
            if (!_message.ContainsKey("status"))
                throw new CommunicationException("The login message did not contain the JSON key 'status'");

            if (!_message["status"].Equals("ok"))
                throw new CommunicationException("Combination of username and password was incorrect");

            Console.WriteLine("===== Login successful");
        }
    }
}
