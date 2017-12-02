using DataDownloader;
using DataDownloader.Prediction;
using DataModel;
using log4net;
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

        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            string _filepath = "";

            var date = ExecutionDateHelper.GetLastExecutionDate();

            try
            {
                Console.WriteLine("\nDownloading csv stared...");
                var csv = new CsvDownloader();
                _filepath = csv.GetScoresCsv(DateTime.Now);
                logger.Info("Csv file has been saved as: " + _filepath);
                Console.WriteLine("Downloading csv succeeded. File is located in: {0}", _filepath);
            }
            catch (Exception e)
            {
                logger.Error("Error while downloading csv file", e);
                Console.WriteLine("Downloading csv file failed.");
                return;
            }

            try
            {
                Console.WriteLine("\nParsing csv started...");
                var cs = new CsvService();
                var n = cs.InsertScores(_filepath, date);
                logger.Info(n + " score records have been added to database");
                Console.WriteLine("Inserting scores to database succeeded.\n{0} records have been added", n);
            }
            catch (Exception e)
            {
                logger.Error("Error while inserting scores.", e);
                Console.WriteLine("Inserting scores to database failed.");
                return;
            }

            try
            {
                Console.WriteLine("\nPredicting scores started...");
                var p = new Predictor();
                string season = SeasonHelper.GetCurrentSeason(DateTime.Now);
                int matchweek = MatchweekService.GetCurrentMatchweek();

                if (matchweek == 0 || matchweek == 38)
                {
                    season = SeasonHelper.GetNextSeason(season);
                    matchweek = 1;
                }
                else
                {
                    matchweek++;
                }

                logger.Info(string.Format("Prediction will be run for matchweek {0} of season {1}", matchweek, season));
                List<Match> sc = p.Predict(season, matchweek);
                logger.Info("Prediction finished successfully");
                Console.WriteLine("Predicting process succeeded.");
                Console.WriteLine("Predicted scores are:\n");
                foreach (var s in sc)
                {
                    Console.WriteLine("{0} - {1}\t\t{2}:{3}", s.Team1.Name, s.Team.Name, s.HomeGoalsPredicted, s.AwayGoalsPredicted);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error while predicting.", e);
                Console.WriteLine("\nAn error occured while predicting scores.");
                return;
            }

            ExecutionDateHelper.SetExecutionDate();
            logger.Info("Bye");
            Console.WriteLine("\nProgram finished.");
        }
    }
}
