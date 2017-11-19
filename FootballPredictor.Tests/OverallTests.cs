using DataDownloader;
using DataModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballPredictor.Tests
{ 
    [TestClass]
    public class OverallTests
    {
        [TestMethod]
        public void ResetDatabase()
        {
            ClearDatabase();

            TeamDataService tds = new TeamDataService();
            tds.InsertTeams();

            //past
            CsvService csvs = new CsvService();
            csvs.InsertMatches("13-14.csv");
            csvs.InsertScores("13-14.csv");
            csvs.InsertMatches("14-15.csv");
            csvs.InsertScores("14-15.csv");
            csvs.InsertMatches("15-16.csv");
            csvs.InsertScores("15-16.csv");
            csvs.InsertMatches("16-17.csv");
            csvs.InsertScores("16-17.csv");

            CsvDownloader csvd = new CsvDownloader();
            csvd.GetScoresCsv(DateTime.Now);

            ApiService aps = new ApiService();
            aps.InsertAllMatches();
            csvs.InsertScores("17-18.csv");
        }

        public void ClearDatabase()
        {
            using (var ctx = new FootballEntities())
            {
                ctx.Database.ExecuteSqlCommand("DELETE FROM [FullStatistics] DBCC CHECKIDENT('dbo.FullStatistics',RESEED, 0)");
                ctx.Database.ExecuteSqlCommand("DELETE FROM [Scores] DBCC CHECKIDENT('dbo.Scores', RESEED, 0)");
                ctx.Database.ExecuteSqlCommand("DELETE FROM [Matches] DBCC CHECKIDENT('dbo.Matches', RESEED, 0)");
                ctx.Database.ExecuteSqlCommand("DELETE FROM [Teams] DBCC CHECKIDENT('dbo.Teams', RESEED, 0)");

                Assert.AreEqual(ctx.FullStatistics.Count(), 0);
                Assert.AreEqual(ctx.Scores.Count(), 0);
                Assert.AreEqual(ctx.Matches.Count(), 0);
                Assert.AreEqual(ctx.Teams.Count(), 0);
            }
        }
    }
}
