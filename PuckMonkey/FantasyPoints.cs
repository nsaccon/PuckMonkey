using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PuckMonkey
{
    public class FantasyPoints
    {
        public int PlayerId;
        private int lastX;

        private PlayerEnhanced player;
        public FantasyPoints(PlayerEnhanced player, int lastX = int.MaxValue)
        {
            PlayerId = player.Player.playerID;
            this.player = player;
            this.lastX = Math.Min(lastX, player.GameStats.Count());
            Init();
        }

        public PointSet Goals;
        public PointSet Assists;
        public PointSet Shots;
        public PointSet Blocked;
        public PointSet ShortHandedPoints;
        public PointSet ShootoutGoal;
        public PointSet HatTrick;
        public PointSet Shots5Plus;
        public PointSet Blocked3Plus;
        public PointSet Points3Plus;
        public PointSet Wins;
        public PointSet Saves;
        public PointSet GoalsAgainst;
        public PointSet Shutout;
        public PointSet OvertimeLoss;
        public PointSet Saves35Plus;

        private void Init()
        {
            int count = 0;
            PointSet goals = new PointSet(PointSetType.GOAL, lastX);
            PointSet assists = new PointSet(PointSetType.ASSIST, lastX);
            PointSet shots = new PointSet(PointSetType.SHOT, lastX);
            PointSet blocked = new PointSet(PointSetType.BLOCK, lastX);
            PointSet shortHandedPoints = new PointSet(PointSetType.SHORTHANDEDPOINT, lastX);
            //PointSet shootoutGoal = null; // Data not returned from NHL API
            PointSet hatTrick = new PointSet(PointSetType.HATTRICK, lastX);
            PointSet shots5Plus = new PointSet(PointSetType.SHOTS5PLUS, lastX);
            PointSet blocked3Plus = new PointSet(PointSetType.BLOCKS3PLUS, lastX);
            PointSet points3Plus = new PointSet(PointSetType.POINTS3PLUS, lastX);
            PointSet wins = new PointSet(PointSetType.WIN, lastX);
            PointSet saves = new PointSet(PointSetType.SAVE, lastX);
            PointSet goalsAgainst = new PointSet(PointSetType.GOALAGAINST, lastX);
            PointSet shutout = new PointSet(PointSetType.SHUTOUT, lastX);
            PointSet overtimeLoss = new PointSet(PointSetType.OVERTIMELOSS, lastX);
            PointSet saves35Plus = new PointSet(PointSetType.SAVES35PLUS, lastX);
            
            for (int g = player.GameStats.Count-1; g >= 0 ; g--)
            {
                AddIntToTimesAchieved(player.GameStats[g].goals, goals);
                AddIntToTimesAchieved(player.GameStats[g].assists, assists);
                AddIntToTimesAchieved(player.GameStats[g].shots, shots);
                AddIntToTimesAchieved(player.GameStats[g].blocked, blocked);
                AddIntToTimesAchieved(player.GameStats[g].shorthandedAssists, shortHandedPoints);
                AddIntToTimesAchieved(player.GameStats[g].shorthandedGoals, shortHandedPoints);
                AddIntToTimesAchieved(player.GameStats[g].goals, hatTrick, 3); // Does not account for double hattricks
                AddIntToTimesAchieved(player.GameStats[g].shots, shots5Plus, 5);
                AddIntToTimesAchieved(player.GameStats[g].blocked, blocked3Plus, 3);
                AddIntToTimesAchieved(new string[] { player.GameStats[g].goals, player.GameStats[g].assists }, points3Plus, 3); // Points = Goals + Assists                
                if(player.GameStats[g].decision.ToUpper() == "W")
                {
                    AddIntToTimesAchieved("1", wins);
                }
                if (player.GameStats[g].decision.ToUpper() == "OTL")
                {
                    AddIntToTimesAchieved("1", overtimeLoss);
                }
                AddIntToTimesAchieved(player.GameStats[g].shotsSaved, saves);
                AddIntToTimesAchieved(player.GameStats[g].shotsSaved, saves35Plus, 35);
                AddIntToTimesAchieved(new string[] { player.GameStats[g].shotsFaced, $"-{player.GameStats[g].shotsSaved }" }, goalsAgainst);
                if (player.GameStats[g].savePercentage.StartsWith("1"))
                {
                    AddIntToTimesAchieved("1", shutout);
                }
                count++;
                if (count >= lastX) break;
            }
            Goals = goals;
            Assists = assists;
            Shots = shots;
            Blocked = blocked;
            ShortHandedPoints = shortHandedPoints;
            HatTrick = hatTrick;
            Shots5Plus = shots5Plus;
            Blocked3Plus = blocked3Plus;
            Points3Plus = points3Plus;
            Wins = wins;
            Saves = saves;
            GoalsAgainst = goalsAgainst;
            Shutout = shutout;
            OvertimeLoss = overtimeLoss;
            Saves35Plus = saves35Plus;
        }
        private void AddIntToTimesAchieved(string statValue, PointSet pointSet, int minToAdd = 1)
        {
            if (!string.IsNullOrEmpty(statValue))
            {
                int valueInt = Convert.ToInt16(statValue);
                if (minToAdd == 1)
                {
                    pointSet.TimesAchieved += valueInt;
                }
                else if(valueInt >= minToAdd)
                {
                    pointSet.TimesAchieved++;
                }
            }
        }
        private void AddIntToTimesAchieved(string[] statValue, PointSet pointSet, int minToAdd = 1)
        {
            int total = 0;
            for (int i = 0; i < statValue.Length; i++)
            {
                if(int.TryParse(statValue[i], out int amount))
                {
                    total += amount;
                }
            }
            AddIntToTimesAchieved(total.ToString(), pointSet, minToAdd);
        }

    }
}
