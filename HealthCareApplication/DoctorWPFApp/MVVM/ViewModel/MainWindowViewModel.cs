using DoctorWPFApp.MVVM.Model;
using DoctorWPFApp.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DoctorWPFApp.MVVM.ViewModel
{
    /// <summary>
    /// Handles the data between the views and the server-client logic
    /// </summary>
    internal class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            Thread listenerThread = new(async () => await RequestHandler.Listen());
            listenerThread.Start();

            // Subscribe to response event handler
            RequestHandler.LoggedIn += OnLoginResponse;
        }

        #region Commands called by the UI

        public RelayCommand LoginCommand => new(async (execute) =>
        {
            // Send the login command
            JsonObject loginRequest = DoctorFormat.LoginMessage(_username, _password);
            await ClientConn.SendJson(loginRequest);

            InitPlaceHolderData();
        }, canExecute => !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password)); // Checks if fields are not null or empty

        private bool _isSessionRunning = false;
        public RelayCommand StartStopSession => new(async (execute) =>
        {
            JsonObject sessionRequest =
                _isSessionRunning
                ? DoctorFormat.SessionStartMessage(SelectedPatient.Name)
                : DoctorFormat.SessionStopMessage(SelectedPatient.Name);

            // Toggle boolean
            _isSessionRunning = !_isSessionRunning;

            await ClientConn.SendJson(sessionRequest);
        }, canExecute => true);

        #endregion

        #region Login properties

        private string? _username;
        public string? Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string? _password;
        public string? Password
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

        #region Response actions

        /// <summary>
        /// When a response is received from the server, give feedback to the user or switch window.
        /// </summary>
        private void OnLoginResponse(object? sender, bool successfulLogin)
        {
            if (successfulLogin)
            {
                Application.Current.Dispatcher.Invoke(() => Navigator.NavToSessionWindow());
                return;
            }

            MessageBox.Show("Wrong username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion

        /// <summary>
        /// TODO remove when it is not needed anymore; this is a temporary method for sample user data.
        /// </summary>
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
    }
}
