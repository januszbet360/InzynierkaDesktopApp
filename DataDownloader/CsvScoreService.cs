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
    public class CsvScoreService
    {
        string dir = AppDomain.CurrentDomain.BaseDirectory;


        public List<RealScoreModel> ParseCsvScores(DateTime startDate)
        {
            var scores = new List<RealScoreModel>();

            using (TextReader reader = File.OpenText(dir + Constants.CSV_FILE_NAME))
            {
                using (CsvReader csv = new CsvReader(reader))
                {
                    while (csv.Read())
                    {
                        var date = csv.GetField<DateTime>("Date");

                        if (date >= startDate)
                        {
                            var score = new RealScoreModel();
                            score.HomeTeam = csv.GetField("HomeTeam");
                            score.AwayTeam = csv.GetField("AwayTeam");
                            score.HomeGoals = csv.GetField<int>("FTHG");
                            score.AwayGoals = csv.GetField<int>("FTAG");
                            score.HomeShots = csv.GetField<int>("HS");
                            score.AwayShots = csv.GetField<int>("AS");
                            score.HomeShotsOnTarget = csv.GetField<int>("HST");
                            score.AwayShotsOnTarget = csv.GetField<int>("AST");
                            score.HalfTimeHomeGoals = csv.GetField<int>("HTHG");
                            score.HalfTimeAwayGoals = csv.GetField<int>("HTAG");
                            score.HomeWoodworks = csv.GetField<int>("HHW");
                            score.AwayWoodworks = csv.GetField<int>("AHW");
                            score.HomeCorners = csv.GetField<int>("HC");
                            score.AwayCorners = csv.GetField<int>("AC");
                            score.HomeFouls = csv.GetField<int>("HF");
                            score.AwayFouls = csv.GetField<int>("AF");
                            score.HomeYellowCards = csv.GetField<int>("HY");
                            score.AwayYellowCards = csv.GetField<int>("AY");
                            score.HomeRedCards = csv.GetField<int>("HR");
                            score.AwayRedCards = csv.GetField<int>("AR");
                            score.Date = date;

                            scores.Add(score);
                        }
                    }
                }
            }
            return scores;
        }

        public int InsertScores(DateTime startDate)
        {
            var scores = ParseCsvScores(startDate);
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
                            var match = ctx.Matches
                                .FirstOrDefault(m => m.Team.Name == s.HomeTeam && m.Team1.Name == s.AwayTeam && m.Date == s.Date);

                            var score = s.ToDbObject();
                            ctx.RealScores.Add(score);
                            counter++;
                            ctx.SaveChanges();

                            match.RealScoreId = score.Id;
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

    }
}
