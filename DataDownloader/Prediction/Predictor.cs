using DataModel;
using DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Prediction
{
    public class Predictor
    {
        public PredictingMachine Machine { get; set; }
        public RatioCalculator Calculator { get; set; }

        public Predictor()
        {
            Machine = new PredictingMachine();
            Calculator = new RatioCalculator();
        }

        public Match Predict(Match match)
        {
            using (var ctx = new FootballEntities())
            {
                var matchDb = ctx.Matches.First(m => m.Id == match.Id);
                string home = ctx.Teams.First(t => t.Id == matchDb.HomeId).Name;
                string away = ctx.Teams.First(t => t.Id == matchDb.AwayId).Name;

                RatioModel ratio = Calculator.CalculateTeamsRatio(home,away,match.Date);

                var res = Machine.PredictScore(ratio);

                matchDb.HomeGoalsPredicted = res.Item1;
                matchDb.AwayGoalsPredicted = res.Item2;

                ctx.SaveChanges();
                return matchDb;
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

        public List<Match> Predict(DateTime endDate)
        {
            return Predict(GetMatchesBetweenDates(DateTime.Now, endDate));
        }

        public List<Match> Predict(DateTime start, DateTime end)
        {
            return Predict(GetMatchesBetweenDates(start, end));
        }

        protected List<Match> GetMatchesForMatchweek(string season, int matchweek)
        {
            using (var ctx = new FootballEntities())
            {
                return ctx.Matches.Where(m => m.Season == season && m.Matchweek == matchweek).ToList();
            }
        }

        protected List<Match> GetMatchesBetweenDates(DateTime start, DateTime end)
        {
            using (var ctx = new FootballEntities())
            {
                return ctx.Matches.Where(m => m.Date >= start && m.Date <= end).ToList();
            }
        }

    }
}
