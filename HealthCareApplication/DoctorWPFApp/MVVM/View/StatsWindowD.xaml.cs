using DoctorWPFApp.Networking;
using System.Windows;

namespace DoctorWPFApp.MVVM.View
{
    /// <summary>
    /// Window showcasing data of the selected patient
    /// </summary>
    public partial class StatsWindowD : Window
    {
        public StatsWindowD()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            RequestHandler.ReceivedSummary += OnSummaryReceived;

            // Send summary request
            // TODO this doesnt work yet because of a server issue, wait till this is fixed
            //ClientConn.SendJson(DoctorFormat.StatsSummaryMessage()).Wait();
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigator.NavToSessionWindow();
        }

        private void SummaryBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Average speed: " + "\n" + "Distance: " + "\n" + "Average heartrate: ", "Summary so far!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnSummaryReceived(object? sender, string summary)
        {
            //TODO
            //MessageBox.Show(summary);
        }

        private void ListView_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
