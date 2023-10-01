using System;
using PatientApp.DeviceConnection.Receiver;

namespace PatientApp.DeviceConnection
{
    public class DeviceManager
    {
        private Statistic currentStat = new Statistic();
        public EventHandler<Statistic> OnReceiveData;
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
            Console.WriteLine("Speed: {0} m/s", speed);

            if (currentStat.Speed == -1)
            {
                currentStat.Speed = speed;
                CheckStatComplete();
            }
        }

        private void OnReceiveDistance(object sender, int distance)
        {
            Console.WriteLine("Distance: {0} meters", distance);

            if (currentStat.Distance == -1)
            {
                currentStat.Distance = distance;
                CheckStatComplete();
            }
        }

        private void OnReceiveHeartRate(object sender, int heartRate)
        {
            Console.WriteLine("Heart rate: {0} bpm", heartRate);

            if (currentStat.HeartRate == -1)
            {
                currentStat.HeartRate = heartRate;
                CheckStatComplete();
            }
        }

        private void OnReceiveRrIntervals(object sender, int[] rrIntervals)
        {
            Console.WriteLine("R-R intervals: {0}", String.Join(", ", rrIntervals));

            if (currentStat.RrIntervals == new int[0])
            {
                currentStat.RrIntervals = rrIntervals;
                CheckStatComplete();
            }
        }

        private void CheckStatComplete()
        {
            if (currentStat.IsComplete())
            {
                OnReceiveData?.Invoke(this, currentStat);
                currentStat = new Statistic();
            }
        }
    }
}
