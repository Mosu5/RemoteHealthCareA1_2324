using System;
using PatientApp.BikeConnection.Receiver;

namespace PatientApp.BikeConnection
{
    public class Client
    {
        /// <summary>
        /// Connects to either the regular BLE devices or uses a built-in emulator to generate random bytes.
        /// If the EmulatedReceiver class is instantiated, upon calling it's Connect() method, it will create
        /// a new BLEReceiver class and will use the ReceivedTrainerMessage and ReceivedHRMMessage of that class,
        /// and give it byte arrays with random payloads. When using BLEReceiver, if no BLE connection can be made
        /// to any of the devices, it will automatically switch to the emulated environment. Both classes implement
        /// the IReceiver interface, as to ensure abstraction.
        /// </summary>
        public Client()
        {
            IReceiver receiver = new BLEReceiver();

            // Subscribe to the receiver's events.
            receiver.ReceivedSpeed += OnReceiveSpeed;
            receiver.ReceivedDistance += OnReceiveDistance;
            receiver.ReceivedHeartRate += OnReceiveHeartRate;
            receiver.ReceivedRrIntervals += OnReceiveRrIntervals;

            receiver.ConnectToTrainer();
            receiver.ConnectToHrm();
        }

        private void OnReceiveSpeed(object sender, double speed)
        {
            Console.WriteLine("Speed: {0} m/s", speed);
        }

        private void OnReceiveDistance(object sender, int distance)
        {
            Console.WriteLine("Distance: {0} meters", distance);
        }

        private void OnReceiveHeartRate(object sender, int heartRate)
        {
            Console.WriteLine("Heart rate: {0} bpm", heartRate);
        }

        private void OnReceiveRrIntervals(object sender, int[] rrIntervals)
        {
            Console.WriteLine("R-R intervals: {0}", String.Join(", ", rrIntervals));
        }
    }
}
