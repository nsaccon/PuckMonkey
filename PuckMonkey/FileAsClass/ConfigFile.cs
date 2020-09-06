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
        private string PATH = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Files\Config.json");

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
            System.IO.File.WriteAllText(PATH, output);
        }
        private void ReadFromFile()
        {
            string input = System.IO.File.ReadAllText(PATH);
            ConfigFile file = JsonConvert.DeserializeObject<ConfigFile>(input);
            if (file != null)
            {
                ScheduleDaysLoaded = file.ScheduleDaysLoaded;
            }
        }
    }
}
