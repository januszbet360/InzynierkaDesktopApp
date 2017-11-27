using CsvHelper;
using DataModel;
using DataModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public class CsvService
    {
        string dir = AppDomain.CurrentDomain.BaseDirectory;

        /******************************     Parse methods     ****************************************/
        public List<ScoreModel> ParseCsvScores(DateTime startDate)
        {
            return ParseCsvScores(dir + "\\" + Constants.CSV_CURRENT_FILE_NAME, startDate);
        }

        public List<ScoreModel> ParseCsvScores(string csvFilePath)
        {
            return ParseCsvScores(csvFilePath, DateTime.MinValue);
        }

        public List<ScoreModel> ParseCsvScores(string csvFilePath, DateTime startDate)
        {
            var scores = new List<ScoreModel>();

            using (TextReader reader = File.OpenText(@csvFilePath))
            {
                using (CsvReader csv = new CsvReader(reader))
                {
                    int asd = 1;
                    try
                    {
                        while (csv.Read())
                        {
                            var date = csv.GetField<DateTime>("Date");
                            var season = SeasonHelper.GetCurrentSeason(date);

                            if (date >= startDate)
                            {
                                var score = new ScoreModel();
                                score.HomeTeam = csv.GetField<string>("HomeTeam");
                                score.AwayTeam = csv.GetField<string>("AwayTeam");
                                score.HomeGoals = csv.GetField<int>("FTHG");
                                score.AwayGoals = csv.GetField<int>("FTAG");
                                score.HomeShots = csv.GetField<int>("HS");
                                score.AwayShots = csv.GetField<int>("AS");
                                score.HomeShotsOnTarget = csv.GetField<int>("HST");
                                score.AwayShotsOnTarget = csv.GetField<int>("AST");
                                score.HalfTimeHomeGoals = csv.GetField<int>("HTHG");
                                score.HalfTimeAwayGoals = csv.GetField<int>("HTAG");
                                score.HomeCorners = csv.GetField<int>("HC");
                                score.AwayCorners = csv.GetField<int>("AC");
                                score.HomeFouls = csv.GetField<int>("HF");
                                score.AwayFouls = csv.GetField<int>("AF");
                                score.HomeYellowCards = csv.GetField<int>("HY");
                                score.AwayYellowCards = csv.GetField<int>("AY");
                                score.HomeRedCards = csv.GetField<int>("HR");
                                score.AwayRedCards = csv.GetField<int>("AR");
                                score.Referee = csv.GetField<string>("Referee");
                                score.Season = season;
                                score.Date = date;

                                scores.Add(score);
                            }
                        }
                    }
                    catch(Exception)
                    { }
                }
            }
            return scores;
        }

        public List<MatchModel> ParseCsvMatches(string csvFilePath)
        {
            List<MatchModel> matches = new List<MatchModel>();

            using (TextReader reader = File.OpenText(@csvFilePath))
            {
                using (CsvReader csv = new CsvReader(reader))
                {
                    int matchOfSeason = 0;

                    try
                    {
                        while (csv.Read())
                        {
                            var date = csv.GetField<DateTime>("Date");

                            MatchModel match = new MatchModel
                            {
                                HomeTeam = csv.GetField<string>("HomeTeam"),
                                AwayTeam = csv.GetField<string>("AwayTeam"),
                                Date = date,
                                Season = SeasonHelper.GetCurrentSeason(date),
                                Matchweek = (int)(matchOfSeason / 10) + 1
                            };
                            matchOfSeason++;
                            matches.Add(match);
                        }
                    }
                    catch(Exception)
                    { }
                }
            }
            return matches;
        }
        /**********************************************************************************************/
        /******************************     Insert methods     ****************************************/
        public int InsertScores(DateTime startDate)
        {
            return InsertScores(null, startDate);
        }

        public int InsertScores(string csvFilePath)
        {
            return InsertScores(csvFilePath, DateTime.MinValue);
        }

        public int InsertScores(string csvFilePath, DateTime startDate)
        {
            List<ScoreModel> scores = null;
            if (csvFilePath == null)
            {
                scores = ParseCsvScores(startDate);
            }
            else
            {
                scores = ParseCsvScores(csvFilePath, startDate);
            }

            int counter = 0;

            using (var ctx = new FootballEntities())
            {
                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        //TODO error model

                        foreach (var s in scores)
                        {
                            var dbScore = s.ToDbObject();

                            if (ctx.Scores.Any(sc => sc.MatchId == dbScore.MatchId))
                            {
                                continue;
                            }
                            else
                            {
                                ctx.Scores.Add(dbScore);
                            }

                            UpdateLeagueTable(s);
                            counter++;
                            ctx.SaveChanges();
                        }

                        ctx.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return counter;
        }

        public int InsertMatches(string csvFilePath)
        {
            List<MatchModel> matches = ParseCsvMatches(csvFilePath);

            int counter = 0;
            using (var ctx = new FootballEntities())
            {
                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        //TODO error model

                        foreach (var match in matches)
                        {
                            var hID = ctx.Teams.FirstOrDefault(t => t.Name == match.HomeTeam).Id;
                            var aID = ctx.Teams.FirstOrDefault(t => t.Name == match.AwayTeam).Id;
                            if (ctx.Matches.Any(m => m.HomeId == hID && m.AwayId == aID && m.Season == match.Season))
                            {
                                continue;
                            }
                            else
                            {
                                ctx.Matches.Add(match.ToDbObjectFromCSV());
                                counter++;
                            }
                        }

                        ctx.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return counter;
        }

        /**********************************************************************************************/

        public void UpdateLeagueTable(ScoreModel s)
        {
            using (var ctx = new FootballEntities())
            {
                // jesli nie ma jeszcze teamu w tabeli, to go dodaj z zerowym dorobkiem
                var home = ctx.FullStatistics
                    .FirstOrDefault(t => t.Team.Name == s.HomeTeam && t.Season == s.Season);
                if (home == null)
                {
                    AddToLeagueTable(s.HomeTeam, s.Season);
                    home = ctx.FullStatistics.FirstOrDefault(t => t.Team.Name == s.HomeTeam && t.Season == s.Season);
                }

                // jesli nie ma jeszcze teamu w tabeli, to go dodaj z zerowym dorobkiem
                var away = ctx.FullStatistics
                    .FirstOrDefault(t => t.Team.Name == s.AwayTeam && t.Season == s.Season);
                if (away == null)
                {
                    AddToLeagueTable(s.AwayTeam, s.Season);
                    away = ctx.FullStatistics.FirstOrDefault(t => t.Team.Name == s.AwayTeam && t.Season == s.Season);
                }

                home.MatchesPlayed++;
                home.GoalsScored += s.HomeGoals;
                home.GoalsLost += s.AwayGoals;

                away.MatchesPlayed++;
                away.GoalsScored += s.AwayGoals;
                away.GoalsLost += s.HomeGoals;

                if (s.HomeGoals > s.AwayGoals)
                {
                    home.MatchesWon++;
                    home.Points += 3;
                    away.MatchesLost++;
                }
                else if (s.HomeGoals < s.AwayGoals)
                {
                    away.MatchesWon++;
                    away.Points += 3;
                    home.MatchesLost++;
                }
                else
                {
                    home.MatchesDrawn++;
                    away.MatchesDrawn++;
                    home.Points++;
                    away.Points++;
                }
                
                ctx.SaveChanges();
            }
        }

        public void AddToLeagueTable(string teamName, string season)
        {
            using (var ctx = new FootballEntities())
            {
                var fs = new FullStatistic();
                var team = ctx.Teams.First(t => t.Name == teamName);
                fs.TeamId = team.Id;
                fs.Season = season;

                ctx.FullStatistics.Add(fs);
                ctx.SaveChanges();
            }
        }

        public void DeleteCsv(string path)
        {
            File.Delete(path);
        }
        
    }
}