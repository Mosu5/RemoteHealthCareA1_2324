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

        private string _username { get; set; }
        private string _password { get; set; }
        public bool hasActiveSession { get; set; }
        public bool isPaused { get; set; }
        public bool isDoctor { get; set; }

        public UserAccount(string username, string password) 
        {
            this._username = username;
            this._password = password;
            this.hasActiveSession = false;
            this.isPaused = false;
        }


        public void SaveUserStats(string jsonData)
        {

            // NOTE: The following path will point to the bin/debug folder of the project serverapp.
            string runTimeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string correctPath = Path.Combine(runTimeDirectory, this._username + @"-stats.json");

            StreamWriter writer = new StreamWriter(correctPath);
            
            if (jsonData != null )
            {
                
                writer.Write(jsonData);
                writer.Flush();
                writer.Close();
                Console.WriteLine("Userdata has been saved into file!");
            }
            else
            {
                Logger.Log("JSONdata has not been saved into file. Check filepath or payload!", LogType.Warning);
                Console.WriteLine("Data has not been saved!");
            }
        }

        public List<List<UserStat>> GetUserStats()
        {
            string runTimeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string correctPath = Path.Combine(runTimeDirectory, this._username + @"-stats.json");

            StreamReader reader = new StreamReader(correctPath);

            string jsonData = reader.ReadToEnd();
            reader.Close();
            
            try
            {
                List<List<UserStat>> retrievedUserStats = JsonSerializer.Deserialize<List<List<UserStat>>>(jsonData);
                return retrievedUserStats;
            }
            catch
            {
                return null;
            }
    

        }

        public bool IsDoctor()
        {
            return this.isDoctor;
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
