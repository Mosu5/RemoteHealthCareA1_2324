using ServerApp.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Utilities.Logging;

namespace ServerApp
{

    /// <summary>
    /// Gets and sets data for patients according to the incoming payload values
    /// This class is purely a one-way hatch for data from the Doctor - to - server - to - Patient  
    /// </summary>
    public class DoctorHandler
    {
        public string userToRespondTo { get; set; }
        public JsonObject responseValue { get; set; }

        public bool Handle(JsonObject packet)
        {

            string command = JsonUtil.GetValueFromPacket(packet, "command").ToString();
            string userName = JsonUtil.GetValueFromPacket(packet, "data", "username").ToString();

            // Error handling
            if (command == null || userName == null)
            {
                string warningMsg = "Server recieved message, but command or username was NULL";
                Logger.Log(warningMsg, LogType.Warning);
                return false;
            }

            Console.WriteLine("Command recieved from doc: " + command);
            Console.WriteLine("Username: " + userName);

            this.userToRespondTo = userName;

            // Handle responsevalue according to the incoming command from the doctor client
            switch (command)
            {
                // Note: Data object in responsevalue is empty. Client expects a data field but doesnt expect any values. Donut Wurry Boudda Ting
                case "session/start":
                    responseValue = new JsonObject { { "command", "session/start" }, {"data", new JsonObject() } };
                    break;
                case "session/stop":
                    responseValue = new JsonObject { { "command", "session/stop" }, { "data", new JsonObject() } };
                    break;
                case "stats/summary":
                    responseValue = new JsonObject { { "command", "stats/summary" }, { "data", new JsonObject() } };
                    break;
                case "session/pause":
                    responseValue = new JsonObject { { "command", "session/pause" }, { "data", new JsonObject() } };
                    break;
                case "session/resume":
                    responseValue = new JsonObject { { "command", "session/resume" }, { "data", new JsonObject() } };
                    break;

            }
            return true;

        }



    }
}
