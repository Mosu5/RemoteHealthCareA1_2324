using System.Threading.Tasks;
using Utilities.Communication;

namespace DoctorApp.Commands
{
    internal interface IDoctorCommand
    {
        Task<bool> Execute();
    }
}
