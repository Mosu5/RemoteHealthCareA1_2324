using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    public class UserStat
    {
        public double speed { get; set; }
        public int distance { get; set; }
        public byte heartrate { get; set; }

        public UserStat(double speed, int distance, byte heartrate)
        {
            this.distance = distance;
            this.speed = speed;
            this.heartrate = heartrate;
        }

        public string ToString()
        {
            return $"Distance: {distance} ---, Speed: {speed}, --- Heartrate: {heartrate}";
        }
    }
}
