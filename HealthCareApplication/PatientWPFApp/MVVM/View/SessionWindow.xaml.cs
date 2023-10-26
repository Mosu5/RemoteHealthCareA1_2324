using PatientWPFApp.PatientLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PatientWPFApp.View
{
    /// <summary>
    /// Interaction logic for SessionWindow.xaml
    /// </summary>
    public partial class SessionWindow : Window
    {
        private bool _sessionActive = false;

        public SessionWindow()
        {
            InitializeComponent();
        }

        private async void ToggleSessionButton_Click(object sender, RoutedEventArgs e)
        {
            JsonObject message;
            if (_sessionActive)
            {
                // Stop the session
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
            JsonObject stopMessage = PatientFormat.SessionStopMessage();
            ClientConn.SendJson(stopMessage).Wait();

            // TODO fix the server side issue of throwing an exception when the connection closes.
            ClientConn.CloseConnection();
            Close();
        }

        private void SendChatButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
