using System;

namespace BikeConnection
{
    // Class to generate bike/heartbeat data
    internal class Emulator
    {
        //data: Speed, Distance, Data Page , Elapsed Time, HeartBeat
        public static byte[] GenerateBikeData(byte elapsedTime)
        {
            //start time 
            byte[] data = new byte[13];

            //Getting random values
            byte[] speed = GenerateSpeedData();
            byte distance = 0;
            
            distance++;
            elapsedTime++;
            data[0] = 0xA4;// sync byte
            data[1] = 0x09;// Message Length
            data[2] = 0x4E;// Message ID
            data[3] = 0x05;// Channel ID
            data[4] = 0x10;// Data page number
            data[5] = 0x19;// Equipment Type Bit Field
            data[6] = (byte)(elapsedTime / 0.25);
            data[7] = distance;
            data[8] = speed[0];// lsb
            data[9] = speed[1];// msb
            data[10] = 0xFF;
            data[11] = 0xFF;
            
            byte checkSum = 0;
            for (int i = 0; i < data.Length - 1; i++)
            {
                checkSum ^= data[i];
            }
            data[12] = checkSum;
            
            return data; 
        }

        public static byte GenerateHeartBeatData()
        {
            Random random = new Random();
            byte minValue = 0;
            byte maxValue = 254;

            return (byte)random.Next(minValue, maxValue);
        }

        public static byte[] GenerateSpeedData()
        {
            Random random = new Random();
            byte minValue = 0; // Minimum value
            byte maxValue = 60; // Maximum value

            //bytes[0] = 1 // 
            //bytes[1] = 255 // lsb
            var speed = (byte)random.Next(minValue, maxValue); ; // value
            Console.WriteLine(speed);
            //var n = Convert.ToUInt16(s2, 16);  // convert naar 16-bit getal

            var bytes = BitConverter.GetBytes(speed); // convert naar aantal bytes
            // Generates a random double between minValue (inclusive) and maxValue (exclusive)
            return bytes;
        }
    }
}
