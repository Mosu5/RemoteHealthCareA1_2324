using DoctorWPFApp.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace DoctorWPFApp.MVVM.View
{
    /// <summary>
    /// Logic for the login window with username and password fields
    /// </summary>
    public partial class LoginWindowD : Window
    {
        public LoginWindowD()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Create a new MainWindowViewModel for DataContext. This can
            // only happen in the window which the UI starts up with. In
            // this case, it's the login window.
            DataContext = new MainWindowViewModel();
            Navigator.CurrentWindow = this;
        }
    }
}
