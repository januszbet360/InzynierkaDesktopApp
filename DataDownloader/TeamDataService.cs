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
        public List<Team> GetTeamsFromFile()
        {
            var teams = new List<Team>();
            try
            {
                string teamsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.TEAMS_INFO_FILE_NAME);
                var lines = File.ReadLines(teamsFilePath);

                foreach (var line in lines)
                {
                    var info = line.Split(',');
                    var team = new Team();
                    team.Name = info[0];
                    team.FullName = info[1];
                    team.ImageURL = info[2];
                    teams.Add(team);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("[Error] DataDownloader::GetTeamsFromFile() - FAILED\n" + ex.ToString());
            }
            return teams;
        }

        public bool InsertTeams()
        {
            using (var ctx = new FootballEntities())
            {
                if (ctx.Teams.Count() == 0)
                {
                    var teams = GetTeamsFromFile();
                    using (var transaction = ctx.Database.BeginTransaction())
                    {
                        try
                        {
                            if (teams.Count > 0)
                            {
                                foreach (var t in teams)
                                {
                                    ctx.Teams.Add(t);
                                }
                                ctx.SaveChanges();
                                transaction.Commit();
                                return true;
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
            return false;
        }
    }
}