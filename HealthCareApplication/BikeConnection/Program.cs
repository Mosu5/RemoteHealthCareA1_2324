using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avans.TI.BLE;

namespace FietsDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int errorCode = 0;
            BLE bleHeart = new BLE();
            Thread.Sleep(1000); // We need some time to list available devices

            // List available devices
            List<String> bleBikeList = bleHeart.ListDevices();
            Console.WriteLine("Devices found: ");
            foreach (var name in bleBikeList)
            {
                Console.WriteLine($"Device: {name}");
            }

            // Heart rate
            errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");

            await bleHeart.SetService("HeartRate");

            bleHeart.SubscriptionValueChanged += SubscriptionValueChanged;
            await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");

            Console.Read();
        }

        private static void SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            byte[] receivedData = e.Data;

            bool ifVerified = VerifyMessage(receivedData);

            DecodeSpeedData(receivedData);
        }

        private static void DecodeSpeedData(byte[] message)
        {
            string stringValue = "";
            foreach (byte b in message)
            {
                int integerValue = b; // Convert byte to integer
                stringValue += integerValue.ToString() + "\t"; // Convert integer to string
            }
            Console.WriteLine(stringValue);
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