using System;
using System.Threading.Tasks;

namespace PatientApp.DeviceConnection.Receiver
{
    public interface IReceiver
    {
        event EventHandler<EventArgs> ConnectedToTrainer;
        event EventHandler<EventArgs> DisconnectedFromTrainer;
        event EventHandler<EventArgs> ConnectedToHrm;
        event EventHandler<EventArgs> DisconnectedFromHrm;

        event EventHandler<double> ReceivedSpeed;
        event EventHandler<int> ReceivedDistance;
        event EventHandler<int> ReceivedHeartRate;
        event EventHandler<int[]> ReceivedRrIntervals;

        void ConnectToTrainer();
        void ConnectToHrm();
        Task SetResistance(int percentage);
    }
}
