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
                return resp.Content.ReadAsStringAsync().Result;
            }
            return null;
        }

        // JSON data from api-football-data.org (league table)
        public string GetTableJson()
        {
            var resp = ApiHost.Instance.Host
                .GetAsync(String.Format("/v1/competitions/{0}/leagueTable", LEAGUE_ID)).Result;

            if (resp.IsSuccessStatusCode)
            {
                return resp.Content.ReadAsStringAsync().Result;
            }
            return null;
        }

        public int GetLeagueId()
        {
            var resp = ApiHost.Instance.Host
                .GetAsync(String.Format("/v1/competitions/?league=PL")).Result;

            if (resp.IsSuccessStatusCode)
            {
                var json = resp.Content.ReadAsStringAsync().Result;
                JArray competitions = JArray.Parse(json);
                foreach (var c in competitions.Children())
                {
                    if ((string)c["league"] == "PL")
                    {
                        return (int)c["id"];
                    }
                }
            }
            return -1;            
        }

        public string GetAllFixturesJson()
        {
            var resp = ApiHost.Instance.Host
                .GetAsync(String.Format("/v1/competitions/{0}/fixtures", LEAGUE_ID)).Result;

            if (resp.IsSuccessStatusCode)
            {
                return resp.Content.ReadAsStringAsync().Result;
            }
            return null;
        }
    }
}