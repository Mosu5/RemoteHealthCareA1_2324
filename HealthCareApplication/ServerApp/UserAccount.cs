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
using System.Globalization;

namespace ServerApp
{
    public class UserAccount
    {

        private string _username { get; set; }
        private string _password { get; set; }
        public bool hasActiveSession { get; set; }
        public bool isPaused { get; set; }

        public UserAccount(string username, string password) 
        {
            this._username = username;
            this._password = password;
            this.hasActiveSession = false;
            this.isPaused = false;
        }


        public void SaveUserts(string jsonData)
        {
            // NOTE: The following path will point to the bin/debug folder of the project serverapp.
            string runTimeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string correctPath = Path.Combine(runTimeDirectory, this._username + @"-stats.json");

            string jsonObject = JsonSerializer.Serialize(jsonData);

            StreamWriter writer;
            if (jsonObject != null)
            {
                if (!File.Exists(correctPath))
                {
                    File.Create(correctPath);
                    writer = new StreamWriter(correctPath);
                    writer.Write(jsonObject);
                    writer.Flush();
                    writer.Close();
                    Console.WriteLine("Userdata has been saved into file!");
                }
                else
                {
                    writer = File.AppendText(correctPath);
                    writer.Write(jsonObject);
                    writer.Flush();
                    writer.Close();
                    Console.WriteLine("Userdata has been saved into file!");
                }
            }
            else
            {
                Logger.Log("JSONdata has not been saved into file. Check filepath or payload!", LogType.Warning);
                Console.WriteLine("Data has not been saved!");
            }
        }

        public void SaveUserStats(string jsonData)
        {
            string runTimeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string correctPath = Path.Combine(runTimeDirectory, this._username + @"-stats.json");

            string wrappedData = $" {jsonData} ";

            File.WriteAllText(correctPath, wrappedData);

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

            string jsonData = File.ReadAllText(correctPath);

            var data = JsonSerializer.Deserialize<Dictionary<string, List<UserStat>>>(jsonData);

            if (data.TryGetValue("sessions", out List<UserStat> retrievedUserStats))
            {
                return retrievedUserStats;
            }

            return null;
        }


        public string GetUserName() 
        { 
            return this._username;
        }

        public string GetPassword()
        {
            return this._password;
        }
    }
}
