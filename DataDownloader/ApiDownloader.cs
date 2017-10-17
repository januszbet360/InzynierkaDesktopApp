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
        // JSON data from api-football-data.org API (only fixture for matchday)
        public string GetMatchdayJson(int matchday)
        {
            var resp = ApiHost.Instance.Host
                .GetAsync(String.Format("/v1/competitions/{0}/fixtures?matchday={1}", Constants.LEAGUE_ID, matchday)).Result;

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
                .GetAsync(String.Format("/v1/competitions/{0}/leagueTable", Constants.LEAGUE_ID)).Result;

            if (resp.IsSuccessStatusCode)
            {
                return resp.Content.ReadAsStringAsync().Result;
            }
            return null;
        }
        
        public string GetAllFixturesJson()
        {
            var resp = ApiHost.Instance.Host
                .GetAsync(String.Format("/v1/competitions/{0}/fixtures?timeFrame=n365", Constants.LEAGUE_ID)).Result;

            if (resp.IsSuccessStatusCode)
            {
                return resp.Content.ReadAsStringAsync().Result;
            }
            return null;
        }
    }
}