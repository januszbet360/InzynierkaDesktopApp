using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Filters;
using Accord.Statistics.Kernels;
using CsvHelper;
using DataModel;
using DataModel.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataDownloader.Prediction
{
    public class PredictingMachine
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PredictingMachine));

        public MulticlassSupportVectorMachine<Gaussian> Predictor { get; set; }
        
        public Dictionary<Tuple<int, int>, int> Codebook { get; set; }

        public IEnumerable<Score> _scores;
        protected double[][] _input = new double[9][];
        protected int[] _output;

        public PredictingMachine()
        {
            var creator = new TrainingSetCreator();
            _scores = creator.GetTrainingScores();
            Codebook = creator.CreateScoresDictionary(_scores);

            Learn();
        }

        public PredictingMachine(DateTime date)
        {
            var creator = new TrainingSetCreator();
            _scores = creator.GetTrainingScores(date);
            Codebook = creator.CreateScoresDictionary(_scores);

            Learn();
        }

        protected void LoadTrainingSet()
        {
            try
            {
                var creator = new TrainingSetCreator();
                _input = creator.CreateTrainingSetInputs(_scores);
                _output = creator.CreateTrainingSetOutputs(_scores, Codebook);
                logger.Info("Training set loaded");
            }
            catch (Exception e)
            {
                logger.Error("Error while loading training set.", e);
            }
        }

        protected void Learn()
        {
            LoadTrainingSet();

            try
            {
                var teacher = new MulticlassSupportVectorLearning<Gaussian>()
                {
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {

                    }
                };

                logger.Info("Learning object created.");
                Predictor = teacher.Learn(_input, _output);
                logger.Info("Learning completed.");
            }
            catch (Exception e)
            {
                logger.Error("Error while teaching the machine.", e);
            }
        }


        public Tuple<int, int> PredictScore(RatioModel ratio)
        {
            try
            {
                var input = new double[] { ratio.HOR, ratio.HDR, ratio.AOR,
                ratio.ADR, ratio.HORH, ratio.HDRH, ratio.AORA, ratio.ADRA };

                //double[] input = Codebook.Translate(query.ToArray()).ToDouble();
                int result = Predictor.Decide(input);
                var decodedResult = Codebook.First(x => x.Value == result).Key;
                
                return decodedResult;
            }
            catch (Exception e)
            {
                logger.Error("Error while predicting score.", e);
                return null;
            }
        }
    }
}