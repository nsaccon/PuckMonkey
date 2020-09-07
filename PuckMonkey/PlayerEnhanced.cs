using Newtonsoft.Json;
using NHLStats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PuckMonkey
{
    [Serializable]
    public class PlayerEnhanced
    {

        public Player Player;

        public List<PlayerGameStats> GameStats;

        public HashSet<string> GamesPlayedIn;

        public SalaryData Salary;

        public PlayerEnhanced()
        {
            GameStats = new List<PlayerGameStats>();
            GamesPlayedIn = new HashSet<string>();
            Salary = new SalaryData();
        }

        public class SalaryData
        {
            public int DraftKings;
            public int Fanduel;
        }
    }
}
