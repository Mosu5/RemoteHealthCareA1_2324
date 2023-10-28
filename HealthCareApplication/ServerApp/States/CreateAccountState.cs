using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    // TODO can this class be removed?
    internal class CreateAccountState : IState
    {
        private ServerContext _context;

        public CreateAccountState(ServerContext context)
        {
            this._context = context;
        }

        public IState Handle(JsonObject packet)
        {

            Console.WriteLine(packet.ToJsonString());
            //extracting username and password
            string username = JsonUtil.GetValueFromPacket(packet, "data", "username").ToString();
            string password = JsonUtil.GetValueFromPacket(packet, "data", "password").ToString();

            //extracting the needed part of the JsonObject
            Console.WriteLine("Create account recieved data: " + username + "   " + password);

            //checks if the packet is contains the correct data

            foreach (UserAccount account in Server.Users)
            {
                if (account.GetUserName() == username)
                {
                    //context.ResponseToClient = AccCreationFailed("Username is already being used by an account. Please Login or use another username to create an account");
                    _context.ResponseToPatient = ResponseDataForClient.GenerateResponse("create account", null, "error");
                    return new LoginState(_context);
                }
                else
                {
                    //context.ResponseToClient = AccSuccesfullCreated();
                    _context.ResponseToPatient = ResponseDataForClient.GenerateResponse("create account", null, "ok");
                    Server.Users.Add(new UserAccount(username, password));
                    return new SessionIdle(_context);
                }
            }
            //Account Creation Failed so it stays in CreateAccountState
            //context.ResponseToClient = AccCreationFailed("Account creation Failed because");
            _context.ResponseToPatient = ResponseDataForClient.GenerateResponse("create account", null, "error");
            return this;
        }
    }
}
