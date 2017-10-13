using DataModel;
using DataModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader
{
    public class TeamDataService
    {
        public List<TeamModel> GetTeamsFromFile()
        {
            var teams = new List<TeamModel>();
            string dir = AppDomain.CurrentDomain.BaseDirectory;

            var lines = File.ReadLines(dir + '\\' + Constants.TEAMS_INFO_FILE_NAME);
            
            foreach (var line in lines)
            {
                var info = line.Split(',');
                var team = new TeamModel();
                team.Name = info[0];
                team.FullName = info[1];
                team.ImageUrl = info[2];
                teams.Add(team);
            }

            return teams;
        }

        public void InsertTeams()
        {
            var teams = GetTeamsFromFile();

            using (var ctx = new FootballEntities())
            {
                using (var transaction = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        if (teams.Count > 0)
                        {
                            foreach (var t in teams)
                            {
                                ctx.Teams.Add(t.ToDbObject());
                            }
                            ctx.SaveChanges();
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                    }
                }
            }
        }
    }
}
