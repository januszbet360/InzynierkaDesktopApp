using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public class CsvDownloader
    {
        // CSV with scores from football-data.co.uk
        public string GetScoresCsv()
        {
            using (var client = new WebClient())
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                string path = dir + Constants.CSV_CURRENT_FILE_NAME;

                client.DownloadFile(@"http://www.football-data.co.uk/mmz4281/1718/E0.csv", path);

                return path;
            }
        }

        public string GetScoresCsv(DateTime date)
        {
            using (var client = new WebClient())
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                string path = dir + Constants.CSV_CURRENT_FILE_NAME;
                string season = SeasonHelper.GetCurrentSeasonShort(date);

                if (File.Exists(Constants.CSV_CURRENT_FILE_NAME))
                {
                    File.Delete(Constants.CSV_CURRENT_FILE_NAME);
                }

                client.DownloadFile(@"http://www.football-data.co.uk/mmz4281/" + season + @"/E0.csv", path);

                return path;
            }
        }
    }
}