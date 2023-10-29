﻿using System;
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
