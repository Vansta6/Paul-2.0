using System.Text.Json;
using System.Text.RegularExpressions;

namespace Paul_2._0
{
    public class Paul
    {
        public Dictionary<string, List<string>> brain { get; set; }
        public Dictionary<string, List<string>> starts { get; set; }

        public int Combobulation { get; set; }
        public int StartSeek { get; set; }

        public Paul(int combobulation, int startSeek)
        {
            brain = new Dictionary<string, List<string>>();
            starts = new Dictionary<string, List<string>>();

            Combobulation = combobulation;
            StartSeek = startSeek;
        }
    }



    public class Brain_Constructor
    {
        static public void Run(int smartness, int memory)
        {
            string basePath = AppContext.BaseDirectory;
            if (basePath.Contains("bin")) 
                basePath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..")); //remove \bin\debug\
            string dataPath = Path.Combine(basePath, @"data\Data.txt");
            string[] data = File.ReadAllLines(dataPath);

            Paul paul = new Paul(smartness, memory);

            List<string> questions = new List<string>();
            List<string> answers = new List<string>();
            List<string> text = new List<string>();



            foreach (string s in data)
            {
                string str = s;
                str = Regex.Replace(s, "[0-9]", "");
                
                if (str.Contains('?'))
                {
                    str = Regex.Replace(str, "[^a-zA-Z0-9? ]", "");
                    string[] split = str.Split('?', 2);
                    if (split[1].Length < 2)
                    {
                        text.Add(str + "."); //if it has no question mark, just add a period
                        continue;
                    }

                    questions.Add(split[0]); //add first half to list of questions
                    answers.Add(split[1] + ".");
                }
                else
                {
                    text.Add(str + ".");
                }
            }



            for(int i = 0; i < questions.Count; i++)
            {
                int start = Math.Max(answers[i].Length - 1 - paul.StartSeek, 0);
                start = Math.Min(start, questions[i].Length);
                int length = Math.Min(paul.StartSeek, questions[i].Length - start);

                string question = questions[i].Substring(start, length);
                string answer = answers[i].Substring(0, Math.Min(answers[i].Length, paul.Combobulation)); //get the last x characters from string

                if(paul.starts.ContainsKey(question)) //create a new list for the answer or add to prexisting
                    paul.starts[question].Add(answer);
                else
                    paul.starts.Add(question, new List<string>() { answer });

                Console.WriteLine("question: " + question + ": " + string.Join(", ", paul.starts[question]));
            }



            text = text.Concat(answers).ToList();
            for (int i = 0; i < text.Count; i++)
            {
                for (int x = 0; x < text[i].Length - paul.Combobulation; x++)
                {
                    string key = text[i].Substring(x, paul.Combobulation);
                    string nextChar = text[i].Substring(x + paul.Combobulation, 1);

                    if(paul.brain.ContainsKey(key))
                        paul.brain[key].Add(nextChar);
                    else
                        paul.brain.Add(key, new List<string>() {nextChar});
                }
            }
            


            
            foreach (string key in paul.brain.Keys)
            {
                Console.WriteLine(key +  ": " + string.Join(", ", paul.brain[key]));
            }
            using FileStream createStream = File.Create(Path.Combine(basePath, @"data\brain.json"));
            JsonSerializer.Serialize(createStream, paul);

        }
    }
}
