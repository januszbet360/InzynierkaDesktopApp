using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PredictiveSystemManagement
{
    public class Report
    {
        private string GenerateTextOfReport()
        {
            try
            {
                string separateLine = "**************************************************************************************";
                string bestSeason = "";
                double bestEffectiveness = 0.0;

                string textOfReport = separateLine +
                    String.Format("\n*** | Dnia: {0} | Godzina: {1} | ********************************************",
                        DateTime.Now.ToString("dd-MM-yyyy"), DateTime.Now.ToString("HH:mm")) +
                    "\n" + separateLine +
                    "\n\n\t\t Raport zbiorczy skuteczności systemu predykcji: " +
                    "\n\t\t\t\t " + Constants.SystemPredictiveName +
                    "\n\n" + separateLine;

                try
                {
                    DateTime currentDate = DateTime.Now;
                    int emptySeason = 0;
                    while (true)
                    {
                        string currentSeason = DataDownloader.SeasonHelper.GetCurrentSeason(currentDate);
                        double currentEffectiveness = new DataDownloader.ScoreEffectivenessService().Compute(currentSeason) * 100.0;
                        double currentEffectivenessWeighted = new DataDownloader.ScoreEffectivenessService().ComputeWeighted(currentSeason) * 100.0;

                        if (currentEffectiveness > bestEffectiveness)
                        {
                            bestEffectiveness = currentEffectiveness;
                            bestSeason = currentSeason;
                        }
                        
                        currentDate = currentDate.AddYears(-1);
                        if (currentEffectiveness == 0)
                        {
                            emptySeason++;
                            continue;
                        }

                        textOfReport += String.Format("\n\n\t Skuteczność predykcji sezonu {0}: {1}% ({2}%)", 
                            currentSeason, Math.Round(currentEffectiveness, 2), Math.Round(currentEffectivenessWeighted, 2));

                        for (int currentMachweek = 1; currentMachweek <= 38; currentMachweek++)
                        {
                            try
                            {
                                double currentMatchweekEffectiveness = new DataDownloader.ScoreEffectivenessService().Compute(currentMachweek, currentSeason) * 100.0;
                                double currentMatchweekEffectivenessWeighted = new DataDownloader.ScoreEffectivenessService().ComputeWeighted(currentMachweek, currentSeason) * 10.0;

                                if (currentMatchweekEffectiveness == 0.0)
                                {
                                    textOfReport += String.Format("\n\t\t- kolejka {0}: - -", currentMachweek);
                                }
                                else
                                {
                                    textOfReport += String.Format("\n\t\t- kolejka {0}: {1}% ({2})",
                                        currentMachweek, Math.Round(currentMatchweekEffectiveness, 2), Math.Round(currentMatchweekEffectivenessWeighted, 2));
                                }
                            }
                            catch (Exception)
                            {
                                break;
                            }
                        }

                        if (emptySeason > 2)
                        {
                            break;
                        }
                    }
                }
                catch(Exception)
                { }

                textOfReport += "\n\n" + separateLine +
                "\n" +
                String.Format("\n\t Ogólna skuteczność systemu predykcji: {0}%", Math.Round(new DataDownloader.ScoreEffectivenessService().Compute() * 100.0, 4)) +
                String.Format("\n\t Najlepsza skuteczność predykcji wystąpiła w sezonie {0}: {1}%", bestSeason, Math.Round(bestEffectiveness, 2)) +
                "\n" +
                "\n" + separateLine +
                String.Format("\n************************** Copyrights (C) 2017 {0} ****************************",
                    Constants.SystemPredictiveName) +
                "\n" + separateLine + "\n";
                
                textOfReport = textOfReport.Replace("\n", Environment.NewLine);
                return textOfReport;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public bool GenerateReport()
        {
            try
            {
                string reportFileName = Constants.ReportFileName + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt";
                string reportFilePath = Path.Combine(Application.StartupPath, reportFileName);

                string textOfReport = GenerateTextOfReport();
                if (textOfReport == "")
                {
                    throw new Exception();
                }

                File.WriteAllText(reportFilePath, textOfReport);

                DialogResult dialogResult = MessageBox.Show("Raport został wygenerowany. Czy chcesz go teraz otworzyć?", "Raport zbiorczy", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Process.Start("notepad.exe", reportFilePath);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}