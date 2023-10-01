using System;
using System.Text;
using System.Text.Json.Nodes;
using Utilities.Communication;

namespace PatientApp.Commands
{
    public class Summary : ISessionCommand
    {
        private readonly JsonObject _dataObject;

        public Summary(JsonObject dataObject)
        {
            _dataObject = dataObject;
        }

        /// <summary>
        /// Prints out a summary of all statistics that were received by the server.
        /// </summary>
        public void Execute()
        {
            StringBuilder sb = new StringBuilder($"==== SUMMARY OF STATISTICS ====\n\tAmount:\t{_dataObject.Count} statistics\n");

            // Check for statistics field
            if (!_dataObject.ContainsKey("statistics"))
                throw new CommunicationException("The message did not contain the JSON key 'statistics'");

            JsonArray statsArray = _dataObject["statistics"].AsArray();

            // Loop through array of the objects containing the statistics
            foreach (var statAsNode in statsArray)
            {
                JsonObject statAsObject = statAsNode.AsObject();
                if (!IsStatValid(statAsObject)) continue;

                int index = statsArray.IndexOf(statAsNode);

                // Append statistics as string
                sb.Append($"\n{index}.\n");
                sb.Append($"\tTime:\t{statAsObject["time"]}\n\tSpeed:\t{statAsObject["speed"]}\n\tDistance:\t{statAsObject["distance"]}\n\tHeart rate:\t{statAsObject["heartrate"]}\n");
            }

            sb.Append("==== END SUMMARY OF STATISTICS ====");
            Console.WriteLine(sb.ToString());
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
