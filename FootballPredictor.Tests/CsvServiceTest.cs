using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataDownloader;
using System.IO;

namespace FootballPredictor.Tests
{
    /// <summary>
    /// Summary description for ScoreServiceTests
    /// </summary>
    [TestClass]
    public class CsvServiceTest
    {
        CsvService _cs = new CsvService();

        [TestMethod]
        public void GetCsv()
        {
            var cd = new CsvDownloader();
            var path = cd.GetScoresCsv();
            Assert.IsTrue(File.Exists(path));
        }

        [TestMethod]
        public void CheckSeasonForDate()
        {

            var jan = SeasonHelper.GetCurrentSeason(new DateTime(2017,1,10));
            var july = SeasonHelper.GetCurrentSeason(new DateTime(2017, 7, 10));
            var nov = SeasonHelper.GetCurrentSeason(new DateTime(2017, 11, 10));

            Assert.AreEqual(jan, "2016/2017");
            Assert.AreEqual(july, "2017/2018");
            Assert.AreEqual(nov, "2017/2018");
        }

        [TestMethod]
        public void CheckParseCsv()
        {
            var res = _cs.ParseCsvScores(new DateTime(2017, 10, 1));
            Assert.AreEqual(res.Count, 3);

            res = _cs.ParseCsvScores(new DateTime(2017, 1, 1));
            Assert.AreEqual(res.Count, 70);
        }

        [TestMethod]
        public void InsertScores()
        {
            var counter = _cs.InsertScores(new DateTime(2017, 1, 1));
            Assert.IsTrue(counter > 0);
        }

    }
}
