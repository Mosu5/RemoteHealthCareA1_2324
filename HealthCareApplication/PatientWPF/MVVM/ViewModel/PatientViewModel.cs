using Newtonsoft.Json.Linq;
using PatientApp.DeviceConnection;
using PatientApp.VrLogic;
using PatientWPF.Utilities.Encryption;
using PatientWPFApp.PatientLogic;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Utilities.Logging;

namespace PatientWPFApp.MVVM.ViewModel
{
    internal class PatientViewModel : ViewModelBase
    {
        private bool _isLoggedIn = false;

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
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public RelayCommand LoginCommand => new RelayCommand(async (execute) =>
        {
            // Send the login command
            string hashedPassword = Encryption.ComputeSha256Hash(_password);
            JObject loginRequest = PatientFormat.LoginMessage(_username, hashedPassword);

            // TODO fix server side issue of exception when incorrect credentials
            await ClientConn.SendJson(loginRequest);
        }, canExecute => !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password));

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

        public ObservableCollection<string> PatientChats { get; set; } = new ObservableCollection<string>();

        public RelayCommand SendChatCommand => new RelayCommand(async (execute) =>
        {
            if (string.IsNullOrEmpty(_messageToSend)) return;

            PatientChats.Add($"You: {_messageToSend}");
            OnPropertyChanged(nameof(PatientChats));

            JObject chatToServer = PatientFormat.ChatsSendMessage(_messageToSend);
            await ClientConn.SendJson(chatToServer);

            MessageToSend = "";
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

        public RelayCommand SetResistance => new RelayCommand((execute) =>
        {
            if (string.IsNullOrEmpty(TrainerResistance)) return;
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

            DeviceManager.Receiver.SetResistance(resistanceAsInt);
            TrainerResistance = "";
        });

        public RelayCommand EmergencyBreak => new RelayCommand(async (execute) =>
        {
            PatientChats.Add($"<<You activated the emergency break.>>");
            OnPropertyChanged(nameof(PatientChats));

            JObject sessionStop = PatientFormat.SessionStopMessage();
            JObject chatSend = PatientFormat.ChatsSendMessage($"\t<<ACTIVATED THE EMERGENCY BREAK!>>");

            await ClientConn.SendJson(sessionStop);

            // Introducte a small delay, because the server otherwise cannot handle the chats/send below
            await Task.Delay(1000);

            await ClientConn.SendJson(chatSend);
        });

        public PatientViewModel()
        {
            // Logger will log if LogType is present
            Logger.SetTypesToLogFor(
                LogType.GeneralInfo,
                LogType.DeviceInfo,
                LogType.VrInfo,
                LogType.CommunicationExceptionInfo,
                LogType.Warning,
                LogType.Error,
                LogType.Debug
            );

            // Thread t = new Thread(() =>
            //{

            //    // Initialize BLE connection
            //    DeviceManager.Initialize().Wait();
            //});
            //t.Start();

           


            // Initialize VR environment
            Thread vrThread = new Thread(() =>
            {
                try
                {
                    if (!VrProgram.Initialize().Result)
                        MessageBox.Show("Could not load VR environment.\nCheck wether you are running NetworkEngine (sim.bat)", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch(Exception e)
                {

                    throw e;
                    //// Connection with VR failed, too bad we keep on going lol
                    //MessageBox.Show("Failed to establish connection to VR...", "Big OOF", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    //return;
                }
               
            });
            vrThread.Start();
            vrThread.Join();

            // Listen for requests
            Thread listenerThread = new Thread(async () => await RequestHandler.Listen());
            listenerThread.Start();

            RequestHandler.LoggedIn += OnLoginResponse;
            RequestHandler.ReceivedChat += OnReceivedChat;
        }

        /// <summary>
        /// When a response is received from the server, give feedback to the user or switch window.
        /// </summary>
        private void OnLoginResponse(object sender, bool successfulLogin)
        {
            if (_isLoggedIn) return;

            if (successfulLogin)
            {
                _isLoggedIn = true;
                Application.Current.Dispatcher.Invoke(() => Navigator.NavToSessionWindow(_username));
                return;
            }

            MessageBox.Show("Wrong username or password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OnReceivedChat(object sender, string chatMessage)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PatientChats.Add(chatMessage);
                OnPropertyChanged(nameof(PatientChats));
            });
        }
    }
}
