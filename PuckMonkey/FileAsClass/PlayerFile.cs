using Newtonsoft.Json;
using NHLStats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using static PuckMonkey.DraftKingsFile;

namespace PuckMonkey
{
    class PlayerFile
    {
        private string PATH = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\Files\Players.json");

        public List<PlayerEnhanced> PlayersEnhanced;

        public PlayerFile()
        {
            ReadFromFile();
            if(PlayersEnhanced == null)
            {
                PlayersEnhanced = new List<PlayerEnhanced>();
            }
        }

        private bool UpdatePlayer(PlayerEnhanced plr, string gameId)
        {
            bool isNew = true;
            int replaceIndex = -1;
            for (int i = 0; i < PlayersEnhanced.Count; i++)
            {
                if(plr.Player.playerID == PlayersEnhanced[i].Player.playerID)
                {
                    isNew = false;
                    replaceIndex = i;
                    break;
                }
            }

            if (isNew)
            {
                plr.GamesPlayedIn.Add(gameId);
                PlayersEnhanced.Add(plr);
                Console.WriteLine($"New Player Created - {plr.Player.playerID}");
                return true;
            }
            else if(!PlayersEnhanced[replaceIndex].GamesPlayedIn.TryGetValue(gameId, out string value))
            {
                PlayersEnhanced[replaceIndex].GameStats.Add(plr.GameStats[0]);
                PlayersEnhanced[replaceIndex].GamesPlayedIn.Add(gameId);
                Console.WriteLine($"Existing Player Updated - {plr.Player.playerID}");
                return true;
            }
            else
            {
                Console.WriteLine($"Existing Player duplicate, no update - {plr.Player.playerID}");
                return false;
            }
        }

        public void SaveToFile()
        {
            string output = JsonConvert.SerializeObject(PlayersEnhanced);
            File.WriteAllText(PATH, output);
            Console.WriteLine("Player File saved.");
        }
        private void ReadFromFile()
        {
            string input = File.ReadAllText(PATH);
            PlayersEnhanced = JsonConvert.DeserializeObject<List<PlayerEnhanced>>(input);
            Console.WriteLine("Player file read.");
        }

        private void UpdatePlayerData(DateTime date)
        {
            string dateString = date.ToString("yyyy-MM-dd");
            ConfigFile file = new ConfigFile(true);
            if (file.ScheduleDaysLoaded.Contains(dateString)) return;
            Schedule sched = new Schedule(dateString);
            bool save = true;
            if (sched.games != null)
            {
                foreach (Game game in sched.games)
                {
                    List<Player> plrs = game.gameParticipants;
                    if (plrs != null && plrs.Count > 0)
                    {
                        Dictionary<int, int> plrsIndex = Player.PlayerIdToIndexVault(plrs);
                        BoxScore bs = game.gameBoxScore;
                        List<PlayerGameStats> playerGameStats = bs.awayTeamStats.teamPlayers.Concat(bs.homeTeamStats.teamPlayers).ToList();
                        for (int i = 0; i < playerGameStats.Count(); i++)
                        {
                            PlayerEnhanced plrE = new PlayerEnhanced();
                            int index;
                            plrsIndex.TryGetValue(playerGameStats[i].playerID, out index);
                            plrE.Player = plrs[index];
                            plrE.GameStats.Add(playerGameStats[i]);
                            UpdatePlayer(plrE, game.gameID);
                        }
                    }
                    else
                    {
                        Console.Error.WriteLine($"gameParticipants found to be null or empty. File not saved for {dateString}");
                        save = false;
                    }
                }
                if (save)
                {
                    this.SaveToFile();
                    file.ScheduleDaysLoaded.Add(dateString);
                    file.SaveToFile();
                }
            }
        }

        public void AddStatsFromDates(string startDate, string endDate) // Dates in format yyyy-MM-dd
        {
            DateTime currentDate = DateTime.Parse(startDate);
            DateTime endDT = DateTime.Parse(endDate);
            if (endDT < currentDate) return;
            while (currentDate <= endDT)
            {
                PlayerFile playerFile = new PlayerFile();
                playerFile.UpdatePlayerData(currentDate);
                currentDate = currentDate.AddDays(1);
            }
        }

        public void AddDraftKingsSalaries(string date)
        {
            DraftKingsFile draftKingsFile = new DraftKingsFile(date);
            List<DraftKingsPlayer> draftKingsPlayers = draftKingsFile.DraftKingsPlayers;
            for (int i = 0; i < draftKingsPlayers.Count(); i++)
            {
                bool playerFound = false;
                for (int j = 0; j < PlayersEnhanced.Count(); j++)
                {
                    if(draftKingsPlayers[i].Name == $"{PlayersEnhanced[j].Player.firstName} {PlayersEnhanced[j].Player.lastName}")
                    {
                        Console.WriteLine($"Found player: {draftKingsPlayers[i].Name}");
                        PlayersEnhanced[j].Salary.DraftKings = draftKingsPlayers[i].Salary;
                        playerFound = true;
                        break;
                    }
                }
                if (!playerFound)
                {
                    Console.WriteLine($"Player name NOT found: {draftKingsPlayers[i].Name}");
                }
            }
        }
    }
}
