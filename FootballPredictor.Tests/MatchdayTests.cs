using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataDownloader;
using DataModel;

namespace FootballPredictor.Tests
{
    [TestClass]
    public class MatchdayTests
    {
        private readonly MatchdayService _ds = new MatchdayService();

        [TestMethod]
        public void GetMatches()
        {
            var res = _ds.GetMatches(1);

            Assert.AreEqual(res[0].HomeTeam, "Arsenal FC");
        }

        [TestMethod]
        public void CheckMatchesCount()
        {
            var res = _ds.GetMatches(1);
            Assert.AreEqual(res.Count*2, 20);
        }
    }
}
