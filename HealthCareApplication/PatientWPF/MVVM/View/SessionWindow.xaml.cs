﻿using Newtonsoft.Json.Linq;
using PatientApp.DeviceConnection;
using PatientWPFApp.PatientLogic;
using System.Windows;
using System.Windows.Media;

namespace PatientWPFApp.View
{
    /// <summary>
    /// Interaction logic for SessionWindow.xaml
    /// </summary>
    public partial class SessionWindow : Window
    {
        private bool _sessionActive = false;

        public SessionWindow(string patientName)
        {
            InitializeComponent();

            PatientNameText.Text = patientName;

            // Initialize BLE connection
            DeviceManager.Initialize().Wait();
        }

        private async void ToggleSessionButton_Click(object sender, RoutedEventArgs e)
        {
            JObject message;
            if (_sessionActive)
            {
                // Stop the session
                DeviceManager.OnReceiveData -= OnReceiveData;

                message = PatientFormat.SessionStopMessage();

                _sessionActive = false;
                ToggleSessionButton.Content = "Start session";
                ToggleSessionButton.Background = Brushes.LightGreen;

                SessionStatusText.Text = "Session stopped. Click the 'Start session' button to start a new training.";
                SessionStatusText.Background = Brushes.Azure;

                EmergencyButton.IsEnabled = false;
            }
            else
            {
                // Start a new session
                DeviceManager.OnReceiveData += OnReceiveData;

                message = PatientFormat.SessionStartMessage();

                _sessionActive = true;
                ToggleSessionButton.Content = "Stop session";
                ToggleSessionButton.Background = Brushes.Salmon;

                SessionStatusText.Text = "Training is in progress.";
                SessionStatusText.Background = Brushes.LightSalmon;

                EmergencyButton.IsEnabled = true;
            }
            await ClientConn.SendJson(message);
        }

        private void EmergencyButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetResistanceButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopExitButton_Click(object sender, RoutedEventArgs e)
        {
            JObject stopMessage = PatientFormat.SessionStopMessage();
            ClientConn.SendJson(stopMessage).Wait();

            // TODO fix the server side issue of throwing an exception when the connection closes.
            ClientConn.CloseConnection();
            Close();
        }

        private void OnReceiveData(object _, Statistic stat)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CurrentSpeedText.Text = stat.Speed.ToString();

                int oldDistance = int.Parse(CurrentDistanceText.Text);
                CurrentDistanceText.Text = (oldDistance + stat.Distance).ToString();

                CurrentHeartRateText.Text = stat.HeartRate.ToString();
            });
        }
    }
}
