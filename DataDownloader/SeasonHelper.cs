using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public static class SeasonHelper
    {
        public static string GetCurrentSeason(DateTime date)
        {
            StringBuilder currentSeason = new StringBuilder();

            // Example: match on 23 October 2012 = season "2012/2013"
            if (date.Month >= 7)
            {
                currentSeason.Append(date.Year);
                currentSeason.Append('/');
                currentSeason.Append(date.Year + 1);
            }
            else
            {
                currentSeason.Append(date.Year - 1);
                currentSeason.Append('/');
                currentSeason.Append(date.Year);
            }
            return currentSeason.ToString();
        }

        public static string GetCurrentSeasonShort(DateTime date)
        {
            StringBuilder currentSeason = new StringBuilder();

            // Example: match on 23 October 2012 = season "1213"
            if (date.Month >= 7)
            {
                currentSeason.Append(date.Year % 100);
                currentSeason.Append((date.Year + 1) % 100);
            }
            else
            {
                currentSeason.Append((date.Year - 1) % 100);
                currentSeason.Append(date.Year % 100);
            }
            return currentSeason.ToString();
        }

    }
}