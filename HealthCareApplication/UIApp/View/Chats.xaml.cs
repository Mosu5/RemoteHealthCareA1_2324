using System.Windows;

namespace UIApp.View
{
    public partial class Chats : Window
    {
        public Chats()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SessionSetting sessionSetting = new SessionSetting();
            Close();
            sessionSetting.Show();
        }
    }
}
