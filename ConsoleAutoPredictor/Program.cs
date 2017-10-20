using DataDownloader;
using DataDownloader.Prediction;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAutoPredictor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var dh = new ExecutionDateHelper();
            var date = dh.GetLastExecutionDate();

            try
            {
                Console.WriteLine("\nDownloading csv stared...");
                var csv = new CsvDownloader();
                var path = csv.GetScoresCsv();
                Console.WriteLine("Downloading csv succeeded. File is located in: {0}", path);
            }
            catch (Exception)
            {
                Console.WriteLine("Downloading csv file failed.");
            }

            try
            {
                Console.WriteLine("\nParsing csv started...");
                var cs = new CsvService();
                var n = cs.InsertScores(dh.FILENAME, date);
                Console.WriteLine("Inserting scores to database succeeded.\n{0} records has been added", n);
            }
            catch (Exception)
            {
                Console.WriteLine("Inserting scores to database failed.");
            }

            try
            {
                Console.WriteLine("\nPredicting scores started...");
                var p = new ScoresPredictor();
                var sc = p.Predict(p.GetNextMatchweekMatchesId());
                Console.WriteLine("Predicting process succeeded.");
                Console.WriteLine("Predicted scores are:\n");
                foreach (var s in sc)
                {
                    Console.WriteLine("{0} - {1}\t\t{2}:{3}", s.HomeTeam, s.AwayTeam, s.HomeGoals, s.AwayGoals);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("\nAn error occured while predicting scores.");
            }

            dh.WriteExecutionDate();
            Console.WriteLine("\nProgram finished.");
            Console.WriteLine("Press any key to close.");
            Console.ReadLine();
        }
    }
}
