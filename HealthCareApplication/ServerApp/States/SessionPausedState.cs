using ServerApp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.States
{
    internal class SessionPausedState : IState
    {
        private ServerContext _context;

        public SessionPausedState(ServerContext context)
        {
            this._context = context;
        }

        public IState Handle(JsonObject packet)
        {
            Console.WriteLine(packet.ToString());
            //Getting needed values from the packet
            string command = JsonUtil.GetValueFromPacket(packet, "command").ToString();

            //checking if the command equals session/pause
            if (command == "session/resume")
            {
                _context.GetUserAccount().isPaused = false;
                this._context.GetUserAccount().hasActiveSession = true;
                this._context.ResponseToClient = ResponseClientData.GenerateResponse("session/resume", null, "ok");
                _context.isSessionActive = true;
                return new SessionActiveState(_context);
            }
            return this;
        }



    }
}
