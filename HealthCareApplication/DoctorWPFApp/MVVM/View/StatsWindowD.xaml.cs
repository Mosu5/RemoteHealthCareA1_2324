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
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            Navigator.NavToSessionWindow();
        }

        private void SummaryBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Average speed: " + "\n" + "Distance: " + "\n" + "Average heartrate: ", "Summary so far!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ListView_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }
    }
}
