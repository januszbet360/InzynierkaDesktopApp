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
    public class PredictingMachineTests
    {
        PredictingMachine _pm = new PredictingMachine();

        [TestMethod]
        public void TeachMachine()
        {            
            Assert.IsNotNull(_pm.Predictor);
        }

        [TestMethod]
        public void CompareMachines()
        {
            var m1 = new PredictingMachine();
            var m2 = new PredictingMachine();

            Assert.AreNotEqual(m1._scores, m2._scores);
            Assert.AreEqual(m1._scores.Count(), m2._scores.Count());
        }



    }
}
