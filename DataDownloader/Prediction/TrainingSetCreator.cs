using CsvHelper;
using DataModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Prediction
{
    public class TrainingSetCreator
    {
        public static readonly int TRAINING_SET_CARDINALITY = 500;

        public static readonly string[] INPUT_COLUMNS = { "HOR", "HDR", "AOR", "ADR", "HORH", "HDRH",
            "AORA", "ADRA" };


        public List<Score> GetTrainingScores()
        {
            return GetTrainingScores(DateTime.Now);
        }

        public List<Score> GetTrainingScores(string season, int matchweek)
        {
            using (var ctx = new FootballEntities())
            {
                var last = ctx.Matches.FirstOrDefault(m => m.Matchweek == matchweek && m.Season == season);

                if (last != null)
                {
                    return GetTrainingScores(last.Date);
                }
                else
                {
                    return GetTrainingScores();
                }
            }

        }

        public List<Score> GetTrainingScores(DateTime date)
        {
            using (var ctx = new FootballEntities())
            {
                var result = new List<Score>();
                var dbScores = ctx.Scores
                    .Where(s => s.Date < date)
                    .OrderByDescending(s => s.Date)
                    .Take(TRAINING_SET_CARDINALITY);

                RatioCalculator rc = new RatioCalculator();

                foreach (var s in dbScores)
                {
                    if (s.HOR == null)
                    {
                        result.Add(rc.SetRatio(s));
                    }
                    else
                        result.Add(s);
                }
                return result;
            }
        }

        public double[][] CreateTrainingSetInputs(IEnumerable<Score> values)
        {
            List<double[]> list = new List<double[]>();

            foreach (var v in values)
            {                
                double[] rt = new double[8];
                rt[0] = (double)v.HOR;
                rt[1] = (double)v.HDR;
                rt[2] = (double)v.AOR;
                rt[3] = (double)v.ADR;
                rt[4] = (double)v.HORH;
                rt[5] = (double)v.HDRH;
                rt[6] = (double)v.AORA;
                rt[7] = (double)v.ADRA;

                list.Add(rt);
            }

            return list.ToArray();
        }

        public int[] CreateTrainingSetOutputs(IEnumerable<Score> values, IDictionary<Tuple<int,int>, int> codebook)
        {
            var list = new List<int>();
                 
            foreach (var v in values)
            {
                int code;

                if (codebook.TryGetValue(new Tuple<int, int>(v.HomeGoals, v.AwayGoals), out code))
                {
                    list.Add(code);
                }
            }

            return list.ToArray();
        }

        public Dictionary<Tuple<int, int>, int> CreateScoresDictionary(IEnumerable<Score> values)
        {
            var dict = new Dictionary<Tuple<int, int>, int>();
            var distinctScores = values.Select(v => new { v.HomeGoals, v.AwayGoals }).Distinct().ToList();

            int counter = 0;
            foreach (var s in distinctScores)
            {
                dict.Add(new Tuple<int, int>(s.HomeGoals, s.AwayGoals), counter);
                counter++;
            }

            return dict;
        }
    }
}