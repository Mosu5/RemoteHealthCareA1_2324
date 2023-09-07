using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;
using BikeConnection;

namespace FietsDemo
{
    class Program
    {
        //bool readData = true;
        //bool readOther = false;
        static async Task Main(string[] args)
        {
            //DEBUG
            foreach (var s in Emulator.GenerateBikeData(0))
            {
                Console.WriteLine(s);
            }

            Console.WriteLine("-------------------------");
            byte[] bytes = Emulator.GenerateSpeedData();

            foreach (var b in bytes)
            {
                Console.WriteLine(b);
            }

            Console.WriteLine(bytes.Length);

            byte lsb = bytes[0];
            byte msb = bytes[1];

            int mergedValue = (msb << 8) | lsb;

            Console.WriteLine(mergedValue);
            //DEBUG

            BLE bleBike;
            BLE bleHeart;
            bleBike = new BLE();
            bleHeart = new BLE();
            int errorCode = 0;

            Thread.Sleep(1000); // We need some time to list available devices

            // List available devices
            List<String> bleBikeList = bleBike.ListDevices();
            Console.WriteLine("Devices found: ");
            foreach (var name in bleBikeList)
            {
                Console.WriteLine($"Device: {name}");
            }

            // Connecting
            errorCode = errorCode = await bleBike.OpenDevice("Tacx Flux 01249");
            // __TODO__ Error check

            var services = bleBike.GetServices;
            foreach (var service in services)
            {
                Console.WriteLine($"Service: {service.Name}");
            }

            // Set service
            errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e"); //6e40fec1
            // __TODO__ error check

            // Subscribe
            bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e"); // 6e40fec2
            // Heart rate
            errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");

            await bleHeart.SetService("HeartRate");

            bleHeart.SubscriptionValueChanged += BleHeart_SubscriptionValueChanged;
            await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");


            Console.Read();
        }


        /// </summary>
        /// <param name="resistance">0-255</param>
        /// <returns>
        /// The resistance is being returned.
        /// </returns>
        //public void SetResistanceAsync(int resistance)
        //{
        //    if (bleBike == null)
        //        return;
        //    byte[] output = new byte[13];
        //    output[0] = 0x4A; //sync byte
        //    output[1] = 0x09; //Message Length
        //    output[2] = 0x4E; //Message type
        //    output[3] = 0x05; //Message type
        //    output[4] = 0x30; //Datatype
        //    output[11] = (byte)resistance;
        //    byte checksum = output[0];
        //    for (int i = 1; i < 12; i++)
        //    {
        //        checksum ^= output[i];
        //    }
        //    output[12] = checksum;
        //    bleBike.WriteCharacteristic("6e40fec3-b5a3-f393-e0a9-e50e24dcca9e", output);
        //}

        // General method to handle everything regarding data changed
        private static void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            byte[] receivedData = e.Data;

            bool ifVerified = VerifyMessage(receivedData);

            if (ifVerified)
            {
                DecodeSpeedData(receivedData);
            }
            else
            {
                Console.WriteLine("Invalid Data");
            }
        }

        private static void BleHeart_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            Console.WriteLine("Heart Data:  {0}", BitConverter.ToString(e.Data).Replace("-", " "));
            Console.WriteLine("Heart Rate: {0}", (int)e.Data[1]); // 2e byte
        }

        private static void DecodeSpeedData(byte[] message)
        {
            // The fifth byte is the datapage pointer (index 4)
            var dataPage = message[4];
            if (dataPage == 16)
            {
                // check if type is of data page 16      
                byte msb = message[8]; // byte 9 is most signifacnt byte
                byte lsb = message[9]; // byte 10 is the least significant byte
                int mergedValue = (msb << 8) | lsb;
                double speed = mergedValue * 0.001;
                int distance = message[7];
                // Console.WriteLine("MSB: " + message[8] + " LSB: " + message[9]);
                Console.WriteLine("Data:  {0}", BitConverter.ToString(message).Replace("-", " "));
                Console.WriteLine("Distance " + distance + " m");
                Console.WriteLine("Speed: " + speed + " m/s");
            }
        }


        private static byte CalculateChecksum(byte[] message, int messageLength)
        {
            byte checkSum = 0;
            for (var i = 0; i < messageLength; i++)
            {
                checkSum ^= message[i];
            }

            return checkSum;
        }

        private static bool VerifyMessage(byte[] message)
        {
            var messageLength = message.Length - 1;

            var calculatedCheckSum = CalculateChecksum(message, messageLength);
            var expectedCheckSum = message[messageLength];
            return calculatedCheckSum == expectedCheckSum;
        }
    }
}