using System.Windows;

namespace DoctorWPFApp.View
{
    public partial class SessionSettingD : Window
    {
        public bool isRunning = false;
        public SessionSettingD()
        {
            InitializeComponent();
        }

        private void chatsBtn_Click(object sender, RoutedEventArgs e)
        {
            ChatsD chats = new ChatsD();
            Close();
            chats.Show();
        }

        private void statsBtn_Click(object sender, RoutedEventArgs e)
        {
            StatsD stats = new StatsD();
            Close();
            stats.Show();
        }

        private void stopstartBtn_Click(object sender, RoutedEventArgs e)
        {
            
            if(isRunning == true)
            {
                isRunning = false;
                stopstartBtn.Content = "Start";
                stopstartBtn.Background = System.Windows.Media.Brushes.LightGreen;
            }
            else
            {
                isRunning = true;
                stopstartBtn.Content = "Stop";
                stopstartBtn.Background = System.Windows.Media.Brushes.Red;
            }
            
        }
    }
}
