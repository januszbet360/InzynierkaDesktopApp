using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models
{
    public class PredictionArchiveScoreModel
    {
        public int Goals { get; set; }
        public int Shots { get; set; }
        public int ShotsOnTarget { get; set; }
        public double OpponentRatio { get; set; }

        public PredictionArchiveScoreModel(int g, int s, int st, int or)
        {
            Goals = g;
            Shots = s;
            ShotsOnTarget = st;
            OpponentRatio = or;
        }
    }
}
