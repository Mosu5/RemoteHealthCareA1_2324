using System.Text.Json.Nodes;

namespace ServerApp.States
{
    internal class SessionStoppedState : IState
    {
        private readonly ServerContext _context;

        public SessionStoppedState(ServerContext serverContext)
        {
            _context = serverContext;
        }

        public IState Handle(JsonObject packet)
        {
            // Save data to file
            // Data will be saved so client/doctor can recieve a stats summary later
            _context.SaveUserData();
            _context.GetUserAccount().HasActiveSession = false;
            _context.IsSessionActive = false;

            return new SessionIdle(_context);
        }

    }
}
