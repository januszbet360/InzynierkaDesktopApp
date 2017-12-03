using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public class ApiDownloader
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ApiDownloader));

        public readonly int LEAGUE_ID;

        public ApiDownloader()
        {
            LEAGUE_ID = GetLeagueId();
        }

        // JSON data from api-football-data.org API (only fixture for matchday)
        public string GetMatchdayJson(int matchday)
        {
            var resp = ApiHost.Instance.Host
                .GetAsync(String.Format("/v1/competitions/{0}/fixtures?matchday={1}", LEAGUE_ID, matchday)).Result;

            if (resp.IsSuccessStatusCode)
            {
                logger.Info("Fixture downloaded successfully. Status code - " + resp.StatusCode);
                return resp.Content.ReadAsStringAsync().Result;
            }
            logger.Error("Fixture failed to download. Status code - " + resp.StatusCode);
            return null;
        }

        // JSON data from api-football-data.org (league table)
        public string GetTableJson()
        {
            var resp = ApiHost.Instance.Host
                .GetAsync(String.Format("/v1/competitions/{0}/leagueTable", LEAGUE_ID)).Result;

            if (resp.IsSuccessStatusCode)
            {
                logger.Info("League table downloaded successfully. Status code - " + resp.StatusCode);
                return resp.Content.ReadAsStringAsync().Result;
            }
            logger.Error("League table failed to download. Status code - " + resp.StatusCode);
            return null;
        }

        public int GetLeagueId()
        {
            var resp = ApiHost.Instance.Host
                .GetAsync(String.Format("/v1/competitions/?league=PL")).Result;

            if (resp.IsSuccessStatusCode)
            {
                logger.Info("League info downloaded successfully - " + resp.StatusCode);

                var json = resp.Content.ReadAsStringAsync().Result;
                JArray competitions = JArray.Parse(json);
                foreach (var c in competitions.Children())
                {
                    if ((string)c["league"] == "PL")
                    {
                        int id = (int)c["id"];
                        logger.Info("Premier League API id is: " + id);
                        return id;
                    }
                }
            }
            logger.Error("Error occured while getting league id. Probably league was not found");
            return -1;            
        }

        public string GetAllFixturesJson()
        {
            var resp = ApiHost.Instance.Host
                .GetAsync(String.Format("/v1/competitions/{0}/fixtures", LEAGUE_ID)).Result;

            if (resp.IsSuccessStatusCode)
            {
                logger.Info("All fixtures downloaded successfully. Status code - " + resp.StatusCode);
                return resp.Content.ReadAsStringAsync().Result;
            }
            logger.Error("Error while downloading fixtures. Status code: " + resp.StatusCode);
            return null;
        }
    }
}