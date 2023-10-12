using System.Windows;

namespace DoctorWPFApp.View
{
  
    public partial class StatsD : Window
    {
        public StatsD()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            SessionSettingD sessionSetting = new SessionSettingD();
            Close();
            sessionSetting.Show();
        }

        private void summaryBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Averege speed: " + "\n" + "Distance: " + "\n" + "Average heartrate: ", "Summary so far!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
