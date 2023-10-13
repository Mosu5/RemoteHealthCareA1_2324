using DoctorWPFApp.MVVM.ViewModel;
using System.Windows;

namespace DoctorWPFApp.MVVM.View
{

    public partial class StatsWindowD : Window
    {
        public StatsWindowD()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            SessionWindowD sessionSetting = new SessionWindowD();
            Hide();
            sessionSetting.Show();
        }

        private void summaryBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Averege speed: " + "\n" + "Distance: " + "\n" + "Average heartrate: ", "Summary so far!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
