using System;
using System.Threading.Tasks;

namespace PatientApp.DeviceConnection.Receiver
{
    public interface IReceiver
    {
        event EventHandler<bool> ConnectedToTrainer;
        event EventHandler<bool> ConnectedToHrm;

        event EventHandler<double> ReceivedSpeed;
        event EventHandler<int> ReceivedDistance;
        event EventHandler<int> ReceivedHeartRate;
        event EventHandler<int[]> ReceivedRrIntervals;

        Task ConnectToTrainer();
        Task ConnectToHrm();
        Task SetResistance(int percentage);
    }
}
