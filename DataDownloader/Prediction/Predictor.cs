using DataModel;
using DataModel.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Prediction
{
    public class Predictor
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Predictor));

        public PredictingMachine Machine { get; set; }
        public RatioCalculator Calculator { get; set; }

        public Predictor()
        {
            Machine = new PredictingMachine();
            Calculator = new RatioCalculator();
        }

        public Match Predict(Match match)
        {
            try
            {
                using (var ctx = new FootballEntities())
                {
                    var matchDb = ctx.Matches.First(m => m.Id == match.Id);
                    string home = ctx.Teams.First(t => t.Id == matchDb.HomeId).Name;
                    string away = ctx.Teams.First(t => t.Id == matchDb.AwayId).Name;

                    RatioModel ratio = Calculator.CalculateTeamsRatio(home, away, match.Date);

                    var res = Machine.PredictScore(ratio);

                    matchDb.HomeGoalsPredicted = res.Item1;
                    matchDb.AwayGoalsPredicted = res.Item2;

                    ctx.SaveChanges();
                    return matchDb;
                }
            }
            catch (Exception e)
            {
                logger.Error(string.Format("Error while predicting match: homeId = {0}, awayId = {1}, date = {2}", match.HomeId, match.AwayId, match.Date), e);
                return null;
            }
        }

        public List<Match> Predict(List<Match> matches)
        {
            var list = new List<Match>();

            foreach (var m in matches)
            {
                list.Add(Predict(m));
            }

            return list;
        }

        public List<Match> Predict(string season, int matchweek)
        {
            return Predict(GetMatchesForMatchweek(season, matchweek));
        }

        public List<Match> Predict(DateTime startDate)
        {
            return Predict(GetMatchesBetweenDates(startDate, DateTime.Now));
        }

        public List<Match> Predict(DateTime start, DateTime end)
        {
            return Predict(GetMatchesBetweenDates(start, end));
        }
        
        protected List<Match> GetMatchesForMatchweek(string season, int matchweek)
        {
            try
            {
                using (var ctx = new FootballEntities())
                {
                    var list = ctx.Matches.Where(m => m.Season == season && m.Matchweek == matchweek).ToList();
                    logger.InfoFormat("GetMatchesForMatchweek (season = {0}, matchweek = {1}) succeeded. {2} matches have been taken", season, matchweek, list.Count);
                    return list;
                }
            }
            catch (Exception e)
            {
                logger.Error(string.Format("GetMatchesForMatchweek (season = {0}, matchweek = {1}) failed due to exception", season, matchweek), e);
                return null;
            }
        }

        protected List<Match> GetMatchesBetweenDates(DateTime start, DateTime end)
        {
            try
            {
                using (var ctx = new FootballEntities())
                {
                    var list = ctx.Matches.Where(m => m.Date >= start && m.Date <= end).ToList();
                    logger.InfoFormat("GetMatchesBetweenDates (start = {0}, end = {1}) succeeded. {2} matches have been taken", start, end, list.Count);
                    return list;
                }
            }
            catch (Exception e)
            {
                logger.Error(string.Format("GetMatchesBetweenDates (start = {0}, end = {1}) failed due to exception.", start, end), e);
                return null;
            }
        }
    }
}