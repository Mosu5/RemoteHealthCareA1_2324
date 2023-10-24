using System;

namespace DoctorWPFApp.MVVM.Model
{
    internal class PatientData
    {
        public DateTime DateTime { get; set; } = DateTime.MinValue;
        public double RecordedSpeed { get; set; } = 0;
        public double RecordedDistance { get; set; } = 0;
        public double RecordedHeartRate { get; set; } = 0;
        public double RecordedRrInterval { get; set; } = 0;
    }
}