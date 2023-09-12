using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Avans.TI.BLE;

namespace TrainerDataTesting.Receiver
{
    public class SimulatedReceiver : IReceiver
    {
        public event EventHandler<EventArgs> ConnectedToTrainer;
        public event EventHandler<EventArgs> DisconnectedFromTrainer;
        public event EventHandler<EventArgs> ConnectedToHRM;
        public event EventHandler<EventArgs> DisconnectedFromHRM;

        public event EventHandler<double> ReceivedSpeed;
        public event EventHandler<int> ReceivedHeartRate;
        public event EventHandler<int> ReceivedDistance;

        private BLEReceiver bleReceiver;

        public SimulatedReceiver()
        {
            bleReceiver = new BLEReceiver();

            // Chain events, so that this class's events are fired when the bleReceiver's events are fired
            bleReceiver.ReceivedSpeed += delegate(object sender, double speed) { ReceivedSpeed?.Invoke(sender, speed); };
            bleReceiver.ReceivedDistance += delegate(object sender, int distance) { ReceivedDistance?.Invoke(sender, distance); };
        }

        public void Connect()
        {
            ConnectedToTrainer?.Invoke(this, EventArgs.Empty);
            ConnectedToHRM?.Invoke(this, EventArgs.Empty);

            Thread thread = new Thread(Simulate);
            thread.Start();
        }

        private void Simulate()
        {
            while (true)
            {
                // Trick bleReceiver into thinking it received data
                bleReceiver.ReceivedTrainerMessage(this, randomTrainerArgs());

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Fabricates a message the trainer would send and randomizes the speed and distance data.
        /// </summary>
        private BLESubscriptionValueChangedEventArgs randomTrainerArgs()
        {
            byte[] speed = new byte[2];
            new Random().NextBytes(speed);
            byte distance = (byte)new Random().Next(256);
            byte[] message = new byte[] { 0xA4, 0x09, 0x4E, 0x05, 0x10, 0x19, 0x34, distance, speed[0], speed[1], 0xFF, 0x34, 0x00 };
            message[message.Length - 1] = bleReceiver.CalculateChecksum(message);

            BLESubscriptionValueChangedEventArgs trainerArgs = new BLESubscriptionValueChangedEventArgs();
            trainerArgs.ServiceName = "SimulatedTrainer";
            trainerArgs.Data = message;
            return trainerArgs;
        }

        /// <summary>
        /// Fabricates a message the heart rate monitor would send and randomizes the speed and distance data.
        /// </summary>
        private BLESubscriptionValueChangedEventArgs randomHRMArgs()
        {
            throw new NotImplementedException();
        }
    }
}
