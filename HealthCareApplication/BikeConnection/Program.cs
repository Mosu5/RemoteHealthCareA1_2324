﻿using System;
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
            BLE bleBike = new BLE();
            BLE bleHeart = new BLE();
            Thread.Sleep(1000); // We need some time to list available devices

            // List available devices
            List<String> bleBikeList = bleBike.ListDevices();
            Console.WriteLine("Devices found: ");
            foreach (var name in bleBikeList)
            {
                Console.WriteLine($"Device: {name}");
            }

            // Connecting
            errorCode = errorCode = await bleBike.OpenDevice("Tacx Flux 00472");
            // __TODO__ Error check

            var services = bleBike.GetServices;
            foreach (var service in services)
            {
                Console.WriteLine($"Service: {service.Name}");
            }

            // Set service
            errorCode = await bleBike.SetService("6e40fec1-b5a3-f393-e0a9-e50e24dcca9e");
            // __TODO__ error check

            // Subscribe
            bleBike.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            errorCode = await bleBike.SubscribeToCharacteristic("6e40fec2-b5a3-f393-e0a9-e50e24dcca9e");

            // Heart rate
            errorCode = await bleHeart.OpenDevice("Decathlon Dual HR");

            await bleHeart.SetService("HeartRate");

            bleHeart.SubscriptionValueChanged += BleBike_SubscriptionValueChanged;
            await bleHeart.SubscribeToCharacteristic("HeartRateMeasurement");


            Console.Read();
        }

        private static void BleBike_SubscriptionValueChanged(object sender, BLESubscriptionValueChangedEventArgs e)
        {
            byte[] receivedData = e.Data;

            bool ifVerified = VerifyMessage(receivedData);

            if (ifVerified)
            {
                DecodeData(receivedData);
            }
            else
            {
                Console.WriteLine("Invalid Data");
            }
        }

        private static void DecodeData(byte[] message)
        {
            var dataPage = message[4];
            if (dataPage == 16)
            {
                // check if type is of data page 16      
                byte msb = message[8];
                byte lsb = message[9];
                int mergedValue = (msb << 8) | lsb;
                double speed = mergedValue * 0.001;
                int distance = message[7];
                // Console.WriteLine("MSB: " + message[8] + " LSB: " + message[9]);
                Console.WriteLine("Data:  {0}",
                    BitConverter.ToString(message).Replace("-", " "));
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