using System;

namespace DoctorWPFApp.Networking
{
    public class Statistic
    {
        public double Speed;
        public int Distance;
        public int HeartRate;
        public string Username;
        public Statistic(double speed, int distance, int heartRate, string username)
        {
            Speed = speed;
            Distance = distance;
            HeartRate = heartRate;
            Username = username;
        }
    }
}
