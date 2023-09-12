using System;

namespace TrainerDataTesting.Receiver
{
    public interface IReceiver
    {
        event EventHandler<EventArgs> ConnectedToTrainer;
        event EventHandler<EventArgs> DisconnectedFromTrainer;
        event EventHandler<EventArgs> ConnectedToHRM;
        event EventHandler<EventArgs> DisconnectedFromHRM;

        event EventHandler<double> ReceivedSpeed;
        event EventHandler<int> ReceivedDistance;
        event EventHandler<int> ReceivedHeartRate;

        void Connect();
    }
}
