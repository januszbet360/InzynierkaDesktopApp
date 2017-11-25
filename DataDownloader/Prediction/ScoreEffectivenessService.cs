using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public class ScoreEffectivenessService
    {
        public const double WINNER_RATIO = 0.8;
        public const double EXACT_SCORE_RATIO = 1 - WINNER_RATIO;

        public enum Winner { Home, Away, Draw };

        // all scores
        public double Compute()
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.ToList();
                return Compute(matches, false);
            }
        }

        // before certain date
        public double Compute(DateTime before)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Date < before).ToList();
                return Compute(matches, false);
            }
        }

        // season (e.g. "2015/2016")
        public double Compute(string season)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Season == season).ToList();
                return Compute(matches, false);
            }
        }

        // only one matchweek from season (e.g. "2015/2016")
        public double Compute(int matchweek, string season)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Season == season && m.Matchweek == matchweek).ToList();
                return Compute(matches, false);
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

                return Compute(matches, false);
            }
        }

        // all scores, including exact score
        public double ComputeWeighted()
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.ToList();
                return Compute(matches, true);
            }
        }
        
        // before certain date, including exact score
        public double ComputeWeighted(DateTime before)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Date < before).ToList();
                return Compute(matches, true);
            }
        }

        // season, including exact score
        public double ComputeWeighted(string season)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Season == season).ToList();
                return Compute(matches, true);
            }
        }
        
        // only one matchweek from season (e.g. "2015/2016")
        public double ComputeWeighted(int matchweek, string season)
        {
            using (var ctx = new FootballEntities())
            {
                var matches = ctx.Matches.Where(m => m.Season == season && m.Matchweek == matchweek).ToList();
                return Compute(matches, true);
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

                return Compute(matches, true);
            }
        }

        // main method
        protected double Compute(List<Match> matches, bool isWeighted)
        {
            using (var ctx = new FootballEntities())
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
                    return 0.0;
                }
                else
                {
                    return effective / (double)matchesNo;
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