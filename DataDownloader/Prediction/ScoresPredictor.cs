using DataModel;
using DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Prediction
{
    public class ScoresPredictor
    {
        public List<PredictedScoreModel> Predict(List<int> toPredict)
        {
            using (var ctx = new FootballEntities())
            {
                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        List<PredictedScoreModel> pScores = new List<PredictedScoreModel>();

                        foreach (var id in toPredict)
                        {
                            var match = ctx.Matches.First(m => m.Id == id);
                            var s = PredictMatch(match);

                            s.HomeTeam = match.Team.Name;
                            s.AwayTeam = match.Team1.Name;

                            match.HomeGoalsPredicted = s.HomeGoals;
                            match.AwayGoalsPredicted = s.AwayGoals;

                            pScores.Add(s);
                        }
                        ctx.SaveChanges();
                        transaction.Commit();
                        return pScores;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return null;
                    }
                }
            }
        }

        private PredictedScoreModel PredictMatch(Match m)
        {
            var model = new PredictedScoreModel();

            Random rnd = new Random();
            model.HomeGoals = rnd.Next(0, 3);
            model.AwayGoals = rnd.Next(0, 3);
            return model;
        }

        // Returns database id's of next matchweek matches
        public List<int> GetNextMatchweekMatchesId()
        {
            using (var ctx = new FootballEntities())
            {
                // Take the closest matchweek
                var closestMatch = ctx.Matches.FirstOrDefault(m => m.Date > DateTime.Now);

                // No more matches in database - return empty collection
                if (closestMatch == null)
                    return new List<int>();
                else
                {
                    var matchweek = closestMatch.Matchweek;
                    var season = closestMatch.Season;
                    return ctx.Matches
                        .Where(m => m.Matchweek == matchweek && m.Season == season && m.Date > DateTime.Now 
                            && m.HomeGoalsPredicted == null && m.AwayGoalsPredicted == null)
                        .Select(m => m.Id).ToList();
                }
            }
        }        

    }
}
