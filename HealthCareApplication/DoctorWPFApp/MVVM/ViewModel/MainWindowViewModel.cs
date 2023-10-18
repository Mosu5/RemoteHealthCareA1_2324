using DoctorWPFApp.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security;
using System.Windows;
using System.Windows.Controls;

namespace DoctorWPFApp.MVVM.ViewModel
{
    /// <summary>
    /// Handles the data between the views and the server-client logic
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
    {


        #region RelayCommands
        public RelayCommand LoginCommand => new RelayCommand(execute =>
        {
            TestLogin();
            InitPlaceHolderData();
        }, canExecute => ValidateUser());
        #endregion

        #region Login
        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {

                _username = value;
                OnPropertyChanged(nameof(Username));

            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        private void TestLogin()
        {
            if (_username != "super" || _password != "sexy") // TODO change 
            {
                MessageBox.Show("Wrong username or password.", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Navigator.NavToSessionWindow();
        }

        private bool ValidateUser()
        {
            if (_username == null || _password == null)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region PatientData
        private Patient _selectedPatient = new Patient(); // start with empty patient
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
        public ObservableCollection<Patient> Patients { get; set; } = new ObservableCollection<Patient>();
        #endregion

        private void InitPlaceHolderData()
        {

            Patients = new ObservableCollection<Patient>
            {
                new Patient
                {
                    Name = "Bob",
                    Speed = 1,
                    Distance = 1,
                    HeartRate = 1,
                    ChatMessages = new List<string> { "hi bob is mijn naam", "fdsfdsfdsf", "dfsdffdfsdf" }
                },

                new Patient
                {
                    Name = "Jan",
                    Speed = 2,
                    Distance = 5,
                    HeartRate = 3,
                    ChatMessages = new List<string> { "jo dit is jan" }
                }
            };

            OnPropertyChanged(nameof(Patients));
        }

        // TODO connect relayCommands and events to doctor code


       





    }
}
