using System.Windows;

namespace UIApp.View
{
    public partial class SessionSetting : Window
    {
        public SessionSetting()
        {
            InitializeComponent();
        }

        private void chatsBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        //todo data required
        private void summaryBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Averege speed: " + "\n" + "Distance: " + "\n" + "Average heartrate: ", "Summary so far", MessageBoxButton.OK);
        }

        private void stopBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
