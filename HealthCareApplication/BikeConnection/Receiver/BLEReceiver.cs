using Avans.TI.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TrainerDataTesting.Receiver
{
    public class BLEReceiver : IReceiver
    {
        public event EventHandler<EventArgs> ConnectedToTrainer;
        public event EventHandler<EventArgs> DisconnectedFromTrainer;
        public event EventHandler<EventArgs> ConnectedToHRM;
        public event EventHandler<EventArgs> DisconnectedFromHRM;

        public event EventHandler<double> ReceivedSpeed;
        public event EventHandler<int> ReceivedDistance;
        public event EventHandler<int> ReceivedHeartRate;

        /// <summary>
        /// Attempts connection to both the trainer and the heart rate monitor. Upon successful connection,
        /// events are fired to signal to other classes.
        /// </summary>
        public async void Connect()
        {
            BLE bleTrainer = new BLE();
            BLE bleHRM = new BLE();

            Thread.Sleep(1000);

            // Pairing the trainer
            int errorCode = await bleTrainer.OpenDevice("Tacx Flux 01140");
            if (errorCode != 0)
            {
                Console.WriteLine("Failed to connect to trainer with error code " + errorCode);
                return;
            }

            // Setting one of the trainer's services
            errorCode = await bleTrainer.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
            if (errorCode != 0)
            {
                Console.WriteLine("Failed to connect to trainer with error code " + errorCode);
                return;
            }

            // Subscribing to one of the trainer's characteristics
            errorCode = await bleTrainer.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");
            if (errorCode != 0)
            {
                Console.WriteLine("Failed to connect to trainer with error code " + errorCode);
                return;
            }

            // Subscribing to the trainer's messages
            bleTrainer.SubscriptionValueChanged += ReceivedTrainerMessage;

            // Signaling successful connection
            ConnectedToTrainer?.Invoke(this, EventArgs.Empty);

            // Pairing the heart rate monitor
            errorCode = await bleHRM.OpenDevice("Decathlon Dual HR");
            if (errorCode != 0)
            {
                Console.WriteLine("Failed to connect to heart rate monitor with error code " + errorCode);
                return;
            }

            // Setting one of the heart rate monitor's services
            errorCode = await bleHRM.SetService("HeartRate");
            if (errorCode != 0)
            {
                Console.WriteLine("Failed to connect to heart rate monitor with error code " + errorCode);
                return;
            }

            // Subscribing to one of the heart rate monitor's characteristics
            errorCode = await bleHRM.SubscribeToCharacteristic("HeartRateMeasurement");
            if (errorCode != 0)
            {
                Console.WriteLine("Failed to connect to heart rate monitor with error code " + errorCode);
                return;
            }

            // Subscribing to the heart rate monitor's messages
            bleHRM.SubscriptionValueChanged += ReceivedHRMMessage;

            // Signal successful connection
            ConnectedToHRM?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Extracts the useful data from the trainer's messages and fires events containing this data.
        /// This method may also be called by the EmulatedReceiver class.
        /// </summary>
        public void ReceivedTrainerMessage(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            byte[] message = e.Data;

            // Checks if checksums are equal to each other
            if (CalculateChecksum(message) != message[message.Length - 1])
            {
                Console.WriteLine("Error: message from trainer had an invalid checksum.");
                return;
            }

            double speed = DecodeTrainerSpeed(message);
            int distance = DecodeTrainerDistance(message);

            if (speed == -1d)
            {
                Console.WriteLine("Error: invalid speed value received.");
            }
            if (distance == -1)
            {
                Console.WriteLine("Error: invalid distance value received.");
            }
            if (speed == -1d || distance == -1) return;

            ReceivedSpeed?.Invoke(this, speed);
            ReceivedDistance?.Invoke(this, distance);
        }

        /// <summary>
        /// Extracts the useful data from the heart rate monitor's messages and fires events containing this data.
        /// This method may also be called by the EmulatedReceiver class.
        /// </summary>
        public void ReceivedHRMMessage(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calculates the checksum of the message by XORing all bytes of the message, except for the checksum byte.
        /// </summary>
        public byte CalculateChecksum(byte[] message)
        {
            byte checksum = 0;
            for (int i = 0; i < message.Length - 1; i++)
            {
                checksum ^= message[i];
            }
            return checksum;
        }

        /// <summary>
        /// Extracts the trainer's speed from the message
        /// </summary>
        private double DecodeTrainerSpeed(byte[] message)
        {
            byte dataPage = message[4];

            if (dataPage != 16) return -1d; 
            
            byte msb = message[8];
            byte lsb = message[9];
            int mergedValue = (lsb << 8) | msb;

            return mergedValue * 0.001;
        }

        /// <summary>
        /// Extracts the trainer's distance from the message
        /// </summary>
        private int DecodeTrainerDistance(byte[] message)
        {
            byte dataPage = message[4];

            if (dataPage != 16) return -1;

            return message[7];
        }

        /// <summary>
        /// Extracts the heart rate monitor's heart rate from the message
        /// </summary>
        private int DecodeHeartRate(byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
