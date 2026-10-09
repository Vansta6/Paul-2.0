
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Paul_2._0
{
    internal class IsraelGPT
    {
        static public string Run(string query, float endChance, int fastmode)
        {
            string basePath = AppContext.BaseDirectory;
            if (basePath.Contains("bin") || basePath.Contains("build"))
                basePath = Path.GetFullPath(Path.Combine(basePath, @"..\..\..")); //remove \bin\debug\


            Paul paul = null;
            string fileData = File.ReadAllText(Path.Combine(basePath, @"data\brain.json"));
            if(!string.IsNullOrEmpty(fileData))
                paul = JsonSerializer.Deserialize<Paul>(fileData);
            else
                throw new ArgumentNullException();

            if(paul == null)
                throw new ArgumentNullException();

            //set up done
            bool is_done = false;
            string output = "";
            Random rand = new Random();
            int i = 0;



            if (query != "")
            {
                query = Regex.Replace(query, "[^a-zA-Z0-9 ]", "");
                query = query.Substring(Math.Max(query.Length - paul.StartSeek, 0), Math.Min(paul.StartSeek, query.Length));

                if (!paul.starts.ContainsKey(query))
                {
                    Console.WriteLine("brain does not contain key:" + query);
                    int minDist = int.MaxValue;
                    List<string> simStrings = new List<string>();
                    foreach (string s in paul.starts.Keys)
                    {

                        int levenDist = Levenshtein(query, s);
                        if (levenDist <= minDist)
                        {
                            if (levenDist < minDist)
                                simStrings = new List<string>();

                            minDist = levenDist;
                            simStrings.Add(s);
                            if(fastmode == 0)
                                Console.WriteLine($"simstring: {s}, ({paul.starts[s][0]}), levenDist: {levenDist}");
                        }
                    }
                    query = simStrings[rand.Next(0, simStrings.Count - 1)];

                }

                output = paul.starts[query][rand.Next(0, paul.starts[query].Count)];
            }
            else
            {
                string value = paul.brain.Keys.ToArray()[rand.Next(0, paul.brain.Keys.Count)];
                output = value;
            }

            while (!is_done)
            {
                string key = output.Substring(i, paul.Combobulation);
                if(!paul.brain.ContainsKey(key)) //if key not in brain, replace key with most similar existing key
                {
                    if (fastmode == 0)
                        Console.WriteLine("brain does not contain key: " + key);
                    int minDist = int.MaxValue;
                    List<string> simStrings = new List<string>();
                    foreach(string s in paul.brain.Keys)
                    {

                        int levenDist = Levenshtein(key, s);
                        if (levenDist <= minDist)
                        {
                            if (levenDist < minDist)
                                simStrings = new List<string>();
                            
                            minDist = levenDist;
                            simStrings.Add(s);
                            //Console.WriteLine($"simstr: {s}, ({paul.brain[s][0]}), levDist: {levenDist}");
                        }
                    }
                    key = simStrings[rand.Next(0, simStrings.Count-1)]; //replace key with any of the most similar strings to it
                    if (fastmode == 0)
                        Console.WriteLine("found key: " + key + " : " + paul.brain[key][0]);
                }

                string nextchar = paul.brain[key][rand.Next(0, paul.brain[key].Count - 1)];
                if (fastmode == 0)
                    Console.WriteLine("text: " + output + "\nadding: " + nextchar);
                output += nextchar;
                i++;

                if (nextchar == ".")
                {
                    if(rand.Next(0, 1000) > endChance*1000) is_done = true; //x% end it when adding a period
                    //output += ' ';
                }
            }


            Console.WriteLine("\n\n-------------------------OUTPUT READY-------------------------\n");
            Console.WriteLine(" " + output + "\n");
            return output;
        }

        public static int Levenshtein(string string1, string string2)
        {
            char[] charArray1 = (" " + string1).ToCharArray();
            char[] charArray2 = (" " + string2).ToCharArray();
            int x = charArray1.Length;
            int y = charArray2.Length;

            int[,] matrix = new int[x+1, y+1];

            for (int i = 0; i < x; i++)
                matrix[i, 0] = i; //defines first row

            for (int i = 0; i < y; i++)
                matrix[0, i] = i; //defines first collumn

            for(int i = 1; i < x; i++) //iterate through string1
            {
                for (int j = 1; j < y; j++) //iterate through string2
                {


                    int minDist = Math.Min(Math.Min(
                                  matrix[i-1, j], //deletion
                                  matrix[i, j-1]), //insertion
                                  matrix[i-1, j-1] //substitution
                                  );
                    if (charArray1[i] != charArray2[j])
                        minDist += 1;

                    //Console.WriteLine($"matrix[{i}, {j}] = {minDist}");
                    matrix[i, j] = minDist;
                }
            }
            //Console.WriteLine($"distance: {matrix[x, y]}");
            return matrix[x-1, y-1];

        }
    }
}