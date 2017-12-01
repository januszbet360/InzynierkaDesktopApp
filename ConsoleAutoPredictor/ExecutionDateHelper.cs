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
        public static readonly string FILENAME = "exec.data";

        // TODO - change it to DateTime min value before deploying
        public static readonly DateTime DEFAULT_LAST_DATE = DateTime.Now.AddDays(-7);

        public static void SetExecutionDate()
        {
            using (var sw = new StreamWriter(FILENAME, false))
            {
                sw.Write(DateTime.Now);
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
                        return DateTime.Parse(strDate);
                    else
                        return DEFAULT_LAST_DATE;
                }
            }
            else
            {
                Console.WriteLine("File {0} not found. All scores will be imported", FILENAME);
                return DEFAULT_LAST_DATE;
            }
        }
    }
}
