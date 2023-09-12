using System;
using BikeConnection.Receiver;

namespace TrainerDataTesting
{
    class Program
    {
        /// <summary>
        /// Connects to either the regular BLE devices or uses a built-in emulator to generate random bytes.
        /// If the EmulatedReceiver class is instantiated, upon calling it's Connect() method, it will create
        /// a new BLEReceiver class and will use the ReceivedTrainerMessage and ReceivedHRMMessage of that class,
        /// and give it byte arrays with random payloads. Both classes implement the IReceiver interface, to
        /// ensure abstraction.
        /// </summary>
        static void Main(string[] args)
        {
            IReceiver receiver = new EmulatedReceiver();

            // Subscribe to the receiver's events.
            receiver.ReceivedSpeed += OnReceiveSpeed;
            receiver.ReceivedDistance += OnReceiveDistance;

            receiver.Connect();
        }

        private static void OnReceiveSpeed(object sender, double speed)
        {
            Console.WriteLine("Speed: {0} km/h", speed);
        }

        private static void OnReceiveDistance(object sender, int distance)
        {
            Console.WriteLine("Distance: {0} meters", distance);
        }
    }
}