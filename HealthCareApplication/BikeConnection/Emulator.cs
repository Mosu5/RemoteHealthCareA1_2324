using System;
using System.Linq;
using System.Threading;

namespace BikeConnection
{
    // Class to generate bike/heartbeat data
    internal class Emulator
    {
        public byte elapsedTime { get; private set; }
        public byte distance { get; private set; }
        public byte speed { get; private set; }
        public int heartRate{ get; private set; }
        public int gear { get; set; }

        private double smoothedSpeed = 0; //
        private double smoothingFactor = 0.3;// the lesser it is the smaller the change in speed is



        private Random random;

        public Emulator()
        {
            random = new Random();
            elapsedTime = 0;
            distance = 0;
            speed = 0; 
        }

        //TODO also make a method that simulates sending the data array for bike and heart rate.

        //Simulates the speed variance
        public void SimulateSpeedVariation()
        {
            TerrainType currentTerrain = TerrainType.Flat; //Starting terrain can be adjusted
            int currentGear = 1; // Starting gear usually 1 

            Console.WriteLine($"Generated Speed Data: 0");
            Console.WriteLine($"Current Terrain: {currentTerrain}, Current Gear: {currentGear}");

            while (true)
            {
                // Generate and print speed data
                byte[] speedData = GenerateSpeedData();

                Console.WriteLine($"Generated Speed Data: {speedData[0]}");
                Console.WriteLine($"Current Terrain: {currentTerrain}, Current Gear: {currentGear}");

                // Change terrain and gear randomly at regular intervals
                if (elapsedTime % 10 == 0) // Change every 10 iterations (adjust as needed)
                { 
                    currentTerrain = (TerrainType)random.Next(0, 3); // Random terrain
                    if (currentTerrain == TerrainType.Uphill)//if youre going uphill its not smart to use 7. gear or 6. or 5. or 4. .
                    {
                        currentGear = random.Next(1, 4);//gears between 4 and 7
                    }
                    else if (currentTerrain == TerrainType.Downhill)//same logic
                    {
                        currentGear = random.Next(4, 8);// gears between 4 and 7
                    }
                    else// else it doesnt matter that much
                    {
                        currentGear = random.Next(1, 8);
                    }
                   
                    Console.WriteLine($"Changed to {currentTerrain} terrain and gear {currentGear}");
                }
                
                Thread.Sleep(1000); // Sleep for 1 second
            }
        }

        public byte[] GenerateBikeData()
        {
            this.distance++;
            this.elapsedTime++;

            //Getting random values
            byte[] speed = GenerateSpeedData();
            byte distance = 0;

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

        public byte GenerateHeartBeatData()
        {
            byte minValue = 0;
            byte maxValue = 254;

            return (byte)random.Next(minValue, maxValue);
        }

        private byte[] GenerateSpeedData()
        {
            byte minValue = 0; // Min speed
            byte maxValue = 60; // Max Speed

            byte randomSpeed = (byte)random.Next(minValue, maxValue);//Random between min and max (max excluded)

            smoothedSpeed = (1 - smoothingFactor) * smoothedSpeed + smoothingFactor * randomSpeed;// Applying smoothing

            var bytes = BitConverter.GetBytes((byte)smoothedSpeed);// Convert smoothedSpeed to bytes

            return bytes;
        }

        //Randomly changes terrain
        public void ChangeTerrain(TerrainType terrain)
        {
            switch (terrain)
            {
                case TerrainType.Flat: // For flat terrain, no changes needed
                    break;
                case TerrainType.Uphill:// When uphill max gear is 3, adjustments for speed and heartrate are needed 
                    if (this.gear > 3)
                    {
                        this.gear = 3; // Limit gear to 3 on uphill 
                    }
                    this.speed -= (byte)random.Next(1, 3); // Decrease speed by a random value between 1 and 2
                    this.gear = Math.Min(7, this.gear + 1);
                    //this.heartRate += (byte)random.Next(10, 20); // Increase heart rate by a random value between 10 and 19
                    break;
                case TerrainType.Downhill:// Adjust speed and heart rate for downhill 
                    if (this.gear > 7)
                    {
                        this.gear = 7; // Limit gear to 5 on downhill
                    }
                    this.speed += (byte)random.Next(1, 3); // Increase speed by a random value between 1 and 2
                    this.gear = Math.Min(7, this.gear + 1);
                    break;
                default:
                    throw new ArgumentException("Invalid terrain type.");
            }
        }

        // Calculate gear based on speed
        public void ChangeGear()
        {
            int maxGear = 7; 

            int newGear = (int)Math.Round((double)this.speed / 4);// the higher the divider is the less it changes based on speed (4 is best)

            if (newGear < 1) newGear = 1;
            if (newGear > maxGear) newGear = maxGear;

            this.gear = newGear;
        }

        //Resets the distance when it reaches 256
        public void ResetDistance()
        {
            if (distance == 256)
            {
                distance = 0;
            }
        }

        //Hex converters
        public static int Convert(byte hex)
        {
            return (int)hex;
        }

        //Hex converters
        public static int[] Convert(byte[] hex)
        {
            return hex.Select(b => (int)b).ToArray();
        }

    }

    //Terrain types for realistic simulation
    public enum TerrainType
    {
        Flat,
        Uphill,
        Downhill
    }
}
