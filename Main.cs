using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Paul_2._0
{
    internal class MainScript
    {
        static void Main(string[] args) //TODO: make it so these scripts are easy to alternate which is running
        {
            string basePath = AppContext.BaseDirectory;
            if (basePath.Contains("bin") || basePath.Contains("build"))
                basePath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..")); //remove \bin\debug\
            Console.WriteLine(basePath);
            string configPath = Path.Combine(basePath, "Config.txt");
            string[] config = File.ReadAllLines(configPath);
            Dictionary<string, string> settings = new Dictionary<string, string>();
            //its not (string, int) because i want to make a dumb joke, hopefully wont cause headaches.


            foreach (string line in config)
            {
                List<string> split = line.Split(": ").ToList();
                string setting = split[0];
                string value = "";

                int.TryParse(split[1], out int val);

                if (split[1] == "true") value = "1";
                else
                    if (split[1] == "false") value = "0";
                    else
                        if (val > 0) value = val.ToString();
                        else
                            value = split[1];

                Console.WriteLine(setting + ": " + value);
                settings.Add(setting, value);
            }

            int smartness = int.Parse(settings["smartness"]);
            int seek = int.Parse(settings["query seek"]);
            float endChance = float.Parse(settings["end chance"]);

            string dataPath = Path.Combine(basePath, @"data\Data.txt");
            string savePath = Path.Combine(basePath, @"data\save");
            string saveconfigPath = Path.Combine(basePath, @"data\configsave");
            string data = File.ReadAllText(dataPath);
            string savedata = File.ReadAllText(dataPath);
            string saveconfigdata = File.ReadAllText(saveconfigPath); //i hate my life



            if (savedata != data || saveconfigdata != File.ReadAllText(configPath)) //detects changes to data or config
            {
                Console.WriteLine("New data detected... Reconstructing brain");
                Thread.Sleep(1000);
                Brain_Constructor.Run(smartness, seek);
                Console.WriteLine("-----DONE-----");
            }

            while (true)
            {
                Console.Write("Enter your query, or enter nothing for no query: ");
                string query = "";
                query = Console.ReadLine();
                IsraelGPT.Run(query, endChance);
            }


        }
    }
}