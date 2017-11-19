using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataDownloader;
using DataModel;
using System.IO;
using System;

namespace FootballPredictor.Tests
{
    [TestClass]
    public class ApiServiceTests
    {
        ApiDownloader _api = new ApiDownloader();

        [TestMethod]
        public void GetFixture()
        {
            var s = _api.GetMatchdayJson(1);
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public void GetAllFixtures()
        {
            var s = _api.GetAllFixturesJson();
            Assert.IsNotNull(s);
        }

        [TestMethod]
        public void GetTable()
        {
            var t = _api.GetTableJson();
            Assert.IsNotNull(t);
        }

        [TestMethod]
        public void GetLeagueId()
        {
            var id = _api.GetLeagueId();
            Assert.AreEqual(id, 445);
        }

    }
}
