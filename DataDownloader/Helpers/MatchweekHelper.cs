using DataModel;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public static class MatchweekHelper
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(MatchweekHelper));

        public static int GetCurrentMatchweek()
        {
            using (var ctx = new FootballEntities())
            {
                var now = DateTime.Now.Date;
                var match = ctx.Matches.FirstOrDefault(m => DbFunctions.TruncateTime(m.Date) >= now);

                if (match != null)
                {
                    int m = match.Matchweek;
                    logger.InfoFormat("Current matchweek is: {0}", m);
                    return m;
                }
                else
                {
                    logger.InfoFormat("No future matches exist in database");
                    return 0;
                }
            }
        }
    }
}