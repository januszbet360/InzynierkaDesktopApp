using DataDownloader;
using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FootballPredictor.Models
{
    public class MainPageModel
    {
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public PredictedScore Score { get; set; }
        public string Fixtures { get; set; }

        public MainPageModel(string home, string away, PredictedScore score)
        {
            HomeTeam = home;
            AwayTeam = away;
            Score = score;
        }
    }
}