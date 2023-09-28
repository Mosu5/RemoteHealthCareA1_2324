using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp.Commands
{
    internal interface ICommand
    {
        /// <summary>
        /// Execution method to be filled with command logic for server.
        /// </summary>
        /// <param name="data"></param>
        void Execute(JsonObject data);

    }
}
