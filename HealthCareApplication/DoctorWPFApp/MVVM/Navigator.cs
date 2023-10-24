using System.Windows;
using DoctorWPFApp.MVVM.View;

namespace DoctorWPFApp.MVVM
{
    /// <summary>
    /// Handles the navigation and DataContext sharing between views
    /// </summary>
    public static class Navigator
    {
        public static Window CurrentWindow = null!;

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
            CurrentWindow.Show();
        }

        public static void NavToSessionWindow()
        {
            SessionWindowD sessionWindow = new();
            NavToWindow(sessionWindow);
        }

        public static void NavToStatWindow()
        {
            StatsWindowD statsWindow = new();
            NavToWindow(statsWindow);
        }

        public static void NavToChatWindow()
        {
            ChatWindowD chatWindow = new();
            NavToWindow(chatWindow);
        }
    }
}
