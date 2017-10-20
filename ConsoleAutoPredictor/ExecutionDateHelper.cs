using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAutoPredictor
{
    public class ExecutionDateHelper
    {
        public readonly string FILENAME = "exec.data";

        public void WriteExecutionDate()
        {
            using (var sw = new StreamWriter(FILENAME, false))
            {
                sw.Write(DateTime.Now);
            }
        }

        public DateTime GetLastExecutionDate()
        {
            if (File.Exists(FILENAME))
            {
                using (var sr = new StreamReader(FILENAME, false))
                {
                    var strDate = sr.ReadLine();
                    Console.WriteLine("Last execution date: {0}", strDate);
                    return DateTime.Parse(strDate);
                }
            }
            else
            {
                Console.WriteLine("File {0} not found. All scores will be imported", FILENAME);
                return new DateTime(1990, 1, 1);
            }
        }
    }
}
