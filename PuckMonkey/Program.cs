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
            PlayerFile playerfile = new PlayerFile();
            playerfile.AddDraftKingsSalaries("2020-09-06");


            Console.ReadKey();
        }
    }
}
