using System.Threading.Tasks;

namespace PatientApp.PatientLogic.Commands
{
    public interface IPatientCommand
    {
        Task<bool> Execute();
    }
}
