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
            if (basePath.Contains("bin"))
                basePath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..")); //remove \bin\debug\

            string configPath = Path.Combine(basePath, "config.txt");
            Dictionary<string, string> settings = new Dictionary<string, string>(); 
            //its not (string, int) because i want to make a dumb joke, hopefully wont cause headaches.


            foreach (string line in File.ReadLines(configPath))
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

            if (int.Parse(settings["create new brain"]) == 1)
            {
                Brain_Constructor.Run(smartness, seek);
            }
            if (int.Parse(settings["run paul"]) == 1)
            {
                IsraelGPT.Run();
            }

        }
    }
}