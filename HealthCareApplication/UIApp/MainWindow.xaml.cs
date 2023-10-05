using System.Windows;
using UIApp.View;

namespace UIApp
{
   
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (wwBox.Password.ToString() == "sexy" && gbBox.Text.ToString() == "super")
            {
                SessionSetting sessionSetting = new SessionSetting();
                Close();
                sessionSetting.Show();
            }
            else
            {
                MessageBox.Show("Wrong username or password.", "ERROR!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        
    }
}
