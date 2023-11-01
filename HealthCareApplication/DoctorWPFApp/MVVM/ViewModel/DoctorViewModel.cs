using DoctorWPFApp.MVVM.Model;
using DoctorWPFApp.Networking;
using Newtonsoft.Json;
using PatientWPF.Utilities.Encryption;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DoctorWPFApp.MVVM.ViewModel
{
    /// <summary>
    /// Handles the data between the views and the server-client logic
    /// </summary>
    internal class DoctorViewModel : ViewModelBase
    {
        public DoctorViewModel()
        {
            Thread listenerThread = new(async () => await RequestHandler.Listen());
            listenerThread.Start();

            // Subscribe to response event handler
            RequestHandler.LoggedIn += OnLoginResponse;
            RequestHandler.ReceivedStat += OnStatReceived;
            RequestHandler.ReceivedChat += OnChatReceived;
            RequestHandler.SessionStarted += OnSessionStarted;
            RequestHandler.SessionStopped += OnSessionStopped;
            RequestHandler.ReceivedSummary += OnSummaryReceived;
            RequestHandler.ReceivedPatients += OnPatientsReceived;

            SessionButtonText = "Start";
            SessionButtonColor = Brushes.LightGreen;
            EmergencyBreakEnabled = "False";
            StatusText = "No active session for current patient.";
            StatusTextColor = Brushes.Azure;
        }

        #region PatientData
        // Contains patient data of currently selected patient
        // Session Window will update shown data based on this property
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

        // Contains a list of currently registered patients
        // On login, the application will receive this data and update the 
        public ObservableCollection<Patient> Patients { get; set; } = new ObservableCollection<Patient>();
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

        private bool _sessionActive = false;

        private bool _isSessionRunning = false;

        private string? _trainerResistance;
        public string? TrainerResistance
        {
            get { return _trainerResistance; }
            set
            {
                _trainerResistance = value;
                OnPropertyChanged(nameof(TrainerResistance));
            }
        }

        private string? _messageToSend;
        public string? MessageToSend
        {
            get { return _messageToSend; }
            set
            {
                _messageToSend = value;
                OnPropertyChanged(nameof(MessageToSend));
            }
        }


        private string? _sessionButtonText;
        public string? SessionButtonText
        {
            get { return _sessionButtonText; }
            set
            {
                _sessionButtonText = value;
                OnPropertyChanged(nameof(SessionButtonText));
            }
        }

        private SolidColorBrush? _sessionButtonColor;
        public SolidColorBrush? SessionButtonColor
        {
            get { return _sessionButtonColor; }
            set
            {
                _sessionButtonColor = value;
                OnPropertyChanged(nameof(SessionButtonColor));
            }
        }

        private SolidColorBrush? _statusTextColor;
        public SolidColorBrush? StatusTextColor
        {
            get { return _statusTextColor; }
            set
            {
                _statusTextColor = value;
                OnPropertyChanged(nameof(StatusTextColor));
            }
        }

        private string? _statusText;
        public string? StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        private string? _emergencyBreakEnabled;
        public string? EmergencyBreakEnabled
        {
            get { return _emergencyBreakEnabled; }
            set
            {
                _emergencyBreakEnabled = value;
                OnPropertyChanged(nameof(EmergencyBreakEnabled));
            }
        }
        #endregion

        #region Commands called by the UI
        public RelayCommand LoginCommand => new(async (execute) =>
        {

            string hashedPass = Encryption.ComputeSha256Hash(_password);

            // Send the login command
            JsonObject loginRequest = DoctorFormat.LoginMessage(_username, hashedPass);
            await ClientConn.SendJson(loginRequest);

        }, canExecute => !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password)); // Checks if fields are not null or empty
        public RelayCommand GetPatientListCommand => new(async (execute) =>
        {
            JsonObject GetPatientRequest = DoctorFormat.GetPatientsMessage();
            await ClientConn.SendJson(GetPatientRequest);
        });
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
        public RelayCommand GetSummaryCommand => new(
        async (execute) =>
        {
            JsonObject summaryRequest = DoctorFormat.StatsSummaryMessage(SelectedPatient.Name);
            await ClientConn.SendJson(summaryRequest);
        }, canExecute => !_isSessionRunning
        );
        public RelayCommand SendChatCommand => new(async (execute) =>
        {
            if (string.IsNullOrEmpty(_messageToSend)) return;

            SelectedPatient.ChatMessages.Add($"You: {_messageToSend}");
            OnPropertyChanged(nameof(SelectedPatient.ChatMessages));

            JsonObject chatToServer = DoctorFormat.ChatsSendMessage(_messageToSend, SelectedPatient.Name);
            await ClientConn.SendJson(chatToServer);

            MessageToSend = "";
        });
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
                StatusTextColor = Brushes.Azure;
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
                StatusTextColor = Brushes.LightSalmon;
            }
            await ClientConn.SendJson(message);
        });
        public RelayCommand EmergencyBreak => new RelayCommand(async (execute) =>
        {
            EmergencyBreakEnabled = "False";
            SelectedPatient.ChatMessages.Add($"<<You activated the emergency break.>>");
            OnPropertyChanged(nameof(SelectedPatient.ChatMessages));

            JsonObject sessionStop = DoctorFormat.SessionStopMessage(SelectedPatient.Name);
            JsonObject chatSend = DoctorFormat.ChatsSendMessage($"\t<<ACTIVATED THE EMERGENCY BREAK!>>", SelectedPatient.Name);

            _sessionActive = false;
            SessionButtonText = "Start";
            SessionButtonColor = Brushes.LightGreen;
            StatusText = "No active session for current patient.";
            EmergencyBreakEnabled = "False";
            StatusTextColor = Brushes.Azure;

            await ClientConn.SendJson(sessionStop);

            // Introducte a small delay, because the server otherwise cannot handle the chats/send below
            await Task.Delay(1000);

            await ClientConn.SendJson(chatSend);
        });
        public RelayCommand SetResistance => new(async (execute) =>
        {
            if (string.IsNullOrEmpty(TrainerResistance))
            {
                TrainerResistance = "";
                return;
            }

            int resistanceAsInt;
            try
            {
                resistanceAsInt = int.Parse(_trainerResistance);
            }
            catch (FormatException)
            {
                TrainerResistance = "";
                return;
            }
            if (resistanceAsInt < 0 || resistanceAsInt > 100)
            {
                TrainerResistance = "";
                return;
            }

            await ClientConn.SendJson(DoctorFormat.SetResistanceMessage(resistanceAsInt, SelectedPatient.Name));

            TrainerResistance = "";
        });
        public RelayCommand StopExitCommand => new((execute) =>
        {
            Thread t = new Thread(() =>
            {
                JsonObject stopMessage = DoctorFormat.SessionStopMessage(SelectedPatient.Name);
                ClientConn.SendJson(stopMessage).Wait();
                ClientConn.CloseConnection();
            });
            t.Start();
            t.Join();
        });
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
        /// <summary>
        /// Update the patient list to enable the dokter to request patient data from server
        /// 
        /// When the list of currently registered patients is received from the server,
        /// check if there already is a patient with the same name, to avoid redudant data
        /// </summary>
        /// <param name="sender"> object that triggered the event </param>
        /// <param name="usernames"> list of usernames to update patient with </param>
        private void OnPatientsReceived(object? sender, string[] usernames)
        {
            if (usernames is null) return;

            foreach (var name in usernames)
            {
                if (!Patients.Any(pat => pat.Name == name))
                {
                    // Update UI elements
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Patients.Add(new Patient { Name = name });
                        OnPropertyChanged(nameof(Patients));
                    });

                }
            }

            // Set default value for patients
            SelectedPatient = Patients[0];
        }
        /// <summary>
        /// Update the real-time data of the currently displayed patient
        /// 
        /// When trainer data is received, update patient and UI accordingly
        /// </summary>
        /// <param name="_"> object that triggered the event </param>
        /// <param name="stat"> patient statistics received from server </param>
        private void OnStatReceived(object? _, Statistic stat)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SelectedPatient.Speed = Math.Round(stat.Speed * 3.6, 1);
                SelectedPatient.Distance = stat.Distance;
                SelectedPatient.HeartRate = stat.HeartRate;
                OnPropertyChanged(nameof(SelectedPatient));
            });
        }
        /// <summary>
        ///  Update the patient summary based on all historical data saved on this patient received from the server
        /// </summary>
        /// <param name="sender"> object that the triggered the event </param>
        /// <param name="json"> patient summary received from server </param>
        private void OnSummaryReceived(object? sender, string json)
        {
            var patientDataList = JsonConvert.DeserializeObject<List<PatientData>>(json);
            Application.Current.Dispatcher.Invoke(() =>
            {
                SelectedPatient.PatientDataCollection = patientDataList != null ? patientDataList : new();

                patientDataList?.ForEach(patientData => patientData.Speed = Math.Round(patientData.Speed, 1));
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
        private void OnSessionStarted(object? sender, bool sessionStopped)
        {
            // Method gets called on a different thread than the current UI thread.
            // Therefore invoke this method within a lambda to make it possible
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_sessionActive) return;

                // Start a new session
                _sessionActive = true;
                SessionButtonText = "Stop";
                SessionButtonColor = Brushes.Salmon;
                EmergencyBreakEnabled = "True";
                StatusText = "Training is in progress for current patient.";
                StatusTextColor = Brushes.LightSalmon;
            });
        }
        private void OnSessionStopped(object? sender, bool sessionStopped)
        {
            // Method gets called on a different thread than the current UI thread.
            // Therefore invoke this method within a lambda to make it possible
            Application.Current.Dispatcher.Invoke(async () =>
            {
                if (!_sessionActive) return;

                // Stop the session
                _sessionActive = false;

                SessionButtonText = "Start";
                SessionButtonColor = Brushes.LightGreen;
                EmergencyBreakEnabled = "False";
                StatusText = "No active session for current patient.";
                StatusTextColor = Brushes.Azure;

                JsonObject summaryRequest = DoctorFormat.StatsSummaryMessage(SelectedPatient.Name);
                await ClientConn.SendJson(summaryRequest);
            });
        }
        #endregion
    }
}