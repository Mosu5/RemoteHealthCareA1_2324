using System;
using System.Collections.Generic;

namespace DoctorWPFApp.MVVM.Model
{
    internal class Patient
    {
        public string Name { get; set; }

        // Real-Time data
        public double Speed { get; set; }

        public double Distance { get; set; }
        public double HeartRate {  get; set; }
        public double RrInterval { get; set; }
        public double Time { get; set; }


        // Historical Data / Logs
        public List<PatientData> PatientDataCollection { get; set; } = new List<PatientData>();

        public List<String> ChatMessages { get; set; } = new List<String>();



    }
}
