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
using System.Xml.Linq;

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
            RequestHandler.ReceivedStat += OnStatReceived;
            RequestHandler.ReceivedChat += OnChatReceived;
        }

        #region Commands called by the UI

        public RelayCommand LoginCommand => new(async (execute) =>
        {
            // Send the login command
            JsonObject loginRequest = DoctorFormat.LoginMessage(_username, _password);
            await ClientConn.SendJson(loginRequest);

            InitPlaceHolderData(); // TODO remove when not needed anymore
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

        private string _messageToSend;
        public string MessageToSend
        {
            get { return _messageToSend; }
            set
            {
                _messageToSend = value;
                OnPropertyChanged(nameof(MessageToSend));
            }
        }

        public RelayCommand SendChatCommand => new RelayCommand(async (execute) =>
        {
            if (string.IsNullOrEmpty(_messageToSend)) return;

            SelectedPatient.ChatMessages.Add($"You: {_messageToSend}");
            OnPropertyChanged(nameof(SelectedPatient.ChatMessages));

            JsonObject chatToServer = DoctorFormat.ChatsSendMessage(_messageToSend, SelectedPatient.Name);
            await ClientConn.SendJson(chatToServer);
        });

        #endregion

        #region Properties of commands

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
        private void OnLoginResponse(object? _, bool successfulLogin)
        {
            if (successfulLogin)
            {
                Application.Current.Dispatcher.Invoke(() => Navigator.NavToSessionWindow());
                return;
            }

            MessageBox.Show("Wrong username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnStatReceived(object? _, Statistic stat)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SelectedPatient.Speed = stat.Speed;
                SelectedPatient.Distance = stat.Distance;
                SelectedPatient.HeartRate = stat.HeartRate;
                OnPropertyChanged(nameof(SelectedPatient));
            });
        }

        private void OnChatReceived(object? sender, string chatMessage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SelectedPatient.ChatMessages.Add($"{SelectedPatient.Name}: {chatMessage}");
                OnPropertyChanged(nameof(SelectedPatient));
            });
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
                    Name = "bob",
                    Speed = 1,
                    Distance = 1,
                    HeartRate = 1,
                    ChatMessages = new ObservableCollection<string> { "Bob: bro stop ik krijg hartaanval", "Bob: fdsfdsfdsf", "Bob: dfsdffdfsdf" },
                    PatientDataCollection = new List<PatientData> {
                        new PatientData
                        {
                            DateTime = DateTime.Now,
                            RecordedSpeed = 1,
                            RecordedDistance = 1,
                            RecordedHeartRate = 1,
                            RecordedRrInterval = 1,
                        },
                        new PatientData
                        {
                            DateTime = DateTime.Now,
                            RecordedSpeed = 1,
                            RecordedDistance = 1,
                            RecordedHeartRate = 1,
                            RecordedRrInterval = 1,
                        },
                    }

                },

                new Patient
                {
                    Name = "jan",
                    Speed = 2,
                    Distance = 5,
                    HeartRate = 3,
                    ChatMessages = new ObservableCollection<string> { "Jan: wanneer beginnen we?" }
                }
            };

            OnPropertyChanged(nameof(Patients));
        }
    }
}
