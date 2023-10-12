using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ServerApp
{
    internal class ResponseClientData
    {

        public static JsonObject GenerateResponse(string command, JsonObject data = null, string status = null)
        {
            if (status != null)
            {
                return new JsonObject()
                 {
                     {"command",command },
                     {"data",new JsonObject
                     {
                         {"status", status}
                     }}
                 };
            }
            return new JsonObject()
             {
                 {"command",command },
                 {"data", data }
             };
        }

        public static JsonObject GenerateSummaryRequest(List<UserStat> userStats)
        {
             return new JsonObject()
             {
                 {"command","stats/summary"},
                 {"data",new JsonArray{userStats} }
             };
        }

    }
}
