using System.Windows;

namespace PatientWPFApp.View
{
    public partial class ChatP : Window
    {
        public ChatP()
        {
            InitializeComponent();
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            SessionSettingP sessionSetting = new SessionSettingP();
            Close();
            sessionSetting.Show();
        }

        private void sendBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
