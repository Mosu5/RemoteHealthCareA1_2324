using Microsoft.SqlServer.Server;
using ServerApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    /// <summary>
    /// State in when a client has connected with the server. 
    /// </summary>
    internal class LoginState : IState
    {
        private readonly ServerContext _context;

        public LoginState(ServerContext context)
        {
            _context = context;
        }

        public IState Handle(JsonObject packet)
        {
            //extracting the needed values from packet
            string username = JsonUtil.GetValueFromPacket(packet, "data", "username").ToString();
            string password = JsonUtil.GetValueFromPacket(packet, "data", "password").ToString();

            Console.WriteLine($"A client attempted to login with username '{username}' and password '{password}'");

            // If the list of users is not empty
            if (Server.Users.Any())
            {
                // Loop through user accounts to see which one's credentials matches the credentials sent by the client.
                foreach (UserAccount account in Server.Users)
                {
                    if (account.GetUserName() == username && account.GetPassword() == password)
                    {
                        // Bind a user account to this client
                        _context.SetNewUser(account);

                        // Send a login response to the client
                        _context.ResponseToPatient = ResponseClientData.GenerateResponse("login", null, "ok");

                        // Associate the current TCPclient connecting with the currently logged in User
                        _context.GetUserAccount().UserClient = _context.tcpClient;

                        // Set the state to idle
                        return new SessionIdle(_context);
                    }
                }
            }

            // Send an error to the client if the credentials are incorrect.
            _context.ResponseToPatient = ResponseClientData.GenerateResponse("login", null, "error");

            //Login Failed so it stays in LoginState
            return this;
        }

    }
}
