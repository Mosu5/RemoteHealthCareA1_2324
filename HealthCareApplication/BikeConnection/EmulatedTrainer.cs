using System;
using System.Linq;
using System.Threading;

namespace BikeConnection
{
    internal class EmulatedTrainer
    {
        private double smoothedSpeed = 15.0; // Initial value, adjust as needed
        private double smoothingFactor = 0.1;
        private double maxSpeed = 60;

        private Random random;
        public byte elapsedTime { get; private set; }
        public byte distance { get; private set; }
        public int gear { get; private set; }
        public TerrainType currentTerrain { get; private set; }

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
                if (previousSpeed > 20) 
                {
                    this.smoothedSpeed *= 1.0;
                    this.gear = random.Next(5, 8);
                }else if (previousSpeed < 20)
                {
                    this.smoothedSpeed *= 0.99;
                    this.gear = random.Next(1, 5);
                }               
            }

            double gearMultiplier = 1 + (gear - 1) * 0.1; // Adjust gear effect

            double speed = this.smoothedSpeed * gearMultiplier;
            double result = Math.Min(speed, maxSpeed);

            if ( result < 0)
            {
                return maxSpeed;
            }
            return result; // Cap speed at maxSpeed if it exceeds
        }


        public byte[] GenerateSpeedData()
        {
            this.elapsedTime++;
            this.distance++;

            // Calculate speed based on terrain and gear
            double speed = CalculateSpeed(currentTerrain, gear);

            // Apply smoothing
            this.smoothedSpeed += (speed - this.smoothedSpeed) * smoothingFactor;

            // Convert smoothedSpeed to bytes
            byte[] bytes = BitConverter.GetBytes((ushort)Math.Round(this.smoothedSpeed));

            return bytes;
        }

        public void SimulateSpeedVariation()
        {
            while (true)
            {
                // Generate and print speed data
                byte[] speedData = GenerateSpeedData();

                Console.WriteLine($"Generated Speed Data: {speedData[0]}");
                Console.WriteLine($"Current Terrain: {currentTerrain}, Current Gear: {gear}");

                // Change terrain and gear randomly at regular intervals
                if (elapsedTime % 5 == 0)
                {
                    currentTerrain = (TerrainType)random.Next(0, 3);
                    Console.WriteLine($"Changed to {currentTerrain} terrain and gear {gear}");
                }

                // Store previous speed data
                previousSpeedData = speedData;

                Thread.Sleep(1000); // Sleep for 1 second
            }
        }
    }

    enum TerrainType
    {
        Flat, Downhill, Uphill
    }
}
