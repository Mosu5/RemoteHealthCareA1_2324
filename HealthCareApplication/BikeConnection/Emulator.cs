using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeConnection
{
    // Class to generate bike/heartbeat data
    internal class Emulator
    {

        
        public Emulator() { }//of hier elapsed time starten

        //data: Speed, Distance, Data Page , Elapsed Time, HeartBeat
        public byte[] GenerateBikeData(byte elapsedTime)
        {
            
            //start time 
            byte[] data = new byte[13];
            Random r = new Random();

            //Getting random values
            byte speed = GenerateSpeedData();
            byte distance = 0;
            distance++;
            elapsedTime++;
            data[0] = 0xA4;
            data[1] = 0x09;
            data[2] = 0x4E;
            data[3] = 0x05;
            data[4] = 0x10; // Data page number
            data[5] = 0x19; // Equipment Type Bit Field
            data[6] = elapsedTime;
            data[7] = distance;
            data[8] = speed;//lsb
            data[9] = speed;//msb
            data[10] = GenerateHeartBeatData();
            data[11] = elapsedTime;
            return data; 
        }

        public static byte GenerateHeartBeatData()
        {
            Random random = new Random();
            byte minValue = 0;
            byte maxValue = 254;

            return (byte)random.Next(minValue, maxValue);
        }

        public static byte GenerateSpeedData()
        {
            Random random = new Random();
            byte minValue = 0; // Minimum value
            byte maxValue = 60; // Maximum value

            // Generates a random double between minValue (inclusive) and maxValue (exclusive)
            return (byte)random.Next(minValue, maxValue);
        }
    }
}
