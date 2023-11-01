using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientApp.PatientLogic.Commands
{
    internal class Logout : IPatientCommand
    {
        public Task<bool> Execute()
        {
            throw new NotImplementedException();
        }
    }
}
