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
    public class PredictorTests
    {
        [TestMethod]
        public void PredictSingle()
        {
            Predictor _pred = new Predictor();

            var res = _pred.Predict("2017/2018", 12);
            //var res2 = _pred.Predict("2017/2018", 2);
            Assert.IsTrue(res.Count > 0);
            Assert.IsNotNull(res[0]);
        }

        [TestMethod]
        public void PredictDate()
        {
            DateTime date = new DateTime(2017, 6, 1);

            Predictor _pred = new Predictor();

            var res = _pred.Predict(date, DateTime.Now);
            //var res2 = _pred.Predict("2017/2018", 2);
            Assert.IsTrue(res.Count > 0);
            Assert.IsNotNull(res[0]);
        }

    }
}
