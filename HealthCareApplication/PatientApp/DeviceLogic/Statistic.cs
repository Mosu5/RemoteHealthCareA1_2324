using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientApp.DeviceConnection
{
    public class Statistic
    {
        public double Speed;
        public int Distance;
        public int HeartRate;
        public int[] RrIntervals;

        public Statistic()
        {
            Speed = -1;
            Distance = -1;
            HeartRate = -1;
            RrIntervals = new int[0];
        }

        public bool IsComplete()
        {
            return Speed != -1 && Distance != -1 &&
                HeartRate != -1 && RrIntervals != new int[0];
        }
    }
}
