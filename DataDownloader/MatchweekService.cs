using DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public static class MatchweekService
    {
        public static int GetCurrentMatchweek()
        {
            using (var ctx = new FootballEntities())
            {                
                var match = ctx.Matches.FirstOrDefault(m => DbFunctions.TruncateTime(m.Date) >= DbFunctions.TruncateTime(DateTime.Now));

                if (match != null)
                    return match.Matchweek;
                else
                    return 0;
            }
        }
    }
}