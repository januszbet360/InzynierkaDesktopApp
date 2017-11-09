using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public class MatchweekService
    {
        public int GetCurrentMatchweek()
        {
            using (var ctx = new FootballEntities())
            {
                var maxDate = (from d in ctx.Scores select d.Date).Max();
                var matchweekId = ctx.Scores.FirstOrDefault(m => m.Date == maxDate).MatchId;
                var matchweek = ctx.Matches.FirstOrDefault(m => m.Id == matchweekId).Matchweek;
                return matchweek;
            }
        }
    }
}