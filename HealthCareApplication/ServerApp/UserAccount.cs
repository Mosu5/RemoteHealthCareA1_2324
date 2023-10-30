using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utilities.Logging;
using System.Text.Json;

namespace ServerApp
{
    public class UserAccount
    {

        private readonly string _username;
        private readonly string _password;

        public bool HasActiveSession { get; set; }
        public bool IsPaused { get; set; }
        public bool IsDoctor { get; set; }
        public TcpClient UserClient { get; set; }

        public UserAccount(string username, string password) 
        {
            _username = username;
            _password = password;
            HasActiveSession = false;
            IsPaused = false;
        }


        public void SaveUserStats(string jsonData)
        {
            JsonElement root;
            using (JsonDocument document = JsonDocument.Parse(jsonData))
            {
                // Access the root element of the JSON document
                root = document.RootElement;
            }

            string runTimeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string correctPath = Path.Combine(runTimeDirectory, this._username + @"-stats.json");

            StreamWriter writer;
            if (!File.Exists(correctPath))
            {
                writer = new StreamWriter(correctPath);
            }
            else
            {
                writer = File.AppendText(correctPath);
            }

            writer.WriteLine(jsonData); // Write the session data as a separate line
            writer.Flush();
            writer.Close();

            Console.WriteLine("Userdata has been saved into file!");
        }
        public List<UserStat> GetUserStats()
        {
            string runTimeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string correctPath = Path.Combine(runTimeDirectory, this._username + @"-stats.json");

            if (!File.Exists(correctPath))
            {
                return null;
            }

            List<UserStat> data = new List<UserStat>();

            foreach (var line in File.ReadLines(correctPath))
            {
                var sessionData = JsonSerializer.Deserialize<List<UserStat>>(line);
                data.AddRange(sessionData);
            }

            return data;
        }

        public string GetUserName() 
        { 
            return _username;
        }

        public string GetPassword()
        {
            return _password;
        }

    }
}
