using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;
using Utilities.Logging;

namespace PatientApp.DeviceConnection.Receiver
{
    public class BLEReceiver : IReceiver
    {
        public event EventHandler<bool> ConnectedToTrainer;
        public event EventHandler<bool> ConnectedToHrm;

        public event EventHandler<double> ReceivedSpeed;
        public event EventHandler<int> ReceivedDistance;
        public event EventHandler<int> ReceivedHeartRate;
        public event EventHandler<int[]> ReceivedRrIntervals;

        private IReceiver _emulatedReceiver = null;
        private readonly int _maxConnectionAttempts = 5;
        private readonly BLE _bleTrainer = new BLE();
        private bool _trainerConnected = false;

        /// <summary>
        /// Attempts connection to both the trainer. Upon successful connection, events are fired to signal to other classes.
        /// Otherwise, after some amount of connection attempts, it will automatically use the simulated environment.
        /// </summary>
        public async Task ConnectToTrainer()
        {
            Thread.Sleep(1000);

            bool connected = await PairDevice(
                _bleTrainer,
                "Tacx Flux 00438",
                "6e40fec1-b5a3-f393-e0a9-e50e24dcca9e",
                "6e40fec2-b5a3-f393-e0a9-e50e24dcca9e"
            );
            if (!connected)
            {
                Console.WriteLine("Could not connect to trainer, switching to emulated trainer environment.");
                if (_emulatedReceiver == null) _emulatedReceiver = new EmulatedReceiver(this);

                // Subscribe to trainer events from the EmulatedReceiver class, so that whenever the EmulatedReceiver's
                // events fire, this class's corresponding events will fire as well.
                _emulatedReceiver.ConnectedToTrainer += (sender, _) =>
                {
                    ConnectedToTrainer?.Invoke(sender, false);
                };
                _emulatedReceiver.ReceivedSpeed += (sender, speed) => ReceivedSpeed?.Invoke(sender, speed);
                _emulatedReceiver.ReceivedDistance += (sender, distance) => ReceivedDistance?.Invoke(sender, distance);

                await _emulatedReceiver.ConnectToTrainer();
                _trainerConnected = true;
                return;
            }

            // Subscribing to the trainer's messages
            _bleTrainer.SubscriptionValueChanged += ReceivedTrainerMessage;

            // Signaling successful connection
            _trainerConnected = true;
            ConnectedToTrainer?.Invoke(this, true);
        }

        /// <summary>
        /// Attempts connection to both the heart rate monitor. Upon successful connection, events are fired to signal to other classes.
        /// Otherwise, after some amount of connection attempts, it will automatically use the simulated environment.
        /// </summary>
        public async Task ConnectToHrm()
        {
            BLE bleHrm = new BLE();

            Thread.Sleep(1000);

            bool connected = await PairDevice(
                bleHrm,
                "Decathlon Dual HR",
                "HeartRate",
                "HeartRateMeasurement"
            );
            if (!connected)
            {
                Logger.Log("Could not connect to heart rate monitor, switching to emulated heart rate monitor environment.", LogType.DeviceInfo);
                if (_emulatedReceiver == null) _emulatedReceiver = new EmulatedReceiver(this);

                // Subscribe to heart rate monitor events from the EmulatedReceiver class, so that whenever the EmulatedReceiver's
                // events fire, this class's corresponding events will fire as well.
                _emulatedReceiver.ConnectedToHrm += (sender, _) => ConnectedToHrm?.Invoke(sender, false);
                _emulatedReceiver.ReceivedHeartRate += (sender, heartRate) => ReceivedHeartRate?.Invoke(sender, heartRate);
                _emulatedReceiver.ReceivedRrIntervals += (sender, rrInterval) => ReceivedRrIntervals?.Invoke(sender, rrInterval);

                await _emulatedReceiver.ConnectToHrm();
                return;
            }

            // Subscribing to the trainer's messages
            bleHrm.SubscriptionValueChanged += ReceivedHrmMessage;

            // Signaling successful connection
            ConnectedToHrm?.Invoke(this, true);
        }

        /// <summary>
        /// Attempts to pair a device using the BLE library. Stops after exceeding the maximum amount of connection attempts.
        /// </summary>
        /// <returns>Wether the device has paired successfully</returns>
        private async Task<bool> PairDevice(BLE bleDevice, string deviceName, string serviceName, string characteristicName)
        {
            int errorCode = 14000;

            Thread t = new Thread(async () =>
            {
                errorCode = await bleDevice.OpenDevice("Tacx Flux 00438");
                if (errorCode != 0) return;
                errorCode = await bleDevice.SetService(serviceName);
                if (errorCode != 0) return;
                errorCode = await bleDevice.SubscribeToCharacteristic(characteristicName);
                if (errorCode != 0) return;
            });
            t.Start();
            t.Join();

            return true;
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
                Logger.Log("Error: invalid message length from trainer message.", LogType.DeviceInfo);
                return;
            }

            // Checks if checksums are equal to each other
            if (CalculateChecksum(message) != message[message.Length - 1])
            {
                Logger.Log("Error: invalid checksum from trainer message.", LogType.DeviceInfo);
                return;
            }

            double speed = DecodeTrainerSpeed(message);
            int distance = DecodeTrainerDistance(message);

            // TODO fix floating point precision
            if (speed != -1d) ReceivedSpeed?.Invoke(this, speed);
            else Logger.Log("Error: invalid speed value from trainer message.", LogType.DeviceInfo);

            if (distance != -1) ReceivedDistance?.Invoke(this, distance);
            else Logger.Log("Error: invalid distance value from trainer message.", LogType.DeviceInfo);
        }

        /// <summary>
        /// Extracts the useful data from the heart rate monitor's messages and fires events containing this data.
        /// This method may also be called by the EmulatedReceiver class.
        /// </summary>
        public void ReceivedHrmMessage(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            byte[] message = e.Data;

            if (message.Length < 3)
            {
                Logger.Log("Error: invalid message length from HRM message.", LogType.DeviceInfo);
                return;
            }

            // Byte containing flags for interpreting the message
            byte hrmFlags = message[0];

            // Wether the heart rate value was sent using uint8 format (true) or uint16 format (false)
            bool isUint8 = (hrmFlags & 0b1) == 0b0;

            if (!isUint8)
            {
                Logger.Log("Warning: skipping message, cannot read heart rate in uint16 format.", LogType.DeviceInfo);
                return;
            }
            
            int heartRate = DecodeHeartRate(message);
            int[] rrIntervals = DecodeRrIntervals(message);

            if (heartRate != -1) ReceivedHeartRate?.Invoke(this, heartRate);
            else Logger.Log("Error: invalid heart rate value from HRM message.", LogType.DeviceInfo);

            if (rrIntervals != new int[0]) ReceivedRrIntervals?.Invoke(this, rrIntervals);
            else Logger.Log("Error: invalid R-R interval value from HRM message.", LogType.DeviceInfo);
        }

        /// <summary>
        /// Send the trainer a message to change its resistance. The trainer does not send
        /// anything back, so we'll have to trust the message has arrived successfully.
        /// TODO test
        /// TODO maybe send message multiple times, in case the message does not fully reach
        /// the trainer.
        /// </summary>
        public async Task SetResistance(int resistance)
        {
            if (!_trainerConnected)
            {
                Logger.Log("Error: trainer is not yet connected. Call the connect method first.", LogType.DeviceInfo);
                return;
            }

            if (resistance < 0 || resistance > 100)
            {
                Logger.Log("Error: resistance must be between 0 and 100 percent.", LogType.DeviceInfo);
                return;
            }

            // Create the message
            byte[] data = new byte[] { 0xA4, 0x09, 0x4E, 0x05, 0x30, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, (byte)(resistance * 2), 0 };

            // Change the last byte to be the checksum
            data[12] = CalculateChecksum(data);

            // Attempt to send the message and check if there are errors
            int errorCode = await _bleTrainer.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", data);
            if (errorCode != 0) Logger.Log("Error: could not send resistance message.", LogType.DeviceInfo);
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
            
            // Combine the most significant and least significant bytes to one value
            byte lsb = message[8];
            byte msb = message[9];
            int mergedValue = (msb << 8) | lsb;

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
            byte hrmFlags = message[0];
            // Whether the HRM has skin contact
            bool hasSkinContact = (hrmFlags & 0b10) == 0b10;
            byte heartRate = message[1];

            return hasSkinContact ? heartRate : -1;
        }

        /// <summary>
        /// Extracts the heart rate monitor's R-R intervals from the message
        /// </summary>
        private int[] DecodeRrIntervals(byte[] message)
        {
            byte hrmFlags = message[0];

            // Whether R-R intervals are present in the message
            bool rrIntervalsPresent = (hrmFlags & 0b10000) == 0b10000;

            // Wether the energy expension byte is present in the message.
            // The presence of this changes the positions of the R-R intervals in the message.
            bool energyExpensionPresent = (hrmFlags & 0b1000) == 0b10;

            // Skip the first 3 elements if the energy expension byte is present, otherwise skip 2.
            byte[] rrIntervals = message.Skip(energyExpensionPresent ? 3 : 2).ToArray();

            if (rrIntervalsPresent)
                // Returns byte array as int array
                return rrIntervals.Select(b => (int)b).ToArray();

            return new int[0];
        }
    }
}
