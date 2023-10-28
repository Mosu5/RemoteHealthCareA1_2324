using PatientWPF.MVVM.View;
using PatientWPFApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PatientWPFApp.MVVM.ViewModel
{
    /// <summary>
    /// Handles the navigation and DataContext sharing between views
    /// </summary>
    public static class Navigator
    {
        public static Window CurrentWindow = null;

        /// <summary>
        /// Closes previous window and opens the next.
        /// </summary>
        private static void NavToWindow(Window window)
        {
            // Null check
            if (CurrentWindow == null) return;

            // Close current window
            window.DataContext = CurrentWindow.DataContext;
            CurrentWindow.Close();

            // Open next window
            CurrentWindow = window;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            CurrentWindow.Show();
        }

        public static void NavToSessionWindow(string patientName)
        {
            SessionWindow sessionWindow = new SessionWindow(patientName);
            NavToWindow(sessionWindow);
        }
    }
}
