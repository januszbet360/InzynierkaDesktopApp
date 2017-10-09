using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models
{
    public class MatchModel
    {
        public int Id { get; set; }
        public int HomeId { get; set; }
        public int AwayId { get; set; }
        public DateTime Date { get; set; }
        public Nullable<int> RealScoreId { get; set; }
        public string Referee { get; set; }

        public TeamModel HomeTeam { get; set; }
        public TeamModel AwayTeam { get; set; }

        public Match ToDbObject()
        {
            var m = new Match();
            m.Date = this.Date;
            using (var ctx = new FootballEntities())
            {
                m.HomeId = ctx.Teams.First(t => t.Name == HomeTeam.Name).Id;
                m.AwayId = ctx.Teams.First(t => t.Name == AwayTeam.Name).Id;
            }
            return m;
        }
    }
}
