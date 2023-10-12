using System;
using System.Threading.Tasks;
using PatientApp.DeviceConnection.Receiver;
using Utilities.Logging;

namespace PatientApp.DeviceConnection
{
    public class DeviceManager
    {
        private static Statistic _currentStat = new Statistic(); // changed to static for hooking and unhooking delegates from events
        public static EventHandler<Statistic> OnReceiveData; // changed to static for hooking and unhooking delegates from events

        public static IReceiver Receiver;

        /// <summary>
        /// Connects to either the regular BLE devices or uses a built-in emulator to generate random bytes.
        /// If the EmulatedReceiver class is instantiated, upon calling it's Connect() method, it will create
        /// a new BLEReceiver class and will use the ReceivedTrainerMessage and ReceivedHRMMessage of that class,
        /// and give it byte arrays with random payloads. When using BLEReceiver, if no BLE connection can be made
        /// to any of the devices, it will automatically switch to the emulated environment. Both classes implement
        /// the IReceiver interface, as to ensure abstraction.
        /// </summary>
        public static async Task Initialize()
        {
            // Change to BLEReceiver in production
            Receiver = new EmulatedReceiver();

            // Subscribe to the receiver's events.
            Receiver.ReceivedSpeed += OnReceiveSpeed;
            Receiver.ReceivedDistance += OnReceiveDistance;
            Receiver.ReceivedHeartRate += OnReceiveHeartRate;
            Receiver.ReceivedRrIntervals += OnReceiveRrIntervals;

            _currentStat.OnStatisticComplete += OnStatisticComplete;

            await Receiver.ConnectToTrainer();
            await Receiver.ConnectToHrm();

            Logger.Log("Device connection initialized", LogType.GeneralInfo);
        }

        private static void OnReceiveSpeed(object sender, double speed)
        {
            Logger.Log($"Speed: {speed} m/s", LogType.DeviceInfo);

            _currentStat.SetSpeed(speed);
        }

        private static void OnReceiveDistance(object sender, int distance)
        {
            Logger.Log($"Distance: {distance} meters", LogType.DeviceInfo);

            _currentStat.SetDistance(distance);
        }

        private static void OnReceiveHeartRate(object sender, int heartRate)
        {
            Logger.Log($"Heart rate: {heartRate} bpm", LogType.DeviceInfo);

            _currentStat.SetHeartRate(heartRate);
        }

        private static void OnReceiveRrIntervals(object sender, int[] rrIntervals)
        {
            Logger.Log($"R-R intervals: {string.Join(", ", rrIntervals)}", LogType.DeviceInfo);

            _currentStat.SetRrIntervals(rrIntervals);
        }

        /// <summary>
        /// Once the Statistic has signaled that all its values are set (speed, distance, etc.),
        /// DeviceManager will invoke the OnReceiveData event.
        /// </summary>
        private static void OnStatisticComplete(object sender, Statistic statistic)
        {
            OnReceiveData?.Invoke(sender, statistic);
        }
    }
}
