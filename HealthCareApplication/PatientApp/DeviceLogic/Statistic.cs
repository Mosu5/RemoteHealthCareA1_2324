using System;

namespace PatientApp.DeviceConnection
{
    public class Statistic
    {
        private double _speed;
        private int _distance;
        private int _heartRate;
        private int[] _rrIntervals;

        public EventHandler<Statistic> OnStatisticComplete;

        public Statistic()
        {
            _speed = -1;
            _distance = -1;
            _heartRate = -1;
            _rrIntervals = new int[0];
        }

        public double GetSpeed()
        {
            return _speed;
        }

        public void SetSpeed(double speed)
        {
            _speed = speed;
            InvokeEventIfComplete();
        }

        public int GetDistance()
        {
            return _distance;
        }

        public void SetDistance(int distance)
        {
            _distance = distance;
            InvokeEventIfComplete();
        }

        public int GetHeartRate()
        {
            return _heartRate;
        }

        public void SetHeartRate(int heartRate)
        {
            _heartRate = heartRate;
            InvokeEventIfComplete();
        }

        public int[] GetRrIntervals()
        {
            return _rrIntervals;
        }

        public void SetRrIntervals(int[] rrIntervals)
        {
            _rrIntervals = rrIntervals;
            InvokeEventIfComplete();
        }

        /// <summary>
        /// Checks if all data attributes (speed, distance, etc.) have been assigned to.
        /// Then invokes the OnStatisticComplete event, signaling that this Statistic has all values it needs.
        /// </summary>
        private void InvokeEventIfComplete()
        {
            if (_speed != -1 && _distance != -1 &&
                _heartRate != -1 && _rrIntervals != new int[0])
                OnStatisticComplete?.Invoke(this, this);
        }
    }
}
