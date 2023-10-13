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
        private Patient _selectedPatient;
        public ObservableCollection<Patient> Patients { get; set; }
        public ObservableCollection<string> ChatMessages { get; set; }

        public Patient SelectedPatient
        {
            get { return _selectedPatient; }
            set
            {
                //if (_origin.CountryCode != value)
                //{
                //    _origin.CountryCode = value;
                //    RaisePropertyChanged(nameof(OriginCountryCode));
                //}

                if (_selectedPatient != value)
                {
                    _selectedPatient = value;
                    OnPropertyChanged(nameof(SelectedPatient));
                   
                }
            }
        }

       


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
                    Speed = 2,
                    Distance = 5,
                    HeartRate = 3,
                    ChatMessages = new List<string> {"hi"}
                }
            };
            ChatMessages = new ObservableCollection<string>();


        }

    }
}
