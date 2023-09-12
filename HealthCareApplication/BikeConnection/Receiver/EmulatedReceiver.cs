﻿using System;
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

        private readonly BLEReceiver _bleReceiver;

        public EmulatedReceiver()
        {
            _bleReceiver = new BLEReceiver();

            // Chain events, so that this class's events are fired when the bleReceiver's events are fired
            _bleReceiver.ReceivedSpeed += (sender, speed) =>
            {
                ReceivedSpeed?.Invoke(sender, speed);
            };
            _bleReceiver.ReceivedDistance += (sender, distance) =>
            {
                ReceivedDistance?.Invoke(sender, distance);
            };
        }

        public void Connect()
        {
            ConnectedToTrainer?.Invoke(this, EventArgs.Empty);
            ConnectedToHrm?.Invoke(this, EventArgs.Empty);

            var thread = new Thread(Emulate);
            thread.Start();
        }

        private void Emulate()
        {
            while (true)
            {
                // Trick bleReceiver into thinking it received data
                _bleReceiver.ReceivedTrainerMessage(this, RandomTrainerArgs());

                Thread.Sleep(1000);
            }
            
        }

        /// <summary>
        /// Fabricates a message the trainer would send and randomizes the speed and distance data.
        /// </summary>
        private BLESubscriptionValueChangedEventArgs RandomTrainerArgs()
        {
            byte[] speed = new byte[2];
            new Random().NextBytes(speed);
            byte distance = (byte)new Random().Next(256);
            byte[] message = new byte[]
                { 0xA4, 0x09, 0x4E, 0x05, 0x10, 0x19, 0x34, distance, speed[0], speed[1], 0xFF, 0x34, 0x00 };
            message[message.Length - 1] = _bleReceiver.CalculateChecksum(message);

            var trainerArgs = new BLESubscriptionValueChangedEventArgs
                {
                    ServiceName = "EmulatedTrainer",
                    Data = message
                };
            return trainerArgs;
        }

        /// <summary>
        /// Fabricates a message the heart rate monitor would send and randomizes the speed and distance data.
        /// </summary>
        private BLESubscriptionValueChangedEventArgs RandomHrmArgs()
        {
            throw new NotImplementedException();
        }
    }
}