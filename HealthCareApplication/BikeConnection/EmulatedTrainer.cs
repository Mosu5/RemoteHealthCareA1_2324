using System;
using System.Linq;
using System.Threading;

namespace BikeConnection
{
    internal class EmulatedTrainer
    {
        private double smoothedSpeed = 2.5; // start speed with 2.5 m/s
        private double smoothingFactor = 0.1;// smoothingfactor
        private double maxSpeed = 10;

        private Random random;
        public byte elapsedTime { get; private set; }
        public byte distance { get; private set; }
        public int gear { get; private set; }
        public TerrainType currentTerrain { get; private set; }
        public byte[] speed; 

        private byte[] previousSpeedData;

        public EmulatedTrainer()
        {
            this.random = new Random();
            this.elapsedTime = 0;
            this.distance = 0;
            this.gear = 1;
            this.currentTerrain = TerrainType.Flat; // Initialize to flat terrain
            this.previousSpeedData = GenerateSpeedData(); // Initialize with some realistic speed
        }

        private double CalculateSpeed(TerrainType terrain, int gear)
        {
            byte previousSpeed = 0;
            if (previousSpeedData != null)
            {
                previousSpeed = this.previousSpeedData[0];
            }

            if (terrain == TerrainType.Uphill)
            {
                this.smoothedSpeed *= 0.92; // Adjust as needed for uphill terrain, 0.8 will decrease the speed
                this.gear = random.Next(1,4);
                
            }
            else if (terrain == TerrainType.Downhill)
            { 
                this.smoothedSpeed *= 1.05; // Adjust as needed for downhill terrain
                this.gear = random.Next(5, 8);
            }
            else // Flat terrain
            {
                if (previousSpeed > 5) 
                {
                    this.smoothedSpeed *= 1.0;
                    this.gear = random.Next(5, 8);
                }else if (previousSpeed < 5)
                {
                    this.smoothedSpeed *= 0.99;
                    this.gear = random.Next(1, 5);
                }               
            }

            double gearMultiplier = 1 + (gear - 1) * 0.1; // Adjust gear effect

            double speed = this.smoothedSpeed * gearMultiplier;
            double result = Math.Min(Math.Max(speed, 0), maxSpeed);

            return result; // Cap speed at maxSpeed if it exceeds
        }

        public byte[] GenerateBikeData()
        {
            //Getting random values
            speed = GenerateSpeedData();

            byte[] data = new byte[13];
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

        public byte[] GenerateSpeedData()
        {
            this.elapsedTime++;
            this.distance++;
            ResetDistance();
            if (elapsedTime % 5 == 0)
            {
                currentTerrain = (TerrainType)random.Next(0, 3);
                Console.WriteLine($"Changed to {currentTerrain} terrain and gear {gear}");
            }


            // Calculate speed based on terrain and gear
            double speed = CalculateSpeed(currentTerrain, gear);

            // Apply smoothing
            this.smoothedSpeed += (speed - this.smoothedSpeed) * smoothingFactor;

            // Convert smoothedSpeed to bytes
            byte[] bytes = BitConverter.GetBytes((ushort)Math.Round(this.smoothedSpeed * 1000));
            Console.WriteLine("Current terrain: " + currentTerrain  + "----------------------------------------------------------" + gear);
            return bytes;
        }

        public void ResetDistance()
        {
            if (distance == 256)
            {
                distance = 0;
            }
        }

        public void SimulateSpeedVariation()
        {
            while (true)
            {
                // Generate and print speed data
                speed = GenerateSpeedData();

                Console.WriteLine($"Generated Speed Data: {speed[0]}");
                Console.WriteLine($"Current Terrain: {currentTerrain}, Current Gear: {gear}");

                // Change terrain and gear randomly at regular intervals
                if (elapsedTime % 5 == 0)
                {
                    currentTerrain = (TerrainType)random.Next(0, 3);
                    Console.WriteLine($"Changed to {currentTerrain} terrain and gear {gear}");
                }

                // Store previous speed data
                previousSpeedData = speed;

                Thread.Sleep(1000); // Sleep for 1 second
            }
        }
    }

    enum TerrainType
    {
        Flat, Downhill, Uphill
    }
}
