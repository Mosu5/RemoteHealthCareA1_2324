using System;
using System.Text;
using System.Text.Json.Nodes;

namespace PatientApp.Commands
{
    internal class Summary : ISessionCommand
    {
        /// <summary>
        /// Currently prints out a summary of all statistics that were received by the server.
        /// </summary>
        public bool Execute(JsonObject data)
        {
            JsonArray dataArray = data.AsArray();
            StringBuilder sb = new StringBuilder($"==== SUMMARY OF STATISTICS ====\n\tAmount:\t{dataArray.Count} statistics\n");

            // Loop through array of the objects containing the statistics
            foreach (var statAsNode in dataArray)
            {
                JsonObject statAsObject = statAsNode.AsObject();
                if (!IsStatValid(statAsObject)) continue;

                int index = dataArray.IndexOf(statAsNode);

                // Append statistics as string
                sb.Append($"\n{index}.\n");
                sb.Append($"\tTime:\t{statAsObject["time"]}\n\tSpeed:\t{statAsObject["speed"]}\n\tDistance:\t{statAsObject["distance"]}\n\tHeart rate:\t{statAsObject["heartrate"]}\n");
            }

            sb.Append("==== END SUMMARY OF STATISTICS ====");
            Console.WriteLine(sb.ToString());

            return true;
        }

        /// <summary>
        /// Wether the given statistics object contains the correct keys
        /// </summary>
        private bool IsStatValid(JsonObject stat)
        {
            return stat.ContainsKey("time") &&
                stat.ContainsKey("speed") &&
                stat.ContainsKey("distance") &&
                stat.ContainsKey("heartrate");
        }
    }
}
