using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    [Serializable]
    public class SummaryStats
    {
        // Testing, so multiple stats can be sent through 1 JSON Object
        // Serializing a list of objects is almost impossible so we serialize 1 object containing a list

        public List<UserStat> stats { get; set; }

        public SummaryStats(List<UserStat> stats) { this.stats = stats; }

    }
}
