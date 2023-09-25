using BikeConnection.Receiver;
using Newtonsoft.Json;
using System;
using System.IO;

namespace BikeConnection
{
    public class Data
    {
        public string DataType { get; set; }
        public object Value { get; set; }
    }

    public class Client
    {
        private StreamWriter streamWriter = new StreamWriter(@"C:\temp\mytest.json");
        
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
            receiver.ReceivedSpeed += OnReceiveSpeed;
            receiver.ReceivedDistance += OnReceiveDistance;
            receiver.ReceivedHeartRate += OnReceiveHeartRate;
            receiver.ReceivedRrIntervals += OnReceiveRrIntervals;

            receiver.ConnectToTrainer();
            receiver.ConnectToHrm();
        }
        
        /// <summary>
        /// These methods are called when the receiver receives data from the BLE bike or emulator. This data is then converted to json
        /// data and stored in the file specified in the streamWriter.
        /// </summary>

        private void OnReceiveSpeed(object sender, double speed)
        {
            Console.WriteLine("Speed: {0} m/s", speed);
            
            string jsonData = JsonConvert.SerializeObject(new Data
            {
                DataType = "Speed",
                Value = speed
            });
            
            WriteDataToFile(jsonData);
        }

        private void OnReceiveDistance(object sender, int distance)
        {
            Console.WriteLine("Distance: {0} meters", distance);
            
            string jsonData = JsonConvert.SerializeObject(new Data
            {
                DataType = "Distance",
                Value = distance
            });
            
            WriteDataToFile(jsonData);
        }

        private void OnReceiveHeartRate(object sender, int heartRate)
        {
            Console.WriteLine("Heart rate: {0} bpm", heartRate);
            
            string jsonData = JsonConvert.SerializeObject(new Data
            {
                DataType = "HeartRate",
                Value = heartRate
            });
            
            WriteDataToFile(jsonData);
        }

        private void OnReceiveRrIntervals(object sender, int[] rrIntervals)
        {
            Console.WriteLine("R-R intervals: {0}", string.Join(", ", rrIntervals));
            
            string jsonData = JsonConvert.SerializeObject(new Data
            {
                DataType = "RRIntervals",
                Value = rrIntervals
            });
            
            WriteDataToFile(jsonData);
        }

        private void WriteDataToFile(string jsonData)
        {
            if (streamWriter != null)
            {
                streamWriter.WriteLine(jsonData);
                streamWriter.Flush();
            }
        }
        
    }
}
