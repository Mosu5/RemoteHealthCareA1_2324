using System.Windows;
using DoctorWPFApp.View;

namespace DoctorWPFApp
{
   
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if (gbBox.Text.ToString() == "super" && wwBox.Password.ToString() == "sexy")
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
