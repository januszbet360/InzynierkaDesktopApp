using CsvHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataModel;
using DataDownloader;

namespace PredictiveSystemManagement
{
    public partial class MainForm : Form
    {
        List<FileDialog> listOfCsvFiles = new List<FileDialog>();

        public MainForm()
        {
            InitializeComponent();
            SetAppearance();

            // Teams are inserted only if table "Teams" is null
            if (new TeamDataService().InsertTeams())
            {
                Log("Tabela zespołów została zaktualizowana");
            }
        }

        private void SetAppearance()
        {
            //HistoricalDataPanel
            HistoricalDataPanel.Size = new Size(Constants.WidthHD, Constants.HeightHD);

            HistoricalDataGroupBox.Size = new Size(Constants.WidthGroupBoxHD, Constants.HeightGroupBoxHD);
            HistoricalDataGroupBox.Location = new Point(
                (HistoricalDataPanel.Width - HistoricalDataGroupBox.Width) / 2,
                (HistoricalDataPanel.Height - HistoricalDataGroupBox.Height) / 2);

            FilesListLabel.Location = new Point(Constants.MarginLeftLabelHD, Constants.MarginTopLabelHD);

            FilesListBox.Size = new Size(Constants.WidthFilesListBoxHD, Constants.HeightFilesListBoxHD);
            FilesListBox.Location = new Point(Constants.MarginLeftFilesListBoxHD, Constants.MarginTopFilesListBoxHD);

            ProcessFilesButton.Size = new Size(Constants.WidthProcessFilesButtonHD, Constants.HeightProcessFilesButtonHD);
            ProcessFilesButton.Location = new Point(Constants.MarginLeftProcessFilesButtonHD, Constants.MarginTopProcessFilesButtonHD);

            AddFileButton.Size = new Size(Constants.WidthAddFileButtonHD, Constants.HeightAddFileButtonHD);
            AddFileButton.Location = new Point(Constants.MarginLeftAddFileButtonHD, Constants.MarginTopAddFileButtonHD);

            RemoveFileButton.Size = new Size(Constants.WidthRemoveFileButtonHD, Constants.HeightRemoveFileButtonHD);
            RemoveFileButton.Location = new Point(Constants.MarginLeftRemoveFileButtonHD, Constants.MarginTopRemoveFileButtonHD);

            //////////////////////////////////////////////////////////////////////
            //ManagementSystemPanel
            ManagementSystemPanel.Size = new Size(Constants.WidthMS, Constants.HeightMS);

            ManagementSystemGroupBox.Size = new Size(Constants.WidthGroupBoxMS, Constants.HeightGroupBoxMS);
            ManagementSystemGroupBox.Location = new Point(
                (ManagementSystemPanel.Width - ManagementSystemGroupBox.Width) / 2,
                (ManagementSystemPanel.Height - ManagementSystemGroupBox.Height) / 2);

            OutputTextBox.Size = new Size(Constants.WidthOutputTextBoxMS, Constants.HeightOutputTextBoxMS);
            OutputTextBox.Location = new Point(
                (ManagementSystemGroupBox.Width - OutputTextBox.Width) / 2,
                (ManagementSystemGroupBox.Height - OutputTextBox.Height - 20) );
            OutputTextBox.ScrollBars = ScrollBars.Vertical;
            OutputTextBox.ReadOnly = true;
            //////////////////////////////////////////////////////////////////////
        }

        private void Log(string message)
        {
            OutputTextBox.Text = "[" + DateTime.Now + "] " + message + Environment.NewLine + OutputTextBox.Text;
        }
        
        private bool CheckCsvFileInList(FileDialog file)
        {
            foreach (FileDialog listElement in listOfCsvFiles)
            {
                if (file.FileName == listElement.FileName)
                {
                    return false;
                }
            }
            return true;
        }
        
        private void ProcessCsvFile(string csvFile)
        {
            try
            {
                int m = new CsvService().InsertMatches(csvFile);
                Log(string.Format("Baza danych została zaktualizowana. {0} nowych rekordów w tablicy 'Matches'.", m));

                int s = new CsvService().InsertScores(csvFile);
                Log(string.Format("Baza danych została zaktualizowana. {0} nowych rekordów w tablicy 'Scores'.", s));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wybrany plik: " + csvFile + "\nzawiera niekompletne dane. Upewnij się, że wybrano prawidłowy plik.", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Error ProcessCsvFile: " + csvFile + "\n" + ex.Message);
            }
        }

        private void CheckMatchweek()
        {
            Log(string.Format("Następuje sprawdzenie aktualnej kolejki sezonu: {0}", 
                DataDownloader.SeasonHelper.GetCurrentSeason(DateTime.Now)));
            
            // Update of CSV
            new CsvDownloader().GetScoresCsv(DateTime.Now);
            Log("Zaktualizowane plik z danymi.");

            ProcessCsvFile(Path.Combine(Application.StartupPath, DataDownloader.Constants.CSV_CURRENT_FILE_NAME));
            Log("Aktualizowanie bazy danych zakończone");
        }

        
        private void AddFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog newFileDialog = new OpenFileDialog
            {
                Title = "Wybierz plik CSV z wynikami",
                InitialDirectory = Application.StartupPath,
                Filter = "Pliki CSV|*.csv"
            };

            if (newFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (CheckCsvFileInList(newFileDialog))
                {
                    listOfCsvFiles.Add(newFileDialog);
                    FilesListBox.Items.Add(Path.GetFileName(newFileDialog.FileName));
                }
                else
                {
                    MessageBox.Show("Wybrany plik znajduje się już na liście plików.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void RemoveFileButton_Click(object sender, EventArgs e) 
        {
            var selectedFile = FilesListBox.SelectedIndex;
            if (selectedFile.ToString() != "-1")
            {
                listOfCsvFiles.RemoveAt(selectedFile);
                FilesListBox.Items.RemoveAt(selectedFile);
            }
            else
            {
                MessageBox.Show("W celu usunięcia pliku z listy, należy go zaznaczyć.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ProcessFilesButton_Click(object sender, EventArgs e)
        {
            if (listOfCsvFiles.Count > 0)
            {
                string information = "Przetwarzanie plików:\n";
                foreach (FileDialog listElement in listOfCsvFiles)
                {
                    ProcessCsvFile(listElement.FileName);
                    information += (">" + Path.GetFileName(listElement.FileName) + "\n");
                }
                MessageBox.Show(information + "zakończone sukcesem. Dane zostały dodane do bazy danych: " + Constants.NameOfDatabase, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Nie wybrano żadnych plików. W celu przetworzenia danych historycznych, należy wybrać odpowiednie pliki z dysku.", "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void CheckMatchweekButton_Click(object sender, EventArgs e)
        {
            CheckMatchweek();
        }
    }
}