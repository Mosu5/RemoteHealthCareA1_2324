namespace Utilities.Logging
{
    public enum LogType
    {
        // General informational messages
        GeneralInfo,

        // Messages concerning the trainer or hrm
        DeviceInfo,

        // Messages triggered by a CommunicationException
        CommunicationExceptionInfo,

        // General warning messages
        Warning,

        // General error messages
        Error,

        // Debugging messages
        Debug,
    }
}
