using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace PuckMonkey
{
    public class DraftKingsFile
    {
        private string SAVE_PATH = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\Files\");

        private const string BASE_URL = @"https://www.draftkings.com";
        private const string GETCONTESTS_PATH = @"/lobby/getcontests?sport=NHL";
        private const string CONTEST_CSV = @"/bulklineup/getdraftablecsv";

        public List<DraftKingsPlayer> DraftKingsPlayers;

        public DraftKingsFile(string date)
        {
            DraftKingsPlayers = new List<DraftKingsPlayer>();
            ReadFileIntoClass(date);
        }

        public void DownloadCSV() 
        {
            DraftGroup group = GetGroupId();
            string filePath = SAVE_PATH + $"DraftKingsSalaries_{group.StartDateEst.ToString("yyyy-MM-dd")}_{group.DraftGroupId}.csv";
            if (!File.Exists(filePath))
            {
                string url = BASE_URL + CONTEST_CSV + $"?draftGroupId={group.DraftGroupId}";
                WebClient client = new WebClient();
                client.DownloadFile(url, SAVE_PATH + filePath);
            }
        }

        private DraftGroup GetGroupId() 
        {
            string resStr;
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(BASE_URL + GETCONTESTS_PATH);
            HttpWebResponse response = (HttpWebResponse)myReq.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                resStr = reader.ReadToEnd();
            }
            CsvIdsResponse csvIdsResponse = JsonConvert.DeserializeObject<CsvIdsResponse>(resStr);
            return csvIdsResponse.GetDraftGroupId();
        }

        private void ReadFileIntoClass(string date) // yyyy-MM-dd
        {
            foreach (var fileName in Directory.GetFiles(SAVE_PATH))
            {
                if (fileName.Contains(date))
                {
                    string[] file= File.ReadAllLines(fileName);
                    for (int i = 8; i < file.Length; i++)
                    {
                        string line = file[i];
                        if (!string.IsNullOrEmpty(line))
                        {
                            DraftKingsPlayers.Add(new DraftKingsPlayer(line));
                        }
                    }
                    Console.WriteLine("DraftKings file read");
                    return;
                }
            }
            Console.Error.WriteLine("Error Reading DrafKings File.");
        }

        private class DraftGroup
        {
            public int DraftGroupId { get; set; }
            public int GameTypeId { get; set; }
            public DateTime StartDateEst { get; set; }
            public int GameCount { get; set; }
        }

        private class CsvIdsResponse
        {
            public List<DraftGroup> DraftGroups { get; set; }

            public DraftGroup GetDraftGroupId()
            {
                int currentIndex = -1;
                int currentMaxGameCount = 0;
                for (int i = 0; i < DraftGroups.Count(); i++)
                {
                    if(DraftGroups[i].GameTypeId == 125 && DraftGroups[i].GameCount > currentMaxGameCount)
                    {
                        currentIndex = i;
                        currentMaxGameCount = DraftGroups[i].GameCount;
                        
                    }
                }
                return DraftGroups[currentIndex];
            }
        }

        public class DraftKingsPlayer
        {
            public string Position { get; set; }
            public string Name { get; set; }
            public string Id { get; set; }
            public int Salary { get; set; }
            public DraftKingsPlayer(string lineFromFile)
            {
                string[] splitLines = lineFromFile.Split(',');
                Position = splitLines[10];
                Name = splitLines[12];
                Id = splitLines[13];
                if(int.TryParse(splitLines[15], out int salary))
                {
                    Salary = salary;
                }
            }
        }
    }
}
