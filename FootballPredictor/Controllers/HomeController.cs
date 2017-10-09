using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FootballPredictor.Models;
using DataModel;
using DataDownloader;

namespace FootballPredictor.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            using (var ctx = new FootballEntities())
            {
                var ds = new MatchdayDataService();

                var data = new List<MainPageModel>();
                foreach (var m in ctx.Matches.Where(e => e.PredictedScoreId == null))
                {
                    var rnd = new Random();
                    var homeGoals = rnd.Next(5);
                    var awayGoals = rnd.Next(5);
                    var ps = new PredictedScore { HomeGoals = homeGoals, AwayGoals = awayGoals };
                    data.Add(new MainPageModel(m.Team.Name, m.Team1.Name, ps));
                }

                return View(data);
            }
        }

        public ActionResult About()
        {
            return View();
        }

    }
}
