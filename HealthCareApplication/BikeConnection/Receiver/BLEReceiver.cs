using System;
using System.Threading;
using Avans.TI.BLE;

namespace BikeConnection.Receiver
{
    public class BLEReceiver : IReceiver
    {
        public event EventHandler<EventArgs> ConnectedToTrainer;
        public event EventHandler<EventArgs> DisconnectedFromTrainer;
        public event EventHandler<EventArgs> ConnectedToHrm;
        public event EventHandler<EventArgs> DisconnectedFromHrm;

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
            BLE bleHrm = new BLE();

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
            errorCode = await bleHrm.OpenDevice("Decathlon Dual HR");
            if (errorCode != 0)
            {
                Console.WriteLine("Failed to connect to heart rate monitor with error code " + errorCode);
                return;
            }

            // Setting one of the heart rate monitor's services
            errorCode = await bleHrm.SetService("HeartRate");
            if (errorCode != 0)
            {
                Console.WriteLine("Failed to connect to heart rate monitor with error code " + errorCode);
                return;
            }

            // Subscribing to one of the heart rate monitor's characteristics
            errorCode = await bleHrm.SubscribeToCharacteristic("HeartRateMeasurement");
            if (errorCode != 0)
            {
                Console.WriteLine("Failed to connect to heart rate monitor with error code " + errorCode);
                return;
            }

            // Subscribing to the heart rate monitor's messages
            bleHrm.SubscriptionValueChanged += ReceivedHrmMessage;

            // Signal successful connection
            ConnectedToHrm?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Extracts the useful data from the trainer's messages and fires events containing this data.
        /// This method may also be called by the EmulatedReceiver class.
        /// </summary>
        public void ReceivedTrainerMessage(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            byte[] message = e.Data;

            if (message.Length != 13)
            {
                Console.WriteLine("Error: invalid trainer message received.");
                return;
            }

            // Checks if checksums are equal to each other
            if (CalculateChecksum(message) != message[message.Length - 1])
            {
                Console.WriteLine("Error: message from trainer had an invalid checksum.");
                return;
            }

            double speed = DecodeTrainerSpeed(message);
            int distance = DecodeTrainerDistance(message);

            if (speed == -1d) // TODO fix floating point precision
            {
                Console.WriteLine("Error: invalid speed value received.");
            }
            if (distance == -1)
            {
                Console.WriteLine("Error: invalid distance value received.");
            }
            if (speed == -1d || distance == -1) return; // TODO fix floating point precision

            ReceivedSpeed?.Invoke(this, speed);
            ReceivedDistance?.Invoke(this, distance);
        }

        /// <summary>
        /// Extracts the useful data from the heart rate monitor's messages and fires events containing this data.
        /// This method may also be called by the EmulatedReceiver class.
        /// </summary>
        public void ReceivedHrmMessage(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            byte[] message = e.Data;

            if (message.Length < 4)
            {
                Console.WriteLine("Error: invalid trainer message received.");
                return;
            }

            byte hrmFlags = message[0];
            bool isUint8 = (hrmFlags & 0b1) == 0;
            bool hasSkinContact = (hrmFlags & 0b10) == 1;
            bool rrIntervalsPresent = (hrmFlags & 0b00010000) == 1;

            byte heartRate = message[1];
            byte[] rrInterval = new byte[] { message[2], message[3] };

            
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
