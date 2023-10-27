using Newtonsoft.Json.Linq;
using PatientApp.DeviceConnection;
using PatientApp.VrLogic;
using PatientWPFApp.PatientLogic;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
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
            JObject loginRequest = PatientFormat.LoginMessage(_username, _password);
            // TODO fix server side issue of exception when incorrect credentials
            await PatientLogic.ClientConn.SendJson(loginRequest);
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

            JObject chatToServer = PatientFormat.ChatsSendMessage(_messageToSend);
            await ClientConn.SendJson(chatToServer);
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

            // Initialize BLE connection
            Thread deviceThread = new Thread(async () => await DeviceManager.Initialize());
            //deviceThread.Start();
            //deviceThread.Join();

            // Initialize VR environment
            Thread vrThread = new Thread(() => VrProgram.Initialize().Wait());
            //vrThread.Start();
            //vrThread.Join();

            //// Listen for requests
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
                Application.Current.Dispatcher.Invoke(() => Navigator.NavToSessionWindow());
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
