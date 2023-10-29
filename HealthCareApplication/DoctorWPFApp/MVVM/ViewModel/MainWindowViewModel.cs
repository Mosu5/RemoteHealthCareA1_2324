using DoctorWPFApp.MVVM.Model;
using DoctorWPFApp.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
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

            SessionButtonText = "Start";
            SessionButtonColor = Brushes.LightGreen;
            EmergencyBreakEnabled = "False";
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

        public RelayCommand SendChatCommand => new(async (execute) =>
        {
            if (string.IsNullOrEmpty(_messageToSend)) return;

            SelectedPatient.ChatMessages.Add($"You: {_messageToSend}");
            OnPropertyChanged(nameof(SelectedPatient.ChatMessages));

            JsonObject chatToServer = DoctorFormat.ChatsSendMessage(_messageToSend, SelectedPatient.Name);
            await ClientConn.SendJson(chatToServer);

            MessageToSend = "";
        });

        private string _sessionButtonText;
        public string SessionButtonText
        {
            get { return _sessionButtonText; }
            set
            {
                _sessionButtonText = value;
                OnPropertyChanged(nameof(SessionButtonText));
            }
        }

        private SolidColorBrush _sessionButtonColor;
        public SolidColorBrush SessionButtonColor
        {
            get { return _sessionButtonColor; }
            set
            {
                _sessionButtonColor = value;
                OnPropertyChanged(nameof(SessionButtonColor));
            }
        }

        private string _statusText;
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        private string _emergencyBreakEnabled;
        public string EmergencyBreakEnabled
        {
            get { return _emergencyBreakEnabled; }
            set
            {
                _emergencyBreakEnabled = value;
                OnPropertyChanged(nameof(EmergencyBreakEnabled));
            }
        }

        private bool _sessionActive = false;
        public RelayCommand ToggleSessionCommand => new(async (execute) =>
        {
            JsonObject message;
            if (_sessionActive)
            {
                // Stop the session
                message = DoctorFormat.SessionStopMessage(SelectedPatient.Name);

                _sessionActive = false;
                SessionButtonText = "Start";
                SessionButtonColor = Brushes.LightGreen;
                StatusText = "No active session for current patient.";
                EmergencyBreakEnabled = "False";
            }
            else
            {
                // Start a new session
                message = DoctorFormat.SessionStartMessage(SelectedPatient.Name);

                _sessionActive = true;
                SessionButtonText = "Stop";
                SessionButtonColor = Brushes.Salmon;
                StatusText = "Training is in progress for current patient.";
                EmergencyBreakEnabled = "True";
            }
            await ClientConn.SendJson(message);
        });

        public RelayCommand EmergencyBreak => new RelayCommand(async (execute) =>
        {
            JsonObject sessionStop = DoctorFormat.SessionStopMessage(SelectedPatient.Name);
            JsonObject chatSend = DoctorFormat.ChatsSendMessage($"\t<<ACTIVATED THE EMERGENCY BREAK!>>", SelectedPatient.Name);

            await ClientConn.SendJson(sessionStop);

            // Introducte a small delay, because the server otherwise cannot handle the chats/send below
            await Task.Delay(1000);

            await ClientConn.SendJson(chatSend);
        });

        private string _trainerResistance;
        public string TrainerResistance
        {
            get { return _trainerResistance; }
            set
            {
                _trainerResistance = value;
                OnPropertyChanged(nameof(TrainerResistance));
            }
        }

        public RelayCommand SetResistance => new((execute) =>
        {
            if (string.IsNullOrEmpty(TrainerResistance)) return;

            int resistanceAsInt = int.Parse(_trainerResistance);
            if (resistanceAsInt < 0 || resistanceAsInt > 100) return;

            // TODO send network message for setting resistance

            TrainerResistance = "";
        });

        public RelayCommand StopExitCommand => new(async (execute) =>
        {
            JsonObject stopMessage = DoctorFormat.SessionStopMessage(SelectedPatient.Name);
            await ClientConn.SendJson(stopMessage);

            // TODO fix the server side issue of throwing an exception when the connection closes.
            ClientConn.CloseConnection();
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

            SelectedPatient = Patients[0];
            OnPropertyChanged(nameof(SelectedPatient));
        }
    }
}
