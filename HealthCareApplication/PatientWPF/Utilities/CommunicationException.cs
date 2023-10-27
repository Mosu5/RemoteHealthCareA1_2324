using System;
using System.Collections.Generic;
using System.Text;

namespace PatientWPF.Utilities
{
    [Serializable]
    internal class CommunicationException : Exception
    {
        public CommunicationException(string message) : base(message) { }
    }
}
