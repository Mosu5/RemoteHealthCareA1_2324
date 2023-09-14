using System;
using System.Threading;
using Avans.TI.BLE;

namespace BikeConnection.Receiver
{
    public class EmulatedReceiver : IReceiver
    {
        public event EventHandler<EventArgs> ConnectedToTrainer;
        public event EventHandler<EventArgs> DisconnectedFromTrainer;
        public event EventHandler<EventArgs> ConnectedToHrm;
        public event EventHandler<EventArgs> DisconnectedFromHrm;

        public event EventHandler<double> ReceivedSpeed;
        public event EventHandler<int> ReceivedHeartRate;
        public event EventHandler<int> ReceivedDistance;
        public event EventHandler<int[]> ReceivedRrIntervals;

        private readonly BLEReceiver _bleReceiver;
        private readonly EmulatedTrainer _emulatedTrainer;

        /// <summary>
        /// Constructor not to be called by the BLEReceiver class. If it calls this method,
        /// it might lead to infinite loops of event calls. Any other class calling this
        /// constructor is fine.
        /// </summary>
        public EmulatedReceiver()
        {
            _bleReceiver = new BLEReceiver();
            _emulatedTrainer = new EmulatedTrainer();

            // Chain events, so that this class's events are fired when the bleReceiver's events are fired
            _bleReceiver.ReceivedSpeed += (sender, speed) => ReceivedSpeed?.Invoke(sender, speed);
            _bleReceiver.ReceivedDistance += (sender, distance) => ReceivedDistance?.Invoke(sender, distance);
            _bleReceiver.ReceivedHeartRate += (sender, heartRate) => ReceivedHeartRate?.Invoke(sender, heartRate);
            _bleReceiver.ReceivedRrIntervals += (sender, rrInterval) => ReceivedRrIntervals?.Invoke(sender, rrInterval);
        }

        /// <summary>
        /// Constructor to be called by any class but the BLEReceiver class.
        /// </summary>
        public EmulatedReceiver(BLEReceiver caller)
        {
            _bleReceiver = caller;
            _emulatedTrainer = new EmulatedTrainer();
        }

        /// <summary>
        /// Emulates successful connection to the trainer and sends emulated messages in a new thread.
        /// </summary>
        public void ConnectToTrainer()
        {
            // Signaling successful connection
            ConnectedToTrainer?.Invoke(this, EventArgs.Empty);

            // Emulate the trainer on a new thread
            var thread = new Thread(() =>
            {
                while (true)
                {
                    // Trick bleReceiver into thinking it received data
                    _bleReceiver.ReceivedTrainerMessage(this, RandomTrainerArgs());
                    Thread.Sleep(1000);
                }
            });
            thread.Start();
        }

        /// <summary>
        /// Emulates successful connection to the heart rate monitor and sends emulated messages in a new thread.
        /// </summary>
        public void ConnectToHrm()
        {
            // Signaling successful connection
            ConnectedToHrm?.Invoke(this, EventArgs.Empty);

            // Emulate the trainer on a new thread
            var thread = new Thread(() =>
            {
                while (true)
                {
                    // Trick bleReceiver into thinking it received data
                    _bleReceiver.ReceivedHrmMessage(this, RandomHrmArgs());
                    Thread.Sleep(1000);
                }
            });
            thread.Start();
        }

        /// <summary>
        /// Fabricates a message the trainer would send and randomizes the speed and distance data.
        /// </summary>
        private BLESubscriptionValueChangedEventArgs RandomTrainerArgs()
        {
            // // Create message
            // byte[] speed = new byte[2];
            // new Random().NextBytes(speed);
            // byte distance = (byte)new Random().Next(256);
            // byte[] message = new byte[]
            //     { 0xA4, 0x09, 0x4E, 0x05, 0x10, 0x19, 0x34, distance, speed[0], speed[1], 0xFF, 0x34, 0x00 };
            //
            // // Append checksum to message
            // message[message.Length - 1] = _bleReceiver.CalculateChecksum(message);

            var data = _emulatedTrainer.GenerateBikeData();

            return new BLESubscriptionValueChangedEventArgs
            {
                ServiceName = "EmulatedTrainerService",
                Data = data
            };
        }

        /// <summary>
        /// Fabricates a message the heart rate monitor would send and randomizes the speed and distance data.
        /// </summary>
        private BLESubscriptionValueChangedEventArgs RandomHrmArgs()
        {
            // Create message
            byte heartRate = (byte)new Random().Next(256);
            byte[] rrIntervals = new byte[new Random().Next(1, 10)];
            new Random().NextBytes(rrIntervals);
            byte[] messageBegin = new byte[] { 0x16, heartRate };
            byte[] message = new byte[messageBegin.Length + rrIntervals.Length];

            // Combine messageBegin and rrIntervals to one single byte array
            Buffer.BlockCopy(messageBegin, 0, message, 0, messageBegin.Length);
            Buffer.BlockCopy(rrIntervals, 0, message, messageBegin.Length, rrIntervals.Length);

            return new BLESubscriptionValueChangedEventArgs
            {
                ServiceName = "EmulatedHrmService",
                Data = message
            };
        }
    }
}