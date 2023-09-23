using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace RHSandbox.Commands
{
    internal interface ISessionCommand
    {
        bool Execute(JsonObject data);
    }
}
