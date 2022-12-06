using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    internal static class CoordsManager
    {
        private const string FILE_PATH = @"..\entities_amount_changes.temp";
        private const string SEPARATOR = ",";

        private static StreamWriter streamWriter;

        static CoordsManager()
        {
            CreateInfoFile();
        }

        public static void CreateInfoFile()
        {
            streamWriter = File.CreateText(FILE_PATH);
            streamWriter.Close();
        }

        public static void WriteInfoIntoFile(int iteration, int predatorCount, int preyCount)
        {
            using (streamWriter = new StreamWriter(FILE_PATH, true))
            {
                streamWriter.WriteLine(iteration + SEPARATOR + predatorCount + SEPARATOR + preyCount);
            }
        }

        public static (Dictionary<int, int> numberAndPredator, Dictionary<int, int> numberAndPrey) ReadInfoFromFile()
        {
            Dictionary<int, int> numberAndPredator = new Dictionary<int, int>();
            Dictionary<int, int> numberAndPrey = new Dictionary<int, int>();

            using (StreamReader streamReader = new StreamReader(FILE_PATH))
            {
                string input = string.Empty;
                string[] splittedString = new string[3];

                while ((input = streamReader.ReadLine()) != null)
                {
                    splittedString = input.Split(SEPARATOR);

                    numberAndPredator.Add(int.Parse(splittedString[0]), int.Parse(splittedString[1]));
                    numberAndPrey.Add(int.Parse(splittedString[0]), int.Parse(splittedString[2]));
                }
            }

            return (numberAndPredator, numberAndPrey);
        }
    }
}
