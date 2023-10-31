using System;

namespace PatientApp.DeviceConnection
{
    public class Statistic
    {
        public double Speed;
        private int _distance;
        public int Distance
        {
            get { return _distance; }
            set
            {
                AccumulatedDistance += value;
                _distance = value;
            }
        }
        public int HeartRate;
        public int[] RrIntervals;

        public int AccumulatedDistance = 0;

        public Statistic()
        {
            Speed = -1;
            _distance = -1;
            HeartRate = -1;
            RrIntervals = new int[0];
        }

        public bool IsComplete()
        {
            return Speed != -1 && Distance != -1
                && HeartRate != -1 && RrIntervals != new int[0];
        }
    }
}
