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
        public void DownloadCSV() 
        {
            DraftGroup group = GetGroupId();
            string fileName = $"DraftKingsSalaries_{group.StartDateEst.ToString("yyyy-MM-dd")}_{group.DraftGroupId}.csv";
            if (!File.Exists(SAVE_PATH + fileName))
            {
                string url = BASE_URL + CONTEST_CSV + $"?draftGroupId={group.DraftGroupId}";
                WebClient client = new WebClient();
                client.DownloadFile(url, SAVE_PATH + fileName);
            }
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
    }
}
