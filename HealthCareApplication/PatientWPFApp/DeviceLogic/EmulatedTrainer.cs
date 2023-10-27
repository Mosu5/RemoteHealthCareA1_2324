using System;
using System.Threading;

namespace PatientApp.DeviceConnection
{
    internal class EmulatedTrainer
    {
        private double smoothedSpeed = 2.5; //Start speed with 2.5 m/s
        private double smoothingFactor = 0.1;//Smoothingfactor for speed adjustments
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
            this.currentTerrain = TerrainType.Flat; //Initializing to flat terrain
            this.previousSpeedData = GenerateSpeedData(); //Initializing with some realistic speed
        }

        //Method that calculates speed based on gear and current Terraintype
        private double CalculateSpeed(TerrainType terrain, int gear)
        {
            byte previousSpeed = 0;//Attribute that stores previous speed for check on line 53 and 57
            if (previousSpeedData != null)
            {
                previousSpeed = this.previousSpeedData[0];
            }

            if (terrain == TerrainType.Uphill)//Speed will decrease when current TerrainType is uphill. Random gears between 1-3.
            {
                this.smoothedSpeed *= 0.92; 
                this.gear = random.Next(1,4);
                
            }
            else if (terrain == TerrainType.Downhill)//Speed will increase when current TerrainType is downhill. Random gears between 5-7.
            { 
                this.smoothedSpeed *= 1.05; 
                this.gear = random.Next(5, 8);
            }
            else //Gear will vary based on current speed
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

            double speed = this.smoothedSpeed * gearMultiplier;//Calculating speed after applying gear effect
            double result = Math.Min(Math.Max(speed, 0), maxSpeed);//Ensures that speed stays within range
            return result;// Cap speed at maxSpeed if it exceeds
        }

        //Simulating Bike data array
        public byte[] GenerateBikeData()
        {
            //Generating speed data
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

        public byte[] GenerateHeartRateData()
        {
            double speed = CalculateSpeed(currentTerrain, gear);// Calculate speed based on terrain and gear

            this.smoothedSpeed += (speed - this.smoothedSpeed) * smoothingFactor;// Apply smoothing

            // Create message
            byte heartRate = (byte)(60 + Math.Round(speed * 15));
            byte[] rrIntervals = new byte[new Random().Next(1, 10)];
            new Random().NextBytes(rrIntervals);
            byte[] messageBegin = new byte[] { 0x16, heartRate };
            byte[] message = new byte[messageBegin.Length + rrIntervals.Length];

            // Combine messageBegin and rrIntervals to one single byte array
            Buffer.BlockCopy(messageBegin, 0, message, 0, messageBegin.Length);
            Buffer.BlockCopy(rrIntervals, 0, message, messageBegin.Length, rrIntervals.Length);

            return message;
        }

        //Generator of the speed data
        public byte[] GenerateSpeedData()
        {
            this.elapsedTime++;
            this.distance++;
            ResetDistance();//Check the 
            if (elapsedTime % 5 == 0)//Changes terrain each 5 secs
            {
                currentTerrain = (TerrainType)random.Next(0, 3);
            }

            double speed = CalculateSpeed(currentTerrain, gear);// Calculate speed based on terrain and gear
            
            this.smoothedSpeed += (speed - this.smoothedSpeed) * smoothingFactor;// Apply smoothing
            
            byte[] bytes = BitConverter.GetBytes((ushort)Math.Round(this.smoothedSpeed * 1000));// Convert smoothedSpeed to bytes
            
            return bytes;
        }
        
        //Resets distance when reaching 255
        public void ResetDistance()
        {
            if (distance == 255)
            {
                distance = 0;
            }
        }

        //Simulate the used for DEBUG
        public void SimulateSpeedVariation()
        {
            while (true)
            {
                // Generate and print speed data
                speed = GenerateSpeedData();

                Console.WriteLine($"Generated Speed Data: {speed[0]}");
                Console.WriteLine($"Current Terrain: {currentTerrain}, Current Gear: {gear}");

                // Change terrain and gear randomly each 5 seconds
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

    public enum TerrainType
    {
        Flat, Uphill, Downhill
    }
}
