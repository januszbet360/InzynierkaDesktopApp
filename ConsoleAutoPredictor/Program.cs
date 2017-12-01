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
    /// <summary>
    /// This console application is made to be ran periodically
    /// on server. It is responsible for:
    /// 
    /// 1. Downloading new csv file with current real scores
    /// 2. Predicting scores in next matchweek
    /// 
    /// </summary>


    public class Program
    {
        public static void Main(string[] args)
        {
            string _filepath = "";

            var date = ExecutionDateHelper.GetLastExecutionDate();

            try
            {
                Console.WriteLine("\nDownloading csv stared...");
                var csv = new CsvDownloader();
                _filepath = csv.GetScoresCsv(DateTime.Now);
                Console.WriteLine("Downloading csv succeeded. File is located in: {0}", _filepath);
            }
            catch (Exception)
            {
                Console.WriteLine("Downloading csv file failed.");
                return;
            }

            try
            {
                Console.WriteLine("\nParsing csv started...");
                var cs = new CsvService();
                var n = cs.InsertScores(_filepath, date);
                Console.WriteLine("Inserting scores to database succeeded.\n{0} records have been added", n);
            }
            catch (Exception)
            {
                Console.WriteLine("Inserting scores to database failed.");
                return;
            }

            try
            {
                Console.WriteLine("\nPredicting scores started...");
                var p = new Predictor();
                List<Match> sc = p.Predict(date, DateTime.Now);
                Console.WriteLine("Predicting process succeeded.");
                Console.WriteLine("Predicted scores are:\n");
                foreach (var s in sc)
                {
                    Console.WriteLine("{0} - {1}\t\t{2}:{3}", s.Team1.Name, s.Team.Name, s.HomeGoalsPredicted, s.AwayGoalsPredicted);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("\nAn error occured while predicting scores.");
                return;
            }

            ExecutionDateHelper.SetExecutionDate();
            Console.WriteLine("\nProgram finished.");
            Console.WriteLine("Press any key to close.");
            Console.ReadLine();
        }
    }
}
