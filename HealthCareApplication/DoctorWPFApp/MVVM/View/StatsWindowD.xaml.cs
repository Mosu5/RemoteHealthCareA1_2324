using System.Windows;

namespace DoctorWPFApp.MVVM.View
{

    public partial class StatsWindowD : Window
    {
        public StatsWindowD()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            SessionWindowD sessionSetting = new SessionWindowD();
            Close();
            sessionSetting.Show();
        }

        private void summaryBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Averege speed: " + "\n" + "Distance: " + "\n" + "Average heartrate: ", "Summary so far!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
