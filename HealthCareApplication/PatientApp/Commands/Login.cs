using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Communication;

namespace PatientApp.Commands
{
    internal class Login : ISessionCommand
    {
        public bool Execute(JsonObject data)
        {
            Console.WriteLine(data.ToString());
            DataTransfer.SendJson(data);
            return true;
            //throw new NotImplementedException();
        }
    }
}
