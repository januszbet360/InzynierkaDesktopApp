using DataDownloader;
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
    public class EffectivenessTests
    {
        ScoreEffectivenessService _es = new ScoreEffectivenessService();

        [TestMethod]
        public void ComputeExactScoreEffectivenessCurrentSeason()
        {
            double res = _es.Compute("2017/2018");
            Assert.IsTrue(res > 10.0);
        }

        [TestMethod]
        public void ComputeWinnerEffectivenessCurrentSeason()
        {
            double res = _es.ComputeWeighted("2017/2018");
            Assert.IsTrue(res > 10.0);
        }


    }
}
