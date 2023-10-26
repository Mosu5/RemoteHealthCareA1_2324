using PatientApp.DeviceConnection;
using PatientApp.VrLogic;
using PatientWPFApp.PatientLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Utilities.Communication;
using Utilities.Logging;

namespace PatientWPFApp.MVVM.ViewModel
{
    internal class PatientViewModel : ViewModelBase
    {
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
            { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public RelayCommand LoginCommand => new(async (execute) =>
        {
            // Send the login command
            JsonObject loginRequest = PatientFormat.LoginMessage(_username, _password);
            await PatientLogic.ClientConn.SendJson(loginRequest);
        }, canExecute => !string.IsNullOrEmpty(_username) && !string.IsNullOrEmpty(_password));

        public PatientViewModel()
        {
            RequestHandler.LoggedIn += OnLoginResponse;

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
            Thread deviceThread = new(async () => await DeviceManager.Initialize());
            //deviceThread.Start();
            //deviceThread.Join();

            // Initialize VR environment
            VrProgram.Initialize().Wait();

            //// Listen for requests
            Thread listenerThread = new(async () => await RequestHandler.Listen());
            listenerThread.Start();

        }

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
    }
}
