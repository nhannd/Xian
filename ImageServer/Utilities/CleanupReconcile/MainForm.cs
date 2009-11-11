#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Utilities.CleanupReconcile
{
    public partial class MainForm : Form
    {
        #region Private Members
        private FolderScanner _scanner;
        private BackgroundTask _moveFolderBackgroundTask;
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }


        private void ScanButton_Click(object sender, EventArgs e)
        {
            _scanner = new FolderScanner();
            _scanner.Path = Path.Text;
            _scanner.ProgressUpdated += ScannerProgressUpdated;
            _scanner.Terminated += ScannerTerminated;
            StopButton.Enabled = true;

            EnableViewResultButtons(false);

            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            _scanner.StartAsync();
            
        }

        private void EnableViewResultButtons(bool b)
        {
            ShowBackupFilesOnlyCases.Enabled = b;
            ShowStudyWasDeletedCases.Enabled = b;
            ShowStudyResentCases.Enabled = b;
            ShowUnknownProblemCases.Enabled = b;
            ShowEmptyFoldersCase.Enabled = b;
            ShowStudyDoesNotExistCases.Enabled = b;
        }

        void ScannerTerminated(object sender, EventArgs e)
        {
            ScanButton.Text = "Scan";
            StopButton.Enabled = false;

            dataGridView1.DataSource = _scanner.ScanResultSet.Results;
            dataGridView1.Update();

            TotalScanned.Text = String.Format("{0}", _scanner.ScanResultSet.TotalScanned);
            Skipped.Text = String.Format("{0}", _scanner.ScanResultSet.SkippedCount);
            InSIQ.Text = String.Format("{0}", _scanner.ScanResultSet.InSIQCount);
            Orphanned.Text = String.Format("{0}", _scanner.ScanResultSet.TotalScanned - _scanner.ScanResultSet.SkippedCount - _scanner.ScanResultSet.InSIQCount);
            EmptyCount.Text = String.Format("{0}", _scanner.ScanResultSet.EmptyCount);
            StudyDeletedCount.Text = String.Format("{0}", _scanner.ScanResultSet.DeletedStudyCount);
            BackupOrTempOnlyCount.Text = String.Format("{0}", _scanner.ScanResultSet.BackupOrTempOnlyCount);
            StudyWasResentCount.Text = String.Format("{0}", _scanner.ScanResultSet.StudyWasResentCount);
            UnidentifiedCount.Text = String.Format("{0}", 
                                                   _scanner.ScanResultSet.StudyIsInWorkQueue + _scanner.ScanResultSet.UnidentifiedCount);
            StudyDoesNotExistCount.Text = String.Format("{0}", _scanner.ScanResultSet.StudyDoesNotExistCount);

            progressBar1.Value = 0;
            EnableViewResultButtons(true);
        }

        void ScannerProgressUpdated(object sender, EventArgs e)
        {
            FolderScanner scanner = sender as FolderScanner;
            progressBar1.Value = scanner.ScanResultSet.Progress.Percent;

            ScanButton.Text = String.Format("Scanning... {0}%", scanner.ScanResultSet.Progress.Percent);

            UpdateScanSummary();
        }

        private void UpdateScanSummary()
        {
            TotalScanned.Text = String.Format("{0}", _scanner.ScanResultSet.TotalScanned);
            Skipped.Text = String.Format("{0}", _scanner.ScanResultSet.SkippedCount);
            InSIQ.Text = String.Format("{0}", _scanner.ScanResultSet.InSIQCount);
            Orphanned.Text = String.Format("{0}", _scanner.ScanResultSet.TotalScanned - _scanner.ScanResultSet.SkippedCount - _scanner.ScanResultSet.InSIQCount);
            EmptyCount.Text = String.Format("{0}", _scanner.ScanResultSet.EmptyCount);
            StudyDeletedCount.Text = String.Format("{0}", _scanner.ScanResultSet.DeletedStudyCount);
            BackupOrTempOnlyCount.Text = String.Format("{0}", _scanner.ScanResultSet.BackupOrTempOnlyCount);
            StudyWasResentCount.Text = String.Format("{0}", _scanner.ScanResultSet.StudyWasResentCount);
            UnidentifiedCount.Text = String.Format("{0}", _scanner.ScanResultSet.UnidentifiedCount);
            StudyDoesNotExistCount.Text = String.Format("{0}", _scanner.ScanResultSet.StudyDoesNotExistCount);
        }

        private void ReconcileCleanupUtils_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_scanner!=null)
            {
                _scanner.Stop();
            }
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            if (_scanner != null)
                _scanner.Stop();
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
            var list = gv.DataSource as List<ScanResultEntry>;

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
                else if (result.StudyWasOnceDeleted)
                {
                    e.CellStyle.BackColor = Color.Yellow;
                }
                
            }

        }

        private void ShowStudyResentCases_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.StudyWasResent);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            dataGridView1.Refresh();
            MoveFolderButton.Enabled = true;
            tabControl1.SelectedTab = ResultsTab;
        }

        private void ShowStudyWasDeletedCases_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.StudyWasOnceDeleted);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            MoveFolderButton.Enabled = true;
            dataGridView1.Refresh();
            tabControl1.SelectedTab = ResultsTab;
        }

        private void ShowBackupFilesOnlyCases_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.BackupFilesOnly);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            dataGridView1.Refresh();
            MoveFolderButton.Enabled = true;
            tabControl1.SelectedTab = ResultsTab;
        }

        private void ShowUnknownProblemCases_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.Undetermined || item.IsInWorkQueue);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            dataGridView1.Refresh();
            MoveFolderButton.Enabled = false;
            tabControl1.SelectedTab = ResultsTab;
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var gv = (DataGridView)sender;
            gv.Sort(gv.Columns[e.ColumnIndex], System.ComponentModel.ListSortDirection.Ascending);
        }

        private void ShowStudyDoesNotExistCases_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.StudyNoLongerExists);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            dataGridView1.Refresh();
            MoveFolderButton.Enabled = true;
            tabControl1.SelectedTab = ResultsTab;
        }

        private void ShowEmptyFoldersCase_Click(object sender, EventArgs e)
        {
            var list = _scanner.ScanResultSet.Results.FindAll(item => item.IsEmpty);
            dataGridView1.DataSource = new SortableResultList<ScanResultEntry>(list);
            dataGridView1.Refresh();
            MoveFolderButton.Enabled = true;
            tabControl1.SelectedTab = ResultsTab;
        }

        private void MoveFolders(SortableResultList<ScanResultEntry> list)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Value = 0;
            _moveFolderBackgroundTask = new BackgroundTask(DoMoveFolderAsync, true, list);
            _moveFolderBackgroundTask.ProgressUpdated += delegate(object sender, BackgroundTaskProgressEventArgs ev)
                                                             {
                                                                 progressBar1.Value = ev.Progress.Percent;
                                                             };

            _moveFolderBackgroundTask.Terminated += delegate
                                                        {
                                                            progressBar1.Value = 0;
                                                            MoveFolderButton.Text = "Move Folders";
                                                            MoveFolderButton.Enabled = true;
                                                        };

            MoveFolderButton.Text = "Stop";
            _moveFolderBackgroundTask.Run();
        }

        private void DoMoveFolderAsync(IBackgroundTaskContext context)
        {
            var list = context.UserState as SortableResultList<ScanResultEntry>;
            int entryCount = list.Count;
            int counter = 0;
            foreach (ScanResultEntry entry in list)
            {
                if (context.CancelRequested)
                    break;

                DirectoryInfo dir = new DirectoryInfo(entry.Path);
                if (dir.Exists)
                {
                    string path = System.IO.Path.Combine(folderBrowserDialog1.SelectedPath, dir.Name);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    DirectoryUtility.Move(entry.Path, path);
                    counter++;
                    context.ReportProgress(new BackgroundTaskProgress(counter*100/entryCount, String.Empty));
                }

            }
        }

        private void MoveFolderButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;

            if (button.Text != "Stop")
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    var list = dataGridView1.DataSource as SortableResultList<ScanResultEntry>;
                    MoveFolders(list);
                }
            }
            else
            {
                MoveFolderButton.Text = "Stopping...";
                MoveFolderButton.Enabled = false;
                _moveFolderBackgroundTask.RequestCancel();
            }
        }
    }
}