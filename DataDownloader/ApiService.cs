using DataModel;
using DataModel.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public class ApiService
    {
        private ApiDownloader _api = new ApiDownloader();

        public List<MatchModel> GetAllMatches()
        {
            var o = JObject.Parse(_api.GetAllFixturesJson());
            return ParseJsonMatches(o);
        }

        public List<MatchModel> GetMatches(int matchday)
        {
            var o = JObject.Parse(_api.GetMatchdayJson(matchday));
            return ParseJsonMatches(o);
        }

        public List<MatchModel> ParseJsonMatches(JObject o)
        {
            var matches = new List<MatchModel>();

            var jsonMatches = o["fixtures"].Children();
            foreach (var m in jsonMatches)
            {
                var match = new MatchModel();
                match.Date = (DateTime)m["date"];
                match.HomeTeam = (String)m["homeTeamName"];
                match.AwayTeam = (String)m["awayTeamName"];
                match.Season = SeasonHelper.GetCurrentSeason(match.Date);
                match.Matchweek = (int)m["matchday"];
                matches.Add(match);
            }
            return matches;
        }

        public int InsertMatches(int matchday)
        {
            var matches = GetMatches(matchday);
            return InsertMatches(matches);
        }

        public int InsertAllMatches()
        {
            var matches = GetAllMatches();
            return InsertMatches(matches);
        }

        public int InsertMatches(List<MatchModel> matches)
        {
            int counter = 0;

            using (var ctx = new FootballEntities())
            {
                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var match in matches)
                        {
                            var hID = ctx.Teams.FirstOrDefault(t => t.FullName == match.HomeTeam).Id;
                            var aID = ctx.Teams.FirstOrDefault(t => t.FullName == match.AwayTeam).Id;
                            
                            if (ctx.Matches.Any(m => m.HomeId == hID && m.AwayId == aID && m.Season == match.Season))
                            {
                                continue;
                            }
                            else
                            {
                                ctx.Matches.Add(match.ToDbObject());
                                counter++;
                            }
                        }

                        ctx.SaveChanges();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return counter;
        }
    }
}