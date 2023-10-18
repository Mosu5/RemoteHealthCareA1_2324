using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class SessionStoppedState : IState
    {
        private ServerContext _context;

        public SessionStoppedState(ServerContext serverContext) 
        {
        this._context = serverContext;
        }


        public IState Handle(JsonObject packet)
        {
            // Save data to file
            // Data will be saved so client/doctor can recieve a stats summary later
            Console.WriteLine("State = Session/stop");
            this._context.SaveUserData();
            _context.GetUserAccount().hasActiveSession = false;
            _context.isSessionActive = false;
            //ResponseClientData.GenerateResponse("session/stop", null, "ok");
            return new SessionIdle(_context);
        }

    }
}
