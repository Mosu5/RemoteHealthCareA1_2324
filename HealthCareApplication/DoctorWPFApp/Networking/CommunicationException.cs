using System;

namespace DoctorWPFApp.Networking
{
    [Serializable]
    internal class CommunicationException : Exception
    {
        public CommunicationException(string message) : base(message) { }
    }
}
