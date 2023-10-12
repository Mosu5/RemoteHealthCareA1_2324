using DoctorWPFApp.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorWPFApp.MVVM.ViewModel
{
    internal class MainWindowViewModel : ObservableObject
    {
        /* Commands */
        // TODO


        /* Data */
        public ObservableCollection<Patient> Patients { get; set; }
        public ObservableCollection<string> ChatMessages { get; set; }



        // TODO add doctor code

        public MainWindowViewModel()
        {
            Patients = new ObservableCollection<Patient>
            {
                new Patient
                {
                    Name = "Bob",
                    Speed = 1,
                    Distance = 1,
                    HeartRate = 1,
                    ChatMessages = new List<string> {"hi"}
                },

                new Patient
                {
                    Name = "Jan",
                    Speed = 1,
                    Distance = 1,
                    HeartRate = 1,
                    ChatMessages = new List<string> {"hi"}
                }
            };
            ChatMessages = new ObservableCollection<string>();


        }

    }
}
