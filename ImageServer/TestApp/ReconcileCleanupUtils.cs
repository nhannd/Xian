using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class ReconcileCleanupUtils : Form
    {
        ReconcileFolderScanner scanner = null;

        public ReconcileCleanupUtils()
        {
            InitializeComponent();
        }


        private void ScanButton_Click(object sender, EventArgs e)
        {
            scanner = new ReconcileFolderScanner();
            scanner.Path = Path.Text;
            scanner.ProgressUpdated += ScannerProgressUpdated;
            scanner.Terminated += ScannerTerminated;
            StopButton.Enabled = true;

            EnableViewResultButtons(false);

            scanner.StartAsync();
            
        }

        private void EnableViewResultButtons(bool b)
        {
            ShowBackupFilesOnlyCases.Enabled = b;
            ShowStudyWasDeletedCases.Enabled = b;
            ShowStudyResentCases.Enabled = b;
            ShowUnknownProblemCases.Enabled = b;
            DeleteEmptyFoldersButton.Enabled = b;
        }

        void ScannerTerminated(object sender, EventArgs e)
        {
            ScanButton.Text = "Scan";
            StopButton.Enabled = false;

            dataGridView1.DataSource = scanner.ScanResult.Results;
            dataGridView1.Update();

            TotalScanned.Text = String.Format("{0}", scanner.ScanResult.TotalScanned);
            Skipped.Text = String.Format("{0}", scanner.ScanResult.SkippedCount);
            InSIQ.Text = String.Format("{0}", scanner.ScanResult.InSIQCount);
            Orphanned.Text = String.Format("{0}", scanner.ScanResult.TotalScanned - scanner.ScanResult.SkippedCount - scanner.ScanResult.InSIQCount);
            EmptyCount.Text = String.Format("{0}", scanner.ScanResult.EmptyCount);
            StudyDeletedCount.Text = String.Format("{0}", scanner.ScanResult.DeletedStudyCount);
            BackupOrTempOnlyCount.Text = String.Format("{0}", scanner.ScanResult.BackupOrTempOnlyCount);
            StudyWasResentCount.Text = String.Format("{0}", scanner.ScanResult.StudyWasResentCount);
            UnidentifiedCount.Text = String.Format("{0}", scanner.ScanResult.UnidentifiedCount);

            EnableViewResultButtons(true);
        }

        void ScannerProgressUpdated(object sender, EventArgs e)
        {
            ReconcileFolderScanner scanner = sender as ReconcileFolderScanner;
            progressBar1.Value = scanner.ScanResult.Progress.Percent;

            ScanButton.Text = String.Format("Scanning... {0}%", scanner.ScanResult.Progress.Percent);

            UpdateScanSummary();
        }

        private void UpdateScanSummary()
        {
            TotalScanned.Text = String.Format("{0}", scanner.ScanResult.TotalScanned);
            Skipped.Text = String.Format("{0}", scanner.ScanResult.SkippedCount);
            InSIQ.Text = String.Format("{0}", scanner.ScanResult.InSIQCount);
            Orphanned.Text = String.Format("{0}", scanner.ScanResult.TotalScanned - scanner.ScanResult.SkippedCount - scanner.ScanResult.InSIQCount);
            EmptyCount.Text = String.Format("{0}", scanner.ScanResult.EmptyCount);
            StudyDeletedCount.Text = String.Format("{0}", scanner.ScanResult.DeletedStudyCount);
            BackupOrTempOnlyCount.Text = String.Format("{0}", scanner.ScanResult.BackupOrTempOnlyCount);
            StudyWasResentCount.Text = String.Format("{0}", scanner.ScanResult.StudyWasResentCount);
            UnidentifiedCount.Text = String.Format("{0}", scanner.ScanResult.UnidentifiedCount);
        }

        private void ReconcileCleanupUtils_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (scanner!=null)
            {
                scanner.Stop();
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (scanner != null)
                scanner.Stop();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var gv = (DataGridView) sender;
            var list = gv.DataSource as List<ResultEntry>;

            if (list!=null)
            {
                var result = list[e.RowIndex];

                if (result.IsEmpty)
                {
                    e.CellStyle.BackColor = Color.Green;
                }
                else if (result.BackupFilesOnly)
                {
                    e.CellStyle.BackColor = Color.YellowGreen;
                }
                else if (result.StudyIsGone)
                {
                    e.CellStyle.BackColor = Color.Yellow;
                }
                
            }

        }


        private void DeleteEmptyFoldersButton_Click(object sender, EventArgs e)
        {
            var list = scanner.ScanResult.Results.FindAll(item => item.IsEmpty);
            progressBar1.Minimum = 0;
            progressBar1.Maximum = list.Count;
            progressBar1.Value = 0;
            try
            {
                foreach (var entry in list)
                {
                    if (Directory.Exists(entry.Path))
                    {
                        Directory.Delete(entry.Path);
                    }

                    progressBar1.Value++;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }


        private void ShowStudyResentCases_Click(object sender, EventArgs e)
        {
            var list = scanner.ScanResult.Results.FindAll(item => item.StudyWasResent);
            dataGridView1.DataSource = list;
            dataGridView1.Refresh();
            tabControl1.SelectedTab = ResultsTab;
        }

        private void ShowStudyWasDeletedCases_Click(object sender, EventArgs e)
        {
            var list = scanner.ScanResult.Results.FindAll(item => item.StudyIsGone);
            dataGridView1.DataSource = list;
            dataGridView1.Refresh();
            tabControl1.SelectedTab = ResultsTab;
        }

        private void ShowBackupFilesOnlyCases_Click(object sender, EventArgs e)
        {
            var list = scanner.ScanResult.Results.FindAll(item => item.BackupFilesOnly);
            dataGridView1.DataSource = list;
            dataGridView1.Refresh();
            tabControl1.SelectedTab = ResultsTab;
        }

        private void ShowUnknownProblemCases_Click(object sender, EventArgs e)
        {
            var list = scanner.ScanResult.Results.FindAll(item => item.NoExplanation);
            dataGridView1.DataSource = list;
            dataGridView1.Refresh();
            tabControl1.SelectedTab = ResultsTab;
        }

    }

}
