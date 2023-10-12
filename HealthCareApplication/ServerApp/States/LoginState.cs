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
        //TODO current username and password are for 

        private ServerContext _context;
        public LoginState(ServerContext context)
        {
            this._context = context;
        }

        public IState Handle(JsonObject packet)
        {
            //extracting the needed values from packet
            string username = JsonUtil.GetValueFromPacket(packet, "data", "username").ToString();
            string password = JsonUtil.GetValueFromPacket(packet, "data", "password").ToString();

            Console.WriteLine("Login recieved data: " + username + "    " + password);


            if (Server.users.Any())
            {

                foreach (UserAccount account in Server.users)
                {
                    if (account.GetUserName() == username && account.GetPassword() == password)
                    {
                        Console.WriteLine("We are actually logging in!");
                        //context.ResponseToClient = ApproveLogin();
                        _context.ResponseToClient = ResponseDataForClient.GenerateResponse("login",null,"ok");
                        Console.WriteLine(_context.ResponseToClient.ToString());
                        return new SessionIdle(_context);
                    }
                    else
                    {
                        Console.WriteLine("Currently going into account creation state.");
                        return new CreateAccountState(_context);
                    }
                }
            }
            else
            {
                _context.ResponseToClient = ResponseDataForClient.GenerateResponse("login",null,"ok");//Approvelogin();
                return new CreateAccountState(_context);
            }
            _context.ResponseToClient = ResponseDataForClient.GenerateResponse("login", null, "error");
            //Login Failed so it stays in LoginState
            return this;
        }

        /// <summary>
        /// Method to send status of login
        /// </summary>
        /// <returns></returns>
        private JsonObject RefuseLogin()
        {
            return new JsonObject
            {
                {"command", "login" },
                {"data", new JsonObject
                    {
                        {"status", "error"}
                    }
                }
            };
        }

        private JsonObject CreateNewAccountMSG()
        {
            return new JsonObject
            {
                {"command", "login" },
                {"data", new JsonObject
                    {
                        {"status", "Creating new account with current information!"}
                    }
                }
            };
        }

        /// <summary>
        /// Mehtod to send status login
        /// </summary>
        /// <returns></returns>
        private JsonObject ApproveLogin()
        {
            return new JsonObject
            {
                {"command", "login" },
                {"data", new JsonObject
                    {
                        {"status", "ok"}
                    }
                }
            };
        }
    }
}
