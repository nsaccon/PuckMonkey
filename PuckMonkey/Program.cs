using NHLStats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PuckMonkey
{
    class Program
    {
        static void Main(string[] args)
        {
            //PlayerFile playerfile = new PlayerFile();
            //foreach (PlayerEnhanced player in playerfile.PlayersEnhanced)
            //{
            //    FantasyPoints fantasyPoints = new FantasyPoints(player);
            //    Console.WriteLine(fantasyPoints.Goals.DraftKingsAvg());
            //}
            DraftKingsFile dkFile = new DraftKingsFile();
            dkFile.DownloadCSV();


            Console.ReadKey();
        }
    }
}
