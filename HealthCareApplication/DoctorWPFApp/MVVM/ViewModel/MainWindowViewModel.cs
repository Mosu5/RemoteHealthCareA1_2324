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
            Patients = new ObservableCollection<Patient>();
            ChatMessages = new ObservableCollection<string>();


        }

    }
}
