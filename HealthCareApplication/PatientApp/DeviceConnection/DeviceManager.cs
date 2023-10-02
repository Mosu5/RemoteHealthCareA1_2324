using System;
using PatientApp.DeviceConnection.Receiver;
using Utilities.Logging;

namespace PatientApp.DeviceConnection
{
    public class DeviceManager
    {
        private static Statistic currentStat = new Statistic(); // changed to static
        public static EventHandler<Statistic> OnReceiveData; // changed to static
       
        public readonly IReceiver Receiver;

        /// <summary>
        /// Connects to either the regular BLE devices or uses a built-in emulator to generate random bytes.
        /// If the EmulatedReceiver class is instantiated, upon calling it's Connect() method, it will create
        /// a new BLEReceiver class and will use the ReceivedTrainerMessage and ReceivedHRMMessage of that class,
        /// and give it byte arrays with random payloads. When using BLEReceiver, if no BLE connection can be made
        /// to any of the devices, it will automatically switch to the emulated environment. Both classes implement
        /// the IReceiver interface, as to ensure abstraction.
        /// </summary>
        public DeviceManager()
        {
            Receiver = new BLEReceiver();

            // Subscribe to the receiver's events.
            Receiver.ReceivedSpeed += OnReceiveSpeed;
            Receiver.ReceivedDistance += OnReceiveDistance;
            Receiver.ReceivedHeartRate += OnReceiveHeartRate;
            Receiver.ReceivedRrIntervals += OnReceiveRrIntervals;

            Receiver.ConnectToTrainer();
            Receiver.ConnectToHrm();
        }

        private void OnReceiveSpeed(object sender, double speed)
        {
            Logger.Log($"Speed: {speed} m/s", LogType.DeviceInfo);

            if (currentStat.Speed == -1)
            {
                currentStat.Speed = speed;
                CheckStatComplete();
            }
        }

        private void OnReceiveDistance(object sender, int distance)
        {
            Logger.Log($"Distance: {distance} meters", LogType.DeviceInfo);

            if (currentStat.Distance == -1)
            {
                currentStat.Distance = distance;
                CheckStatComplete();
            }
        }

        private void OnReceiveHeartRate(object sender, int heartRate)
        {
            Logger.Log($"Heart rate: {heartRate} bpm", LogType.DeviceInfo);

            if (currentStat.HeartRate == -1)
            {
                currentStat.HeartRate = heartRate;
                CheckStatComplete();
            }
        }

        private void OnReceiveRrIntervals(object sender, int[] rrIntervals)
        {
            Logger.Log($"R-R intervals: {string.Join(", ", rrIntervals)}", LogType.DeviceInfo);

            if (currentStat.RrIntervals == new int[0])
            {
                currentStat.RrIntervals = rrIntervals;
                CheckStatComplete();
            }
        }

        /// <summary>
        /// Check if stats have been filled with data and calls eventhandler to pass data 
        /// </summary>
        private static void CheckStatComplete() // changed to static
        {
            if (currentStat.IsComplete())
            {
                OnReceiveData?.Invoke(typeof(DeviceManager), currentStat);
                currentStat = new Statistic();
            }
        }
    }
}
