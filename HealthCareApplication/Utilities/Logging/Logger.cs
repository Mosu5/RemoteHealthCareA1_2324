using System;
using System.Collections.Generic;

namespace Utilities.Logging
{
    /// <summary>
    /// Console logging class that makes it easy to separate and/or ignore certain types of logging,
    /// to prevent cluttering of the console.
    /// </summary>
    public class Logger
    {
        // Make sure that all log types are added in this list!!
        private static List<LogType> _typesToLogFor = new List<LogType>
        {
            LogType.GeneralInfo,
            LogType.DeviceInfo,
            LogType.CommunicationExceptionInfo,
            LogType.Warning,
            LogType.Error,
            LogType.Debug
        };

        /// <summary>
        /// Writes a line to the console with the specified log type
        /// </summary>
        public static void Log(string message, LogType logType)
        {
            if (_typesToLogFor.Contains(logType))
                Console.WriteLine($"<<{logType}>>\t{message}");
        }

        /// <summary>
        /// Configures this class to only log certain log types, and ignore the rest of the log types.
        /// Listens to all log types by default.
        /// </summary>
        /// <param name="logTypes">The log types that are allowed to be logged</param>
        public static void SetTypesToLogFor(params LogType[] logTypes)
        {
            _typesToLogFor.Clear();

            foreach (LogType type in logTypes)
                _typesToLogFor.Add(type);
        }
    }
}
