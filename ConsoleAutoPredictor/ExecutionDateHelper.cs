using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAutoPredictor
{
    public static class ExecutionDateHelper
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ExecutionDateHelper));


        public static readonly string FILENAME = "exec.data";

        // TODO - change it to DateTime min value before deploying
        public static readonly DateTime DEFAULT_LAST_DATE = DateTime.Now.AddDays(-7);

        public static void SetExecutionDate()
        {
            using (var sw = new StreamWriter(FILENAME, false))
            {
                var d = DateTime.Now;
                sw.Write(d);
                logger.InfoFormat("Date {0} has been set as last execution date to file: {1}", d, FILENAME);
            }
        }

        public static DateTime GetLastExecutionDate()
        {
            if (File.Exists(FILENAME))
            {
                using (var sr = new StreamReader(FILENAME, false))
                {
                    var strDate = sr.ReadLine();
                    DateTime date;
                    Console.WriteLine("Last execution date: {0}", strDate);
                    if (DateTime.TryParse(strDate, out date))
                    {
                        logger.Info("Method GetLastExecutionDate is returning: " + strDate);
                        return DateTime.Parse(strDate);
                    }
                    else
                    {
                        logger.Info("Method GetLastExecutionDate is returning: " + DEFAULT_LAST_DATE);
                        return DEFAULT_LAST_DATE;
                    }
                }
            }
            else
            {
                Console.WriteLine("File {0} not found. All scores will be imported", FILENAME);
                logger.Warn("Last execution date file not found. All scores are going to be imported to database");
                return DEFAULT_LAST_DATE;
            }
        }
    }
}
