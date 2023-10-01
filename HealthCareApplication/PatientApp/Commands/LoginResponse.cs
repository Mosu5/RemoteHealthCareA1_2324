using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace PatientApp.Commands
{
    internal class LoginResponse : ISessionCommand
    {
        /// <summary>
        /// Confirms if login was successful
        /// </summary>
        /// <param name="data"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        /// <exception cref="CommunicationException"></exception>
        public bool Execute(JsonObject data, ClientConn conn)
        {
            string status = data["status"]?.GetValue<string>();

            if (status == "error")
            {
                throw new CommunicationException("Combination of username and password was incorrect");
            }

            Console.WriteLine("Login successful");
            return true;
        }
    }
}
