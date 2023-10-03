using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    public class UserAccount
    {

        private string _username { get; set; }
        private string _password { get; set; }
        public bool hasActiveSession { get; set; }

        public UserAccount(string username, string password) 
        {
            this._username = username;
            this._password = password;
            this.hasActiveSession = false;
        }


        // To Do: In the future we want to check duplicate files, and add data if file already exist, rather than overwriting
        public void SaveUserStats(string jsonData)
        {
            StreamWriter writer = new StreamWriter($"@{Environment.CurrentDirectory}/{this._username}-stats.json");
            if (jsonData != null )
            {
                writer.Write(jsonData);
            }
        }

        public string GetUserStats()
        {
            StreamReader reader = new StreamReader($"@{Environment.CurrentDirectory}/{this._username}-stats.json");
            
            string jsonData = reader.ReadToEnd();
            return jsonData;
        }



    }
}
