using System;

namespace DoctorWPFApp.Networking
{
    public class Statistic
    {
        public double Speed;
        public int Distance;
        public int HeartRate;

        public Statistic(double speed, int distance, int heartRate)
        {
            Speed = speed;
            Distance = distance;
            HeartRate = heartRate;
        }
    }
}
