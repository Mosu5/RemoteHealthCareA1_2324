using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Communication
{
    [Serializable]
    internal class CommunicationException : Exception
    {
        public CommunicationException(string message) : base(message) { }
    }
}
