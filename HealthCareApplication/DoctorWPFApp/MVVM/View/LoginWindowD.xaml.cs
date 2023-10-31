using System.Windows;

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

            // Create a new DoctorViewModel for DataContext. This can
            // only happen in the window which the UI starts up with. In
            // this case, it's the login window.
            Navigator.CurrentWindow = this;
        }
    }
}
