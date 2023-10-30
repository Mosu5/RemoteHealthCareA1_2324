using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DoctorWPFApp.MVVM.Model
{
    internal class Patient
    {
        public string Name { get; set; }

        // Real-Time data
        public double Speed { get; set; }

        public double Distance { get; set; }
        public double HeartRate {  get; set; }


        // Historical Data / Logs
        public List<PatientData> PatientDataCollection { get; set; } = new();

        public ObservableCollection<string> ChatMessages { get; set; } = new();



    }
}
