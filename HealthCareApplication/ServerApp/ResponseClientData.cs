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

        public static JsonObject GenerateSummaryRequest(string userStats)
        {
            return new JsonObject()
             {
                 {"command","stats/summary"},
                 {"data",userStats }
             };
        }


        public static JsonObject GenerateDoctorResponse(string command, JsonObject data, string username)
        {
            return new JsonObject()
                 {
                     {"command",command },
                     {"data",new JsonObject
                     {
                         {"stats", data },
                         {"username", username}
                     }}
                 };
        }

        public static JsonObject DoctorChatSendResponse(string message, string patientName)
        {
            return new JsonObject()
            {
                { "command", "chats/send" },
                { "data", new JsonObject()
                {
                    { "message", message },
                    { "username", patientName }
                } }
            };
        }

        internal static JsonObject GenerateSummaryRequest(List<UserStat> userStatsBuffer)
        {
            throw new NotImplementedException();
        }
    }

}
