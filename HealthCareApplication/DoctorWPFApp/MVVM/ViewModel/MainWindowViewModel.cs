using DoctorWPFApp.MVVM.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DoctorWPFApp.MVVM.ViewModel
{
    /// <summary>
    /// Handles the data between the views and the server-client logic
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
    {
        /* Commands */
        public RelayCommand GetPatientData => new RelayCommand(execute => InitPlaceHolderData());

        // TODO


        /* Data */
        private Patient _selectedPatient = new Patient(); // start with empty patient
        public ObservableCollection<Patient> Patients { get; set; } = new ObservableCollection<Patient>();


        public Patient SelectedPatient
        {
            get { return _selectedPatient; }
            set
            {

                if (_selectedPatient != value)
                {
                    _selectedPatient = value;
                    OnPropertyChanged(nameof(SelectedPatient));
                }
            }
        }

        // TODO connect relayCommands and events to doctor code

        public MainWindowViewModel()
        {
            //try
            //{
            //    // Start listening for messages 
            //   // doesnt work
            //    DoctorProxy.Listen().Wait();

            //    // Get placeholder data
            //    Patients = InitPlaceHolderData();
            //}
            //catch (CommunicationException ex)
            //{
            //    Logger.Log($"CommunicationException: {ex.Message}\n{ex.StackTrace}", LogType.CommunicationExceptionInfo);
            //}
        }

        private void InitPlaceHolderData()
        {
            Patients.Add(new Patient
            {
                Name = "Bob",
                Speed = 1,
                Distance = 1,
                HeartRate = 1,
                ChatMessages = new List<string> { "hi bob is mijn naam", "fdsfdsfdsf", "dfsdffdfsdf" }
            });

            Patients.Add(

                new Patient
                {
                    Name = "Jan",
                    Speed = 2,
                    Distance = 5,
                    HeartRate = 3,
                    ChatMessages = new List<string> { "jo dit is jan" }
                });
        }


    }
}
