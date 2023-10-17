using System.Windows;
using DoctorWPFApp.MVVM.View;

namespace DoctorWPFApp.MVVM
{
    /// <summary>
    /// Handles the navigation and datacontext sharing between views
    /// </summary>
    public static class Navigator
    {
        public static Window CurrentWindow = null!;

        private static void NavToWindow(Window window)
        {
            if (CurrentWindow != null)
            {
               
                window.DataContext = CurrentWindow.DataContext;
                CurrentWindow.Close();
                CurrentWindow = window;
                CurrentWindow.Show();
            }
        }

        public static void NavToSessionWindow()
        {
            SessionWindowD sessionWindow = new SessionWindowD();
            NavToWindow(sessionWindow);
        }

        public static void NavToStatWindow()
        {
            StatsWindowD statsWindow = new StatsWindowD();
            NavToWindow(statsWindow);
        }

        public static void NavToChatWindow()
        {
            ChatWindowD chatWindow = new ChatWindowD();
            NavToWindow(chatWindow);
        }
    }
}
