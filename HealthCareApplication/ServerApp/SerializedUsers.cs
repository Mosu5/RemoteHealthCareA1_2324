using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{

    [Serializable]
    public class SerializedUsers
    {
        public List<string> usernames { get; set; }

        public SerializedUsers(List<string> names)
        {

            usernames = names;

        }

    }
}
