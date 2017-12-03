using DataModel;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public class ScoreEffectivenessService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ScoreEffectivenessService));

        public const double WINNER_RATIO = 0.0;
        public const double EXACT_SCORE_RATIO = 1 - WINNER_RATIO;

        public enum Winner { Home, Away, Draw };

        // all scores
        public double Compute()
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.ToList();
                double res =  Compute(matches, false);
                logger.InfoFormat("Prediction effectiveness of all scores equals: {0}", res);
                return res;
            }
        }

        // before certain date
        public double Compute(DateTime before)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Date < before).ToList();
                double res = Compute(matches, false);
                logger.InfoFormat("Prediction effectiveness of scores before {0} equals: {1}", before, res);
                return res;
            }
        }

        // season (e.g. "2015/2016")
        public double Compute(string season)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Season == season).ToList();
                double res = Compute(matches, false);
                logger.InfoFormat("Prediction effectiveness of scores in season {0} equals: {1}", season, res);
                return res;
            }
        }

        // only one matchweek from season (e.g. "2015/2016")
        public double Compute(int matchweek, string season)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Season == season && m.Matchweek == matchweek).ToList();
                double res = Compute(matches, false);
                logger.InfoFormat("Prediction effectiveness of scores in matchweek {0} of season {1} equals: {2}", matchweek, season, res);
                return res;
            }
        }

        // last n matchweeks
        public double Compute(int n)
        {
            using (var ctx = new FootballEntities())
            {
                var lastMatch = ctx.Matches.Where(m => m.HomeGoalsPredicted != null && m.AwayGoalsPredicted != null).Last();
                var matches = ctx.Matches
                    .Where(m => m.Season == lastMatch.Season && m.Matchweek + n >= lastMatch.Matchweek)
                    .ToList();

                double res = Compute(matches, false);
                logger.InfoFormat("Prediction effectiveness of last {0} matchweeks equals: {1}", n, res);
                return res;
            }
        }

        // all scores, including exact score
        public double ComputeWeighted()
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.ToList();
                double res = Compute(matches, true);
                logger.InfoFormat("Prediction weighted effectiveness of all scores equals: {0}", res);
                return res;
            }
        }
        
        // before certain date, including exact score
        public double ComputeWeighted(DateTime before)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Date < before).ToList();
                double res = Compute(matches, true);
                logger.InfoFormat("Prediction weighted effectiveness of scores before {0} equals: {1}", before, res);
                return res;
            }
        }

        // season, including exact score
        public double ComputeWeighted(string season)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Season == season).ToList();
                double res = Compute(matches, true);
                logger.InfoFormat("Prediction weighted effectiveness of scores in season {0} equals: {1}", season, res);
                return res;
            }
        }
        
        // only one matchweek from season (e.g. "2015/2016")
        public double ComputeWeighted(int matchweek, string season)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Season == season && m.Matchweek == matchweek).ToList();
                double res = Compute(matches, true);
                logger.InfoFormat("Prediction weighted effectiveness of scores in {0} matchweek of season {1} equals: {2}", matchweek, season, res);
                return res;
            }
        }

        // last n matchweeks, including exact score
        public double ComputeWeighted(int n)
        {
            using (var ctx = new FootballEntities())
            {
                var lastMatch = ctx.Matches.Where(m => m.HomeGoalsPredicted != null && m.AwayGoalsPredicted != null).ToList().Last();
                var matches = ctx.Matches
                    .Where(m => m.Season == lastMatch.Season && m.Matchweek + n >= lastMatch.Matchweek)
                    .ToList();

                double res = Compute(matches, true);
                logger.InfoFormat("Prediction weighted effectiveness of last {0} matchweeks equals: {1}", n, res);
                return res;
            }
        }

        // main method
        protected double Compute(List<Match> matches, bool isWeighted)
        {
            using (var ctx = new FootballEntities())
            {
                try
                {
                    int matchesNo = 0;
                    double effective = 0.0;

                    foreach (var m in matches)
                    {
                        var score = ctx.Scores.FirstOrDefault(s => s.MatchId == m.Id);

                        if (score != null)
                        {
                            var pHome = m.HomeGoalsPredicted;
                            var pAway = m.AwayGoalsPredicted;
                            var rHome = score.HomeGoals;
                            var rAway = score.AwayGoals;

                            if (pHome != null && pAway != null)
                            {
                                matchesNo++;

                                if (GetWinner((int)pHome, (int)pAway) == GetWinner(rHome, rAway))
                                {
                                    if (isWeighted)
                                    {
                                        effective += WINNER_RATIO;
                                        if ((int)pHome == rHome && (int)pAway == rAway)
                                        {
                                            effective += EXACT_SCORE_RATIO;
                                        }
                                    }
                                    else
                                    {
                                        effective += 1.0;
                                    }
                                }
                            }
                        }
                    }

                    if (matchesNo == 0)
                    {
                        logger.Warn("Matches count = 0. No effectiveness could be computed");
                        return 0.0;
                    }
                    else
                    {
                        return effective / (double)matchesNo;
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error while computing prediction effectiveness.", e);
                    return -1.0;
                }
            }
        }

        // returns winner of match with given home and away team goals
        protected Winner GetWinner(int homeGoals, int awayGoals)
        {
            if (homeGoals > awayGoals)
            {
                return Winner.Home;
            }
            else if (homeGoals < awayGoals)
            {
                return Winner.Away;
            }
            else
            {
                return Winner.Draw;
            }
        }
    }
}