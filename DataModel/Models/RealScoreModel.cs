using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models
{
    public class RealScoreModel
    {
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public int HalfTimeHomeGoals { get; set; }
        public int HalfTimeAwayGoals { get; set; }
        public DateTime Date { get; set; }
        public int HomeShots { get; set; }
        public int AwayShots { get; set; }
        public int HomeShotsOnTarget { get; set; }
        public int AwayShotsOnTarget { get; set; }
        public int HomeWoodworks { get; set; }
        public int AwayWoodworks { get; set; }
        public int HomeCorners { get; set; }
        public int AwayCorners { get; set; }
        public int HomeFouls { get; set; }
        public int AwayFouls { get; set; }
        public int HomeYellowCards { get; set; }
        public int AwayYellowCards { get; set; }
        public int HomeRedCards { get; set; }
        public int AwayRedCards { get; set; }


        public RealScore ToDbObject()
        {
            var s = new RealScore();

            s.HomeGoals = HomeGoals;
            s.AwayGoals = AwayGoals;
            // TODO update DB
            return s;
        }
    }
}
