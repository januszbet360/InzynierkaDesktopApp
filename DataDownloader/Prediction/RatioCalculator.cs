using DataModel;
using DataModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Prediction
{
    public class RatioCalculator
    {
        public static readonly int MAX_ARCHIVE_SCORES_NO = 100;
        public static readonly int MAX_ARCHIVE_HOME_SCORES_NO = 10;
        public static readonly double OFFENSIVE_RATIO_DEFAULT = 1.2;
        public static readonly double DEFENSIVE_RATIO_DEFAULT = 1.0;

        public static readonly double GOALS_WEIGHT = 3.0;
        public static readonly double SHOTS_WEIGHT = 0.1;
        public static readonly double SHOTS_ON_TARGET_WEIGHT = 0.1;
        public static readonly double OPPONENT_RATIO_WEIGHT = 0.2;
        public static readonly double WEIGHTS_SUM = GOALS_WEIGHT + SHOTS_WEIGHT + SHOTS_ON_TARGET_WEIGHT;

        public RatioModel CalculateTeamsRatio(string home, string away)
        {
            RatioModel rm = new RatioModel();
            rm.HOR = CalculateOffensiveRatio(GetArchiveScores(home));
            rm.HDR = CalculateDefensiveRatio(GetArchiveScores(home));
            rm.AOR = CalculateOffensiveRatio(GetArchiveScores(away));
            rm.ADR = CalculateDefensiveRatio(GetArchiveScores(away));
            rm.HORH = CalculateOffensiveRatio(GetArchiveScores(home, true));
            rm.HDRH = CalculateDefensiveRatio(GetArchiveScores(home, true));
            rm.AORA = CalculateOffensiveRatio(GetArchiveScores(away, false));
            rm.ADRA = CalculateDefensiveRatio(GetArchiveScores(away, false));
            return rm;
        }

        public RatioModel CalculateTeamsRatio(string home, string away, DateTime date)
        {
            RatioModel rm = new RatioModel();
            rm.HOR = CalculateOffensiveRatio(GetArchiveScores(home, date));
            rm.HDR = CalculateDefensiveRatio(GetArchiveScores(home, date));
            rm.AOR = CalculateOffensiveRatio(GetArchiveScores(away, date));
            rm.ADR = CalculateDefensiveRatio(GetArchiveScores(away, date));
            rm.HORH = CalculateOffensiveRatio(GetArchiveScores(home, true, date));
            rm.HDRH = CalculateDefensiveRatio(GetArchiveScores(home, true, date));
            rm.AORA = CalculateOffensiveRatio(GetArchiveScores(away, false, date));
            rm.ADRA = CalculateDefensiveRatio(GetArchiveScores(away, false, date));
            return rm;
        }

        public Score SetRatio(Score score)
        {
            using (var ctx = new FootballEntities())
            {
                var scoreDb = ctx.Scores.SingleOrDefault(s => s.Id == score.Id);
                var match = ctx.Matches.First(m => m.Id == scoreDb.MatchId);
                var home = ctx.Teams.First(t => t.Id == match.HomeId).Name;
                var away = ctx.Teams.First(t => t.Id == match.AwayId).Name;

                scoreDb.HOR = CalculateOffensiveRatio(GetArchiveScores(home, scoreDb.Date));
                scoreDb.HDR = CalculateDefensiveRatio(GetArchiveScores(home, scoreDb.Date));
                scoreDb.AOR = CalculateOffensiveRatio(GetArchiveScores(away, scoreDb.Date));
                scoreDb.ADR = CalculateDefensiveRatio(GetArchiveScores(away, scoreDb.Date));
                scoreDb.HORH = CalculateOffensiveRatio(GetArchiveScores(home, true, scoreDb.Date));
                scoreDb.HDRH = CalculateDefensiveRatio(GetArchiveScores(home, true, scoreDb.Date));
                scoreDb.AORA = CalculateOffensiveRatio(GetArchiveScores(away, false, scoreDb.Date));
                scoreDb.ADRA = CalculateDefensiveRatio(GetArchiveScores(away, false, scoreDb.Date));

                ctx.SaveChanges();

                return scoreDb;
            }
        }

        public double CalculateOffensiveRatio(IDictionary<Score, bool> scores)
        {
            double GoalsSum = 0.0, ShotsSum = 0.0, OnTargetSum = 0.0;

            foreach (KeyValuePair<Score, bool> s in scores)
            {
                if (s.Value)
                {
                    GoalsSum += s.Key.HomeGoals * GOALS_WEIGHT;
                    ShotsSum += s.Key.HomeShots * SHOTS_WEIGHT;
                    OnTargetSum += s.Key.HomeShotsOnTarget * SHOTS_ON_TARGET_WEIGHT;
                }
                else
                {
                    GoalsSum += s.Key.AwayGoals * GOALS_WEIGHT;
                    ShotsSum += s.Key.AwayShots * SHOTS_WEIGHT;
                    OnTargetSum += s.Key.AwayShotsOnTarget * SHOTS_ON_TARGET_WEIGHT;
                }
            }
            if (scores.Count == MAX_ARCHIVE_HOME_SCORES_NO || scores.Count == MAX_ARCHIVE_SCORES_NO)
                return (GoalsSum + ShotsSum + OnTargetSum) / (WEIGHTS_SUM * scores.Count);
            else if (scores.Count != 0)
                return ((GoalsSum + ShotsSum + OnTargetSum) / (WEIGHTS_SUM * scores.Count) + OFFENSIVE_RATIO_DEFAULT) / 2;
            else
                return OFFENSIVE_RATIO_DEFAULT;

        }

        public double CalculateDefensiveRatio(IDictionary<Score, bool> scores)
        {
            double GoalsSum = 0.0, ShotsSum = 0.0, OnTargetSum = 0.0;

            foreach (KeyValuePair<Score, bool> s in scores)
            {
                if (s.Value)
                {
                    GoalsSum += s.Key.AwayGoals * GOALS_WEIGHT;
                    ShotsSum += s.Key.AwayShots * SHOTS_WEIGHT;
                    OnTargetSum += s.Key.AwayShotsOnTarget * SHOTS_ON_TARGET_WEIGHT;
                }
                else
                {
                    GoalsSum += s.Key.HomeGoals * GOALS_WEIGHT;
                    ShotsSum += s.Key.HomeShots * SHOTS_WEIGHT;
                    OnTargetSum += s.Key.HomeShotsOnTarget * SHOTS_ON_TARGET_WEIGHT;
                }
            }
            if (scores.Count == MAX_ARCHIVE_HOME_SCORES_NO || scores.Count == MAX_ARCHIVE_SCORES_NO)
                return (GoalsSum + ShotsSum + OnTargetSum) / (WEIGHTS_SUM * scores.Count);
            else if (scores.Count != 0)
                return ((GoalsSum + ShotsSum + OnTargetSum) / (WEIGHTS_SUM * scores.Count) + DEFENSIVE_RATIO_DEFAULT) / 2;
            else
                return DEFENSIVE_RATIO_DEFAULT;

        }

        public IDictionary<Score, bool> GetArchiveScores(string teamName, DateTime date)
        {
            using (var ctx = new FootballEntities())
            {
                int teamId = ctx.Teams.First(t => t.Name == teamName).Id;

                var scores = ctx.Scores
                    .Where(s => s.Date < date
                        && (s.Match.HomeId == teamId || s.Match.AwayId == teamId))
                    .OrderByDescending(d => d.Date)
                    .Take(MAX_ARCHIVE_SCORES_NO);

                var dict = new Dictionary<Score, bool>();

                foreach (var s in scores)
                {
                    if (s.Match.HomeId == teamId)
                    {
                        dict.Add(s, true);
                    }
                    else
                    {
                        dict.Add(s, false);
                    }
                }
                return dict;
            }
        }

        public IDictionary<Score, bool> GetArchiveScores(string teamName)
        {
            return GetArchiveScores(teamName, DateTime.Now);
        }

        public IDictionary<Score, bool> GetArchiveScores(string teamName, bool isHome, DateTime date)
        {
            using (var ctx = new FootballEntities())
            {
                int teamId = ctx.Teams.First(t => t.Name == teamName).Id;
                var dict = new Dictionary<Score, bool>();

                if (isHome)
                {
                    var match = ctx.Matches.First(m => m.HomeId == teamId);

                    var scores = ctx.Scores
                        .Where(s => s.Date < date && s.Match.HomeId == teamId)
                        .OrderByDescending(d => d.Date)
                        .Take(MAX_ARCHIVE_HOME_SCORES_NO);

                    foreach (var s in scores)
                    {
                        dict.Add(s, true);
                    }
                }
                else
                {
                    var match = ctx.Matches.First(m => m.AwayId == teamId);

                    var scores = ctx.Scores
                        .Where(s => s.Date < date && s.Match.AwayId == teamId)
                        .OrderByDescending(d => d.Date)
                        .Take(MAX_ARCHIVE_HOME_SCORES_NO);

                    foreach (var s in scores)
                    {
                        dict.Add(s, false);
                    }
                }
                return dict;
            }
        }

        public IDictionary<Score, bool> GetArchiveScores(string teamName, bool isHome)
        {
            return GetArchiveScores(teamName, isHome, DateTime.Now);
        }
    }
}
