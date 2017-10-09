using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataDownloader;
using DataModel;
using System.IO;

namespace FootballPredictor.Tests
{
    [TestClass]
    public class ApiServiceTests
    {

        [TestMethod]
        public void GetCsv()
        {
            var _api = new ApiDownloader();

            _api.GetScoresCsv();
            Assert.IsTrue(File.Exists("E0.csv"));
        }
    }
}
