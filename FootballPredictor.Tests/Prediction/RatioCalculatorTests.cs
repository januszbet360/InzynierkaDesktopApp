using DataDownloader.Prediction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballPredictor.Tests.Prediction
{
    [TestClass]
    public class RatioCalculatorTests
    {
        RatioCalculator _rc = new RatioCalculator(); 

        [TestMethod]
        public void GetArchiveScores()
        {
            var res = _rc.GetArchiveScores("Arsenal");

            Assert.IsTrue(res.Keys.Count > 0);
        }

        [TestMethod]
        public void GetArchiveScoresHome()
        {
            var res = _rc.GetArchiveScores("Arsenal", true);

            Assert.IsTrue(res.Keys.Count > 0);

        }

        [TestMethod]
        public void GetArchiveScoresAway()
        {
            var res = _rc.GetArchiveScores("Arsenal", false);

            Assert.IsTrue(res.Keys.Count > 0);
        }

        [TestMethod]
        public void CalculateTeamRatio()
        {
            var res = _rc.CalculateTeamsRatio("Arsenal", "Burnley");

            Assert.AreEqual(res.HOR, 1.5);
        }

        [TestMethod]
        public void CalculateOffensiveRatio()
        {
            var res = _rc.CalculateTeamsRatio("Arsenal", "Burnley");

            Assert.IsTrue(res.HOR > 1.0);
        }

        [TestMethod]
        public void CalculateDefensiveRatio()
        {
            var res = _rc.CalculateTeamsRatio("Arsenal", "Burnley");

            Assert.IsTrue(res.HOR > 1.0);
        }

        [TestMethod]
        public void SetRatio()
        {
            var score = _rc.GetArchiveScores("Arsenal").First().Key;
            _rc.SetRatio(score);
            score = _rc.GetArchiveScores("Arsenal").First().Key;

            Assert.IsTrue(score.HOR > 1.0);
        }

        [TestMethod]
        public void GetPreviousRatio()
        {
            var ort = _rc.GetTeamRatio(1, DateTime.Now, true); // Arsenal offensive
            var drt = _rc.GetTeamRatio(1, DateTime.Now, false); // Arsenal defensive

            Assert.IsTrue(ort == 0.0);
            Assert.IsTrue(drt == 0.0);
        }


    }
}
