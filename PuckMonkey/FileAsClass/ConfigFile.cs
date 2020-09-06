using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace PuckMonkey
{
    class ConfigFile
    {
        private string PATH = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\Files\Config.json");

        public HashSet<string> ScheduleDaysLoaded;

        public ConfigFile(bool readfile = false)
        {
            if (readfile)
            {
                ReadFromFile();
                if (ScheduleDaysLoaded == null)
                {
                    ScheduleDaysLoaded = new HashSet<string>();
                }
            }
        }

        public void SaveToFile()
        {
            string output = JsonConvert.SerializeObject(this);
            File.WriteAllText(PATH, output);
            Console.WriteLine("Config file saved.");
        }
        private void ReadFromFile()
        {
            string input = File.ReadAllText(PATH);
            ConfigFile file = JsonConvert.DeserializeObject<ConfigFile>(input);
            if (file != null)
            {
                ScheduleDaysLoaded = file.ScheduleDaysLoaded;
                Console.WriteLine("Config file read.");
            }
        }
    }
}
