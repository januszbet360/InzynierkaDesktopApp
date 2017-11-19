using DataDownloader.Prediction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootballPredictor.Tests.Prediction
{
    [TestClass]
    public class TrainingSetCreatorTests
    {
        TrainingSetCreator _tsc = new TrainingSetCreator();

        [TestMethod]
        public void GetTrainingScores()
        {
            var res = _tsc.GetTrainingScores();

            Assert.AreEqual(res.Count, TrainingSetCreator.TRAINING_SET_CARDINALITY);
        }

    }
}
