namespace ClearCanvas.ImageServer.TestApp
{
    partial class ReconcileCleanupUtils
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Path = new System.Windows.Forms.TextBox();
            this.ScanButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.StopButton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.SummaryTab = new System.Windows.Forms.TabPage();
            this.ShowBackupFilesOnlyCases = new System.Windows.Forms.Button();
            this.ShowStudyWasDeletedCases = new System.Windows.Forms.Button();
            this.ShowUnknownProblemCases = new System.Windows.Forms.Button();
            this.ShowStudyResentCases = new System.Windows.Forms.Button();
            this.DeleteEmptyFoldersButton = new System.Windows.Forms.Button();
            this.StudyWasResentCount = new System.Windows.Forms.Label();
            this.StudyDeletedCount = new System.Windows.Forms.Label();
            this.BackupOrTempOnlyCount = new System.Windows.Forms.Label();
            this.EmptyCount = new System.Windows.Forms.Label();
            this.UnidentifiedCount = new System.Windows.Forms.Label();
            this.Orphanned = new System.Windows.Forms.Label();
            this.InSIQ = new System.Windows.Forms.Label();
            this.Skipped = new System.Windows.Forms.Label();
            this.TotalScanned = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.ResultsTab = new System.Windows.Forms.TabPage();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.SummaryTab.SuspendLayout();
            this.ResultsTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // Path
            // 
            this.Path.Location = new System.Drawing.Point(38, 38);
            this.Path.Name = "Path";
            this.Path.Size = new System.Drawing.Size(419, 20);
            this.Path.TabIndex = 0;
            this.Path.Text = "\\\\172.16.10.221\\dicom_01\\JDMIPACS\\Reconcile";
            // 
            // ScanButton
            // 
            this.ScanButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ScanButton.Location = new System.Drawing.Point(484, 17);
            this.ScanButton.Name = "ScanButton";
            this.ScanButton.Size = new System.Drawing.Size(130, 41);
            this.ScanButton.TabIndex = 1;
            this.ScanButton.Text = "Scan";
            this.ScanButton.UseVisualStyleBackColor = true;
            this.ScanButton.Click += new System.EventHandler(this.ScanButton_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 486);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(768, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // StopButton
            // 
            this.StopButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StopButton.Location = new System.Drawing.Point(637, 17);
            this.StopButton.Name = "StopButton";
            this.StopButton.Size = new System.Drawing.Size(93, 41);
            this.StopButton.TabIndex = 1;
            this.StopButton.Text = "Stop";
            this.StopButton.UseVisualStyleBackColor = true;
            this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(754, 454);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            this.dataGridView1.Click += new System.EventHandler(this.dataGridView1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.SummaryTab);
            this.tabControl1.Controls.Add(this.ResultsTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(768, 486);
            this.tabControl1.TabIndex = 5;
            // 
            // SummaryTab
            // 
            this.SummaryTab.Controls.Add(this.ShowBackupFilesOnlyCases);
            this.SummaryTab.Controls.Add(this.StopButton);
            this.SummaryTab.Controls.Add(this.ShowStudyWasDeletedCases);
            this.SummaryTab.Controls.Add(this.ScanButton);
            this.SummaryTab.Controls.Add(this.ShowUnknownProblemCases);
            this.SummaryTab.Controls.Add(this.Path);
            this.SummaryTab.Controls.Add(this.ShowStudyResentCases);
            this.SummaryTab.Controls.Add(this.DeleteEmptyFoldersButton);
            this.SummaryTab.Controls.Add(this.StudyWasResentCount);
            this.SummaryTab.Controls.Add(this.StudyDeletedCount);
            this.SummaryTab.Controls.Add(this.BackupOrTempOnlyCount);
            this.SummaryTab.Controls.Add(this.EmptyCount);
            this.SummaryTab.Controls.Add(this.UnidentifiedCount);
            this.SummaryTab.Controls.Add(this.Orphanned);
            this.SummaryTab.Controls.Add(this.InSIQ);
            this.SummaryTab.Controls.Add(this.Skipped);
            this.SummaryTab.Controls.Add(this.TotalScanned);
            this.SummaryTab.Controls.Add(this.label4);
            this.SummaryTab.Controls.Add(this.label9);
            this.SummaryTab.Controls.Add(this.label7);
            this.SummaryTab.Controls.Add(this.label10);
            this.SummaryTab.Controls.Add(this.label6);
            this.SummaryTab.Controls.Add(this.label5);
            this.SummaryTab.Controls.Add(this.label8);
            this.SummaryTab.Controls.Add(this.label3);
            this.SummaryTab.Controls.Add(this.label2);
            this.SummaryTab.Controls.Add(this.label1);
            this.SummaryTab.Location = new System.Drawing.Point(4, 22);
            this.SummaryTab.Name = "SummaryTab";
            this.SummaryTab.Padding = new System.Windows.Forms.Padding(3);
            this.SummaryTab.Size = new System.Drawing.Size(760, 460);
            this.SummaryTab.TabIndex = 2;
            this.SummaryTab.Text = "Summary";
            this.SummaryTab.UseVisualStyleBackColor = true;
            // 
            // ShowBackupFilesOnlyCases
            // 
            this.ShowBackupFilesOnlyCases.Enabled = false;
            this.ShowBackupFilesOnlyCases.Location = new System.Drawing.Point(642, 185);
            this.ShowBackupFilesOnlyCases.Name = "ShowBackupFilesOnlyCases";
            this.ShowBackupFilesOnlyCases.Size = new System.Drawing.Size(99, 23);
            this.ShowBackupFilesOnlyCases.TabIndex = 8;
            this.ShowBackupFilesOnlyCases.Text = "View";
            this.ShowBackupFilesOnlyCases.UseVisualStyleBackColor = true;
            this.ShowBackupFilesOnlyCases.Click += new System.EventHandler(this.ShowBackupFilesOnlyCases_Click);
            // 
            // ShowStudyWasDeletedCases
            // 
            this.ShowStudyWasDeletedCases.Enabled = false;
            this.ShowStudyWasDeletedCases.Location = new System.Drawing.Point(642, 216);
            this.ShowStudyWasDeletedCases.Name = "ShowStudyWasDeletedCases";
            this.ShowStudyWasDeletedCases.Size = new System.Drawing.Size(99, 23);
            this.ShowStudyWasDeletedCases.TabIndex = 8;
            this.ShowStudyWasDeletedCases.Text = "View";
            this.ShowStudyWasDeletedCases.UseVisualStyleBackColor = true;
            this.ShowStudyWasDeletedCases.Click += new System.EventHandler(this.ShowStudyWasDeletedCases_Click);
            // 
            // ShowUnknownProblemCases
            // 
            this.ShowUnknownProblemCases.Enabled = false;
            this.ShowUnknownProblemCases.Location = new System.Drawing.Point(642, 274);
            this.ShowUnknownProblemCases.Name = "ShowUnknownProblemCases";
            this.ShowUnknownProblemCases.Size = new System.Drawing.Size(99, 23);
            this.ShowUnknownProblemCases.TabIndex = 8;
            this.ShowUnknownProblemCases.Text = "View";
            this.ShowUnknownProblemCases.UseVisualStyleBackColor = true;
            this.ShowUnknownProblemCases.Click += new System.EventHandler(this.ShowUnknownProblemCases_Click);
            // 
            // ShowStudyResentCases
            // 
            this.ShowStudyResentCases.Enabled = false;
            this.ShowStudyResentCases.Location = new System.Drawing.Point(642, 244);
            this.ShowStudyResentCases.Name = "ShowStudyResentCases";
            this.ShowStudyResentCases.Size = new System.Drawing.Size(99, 23);
            this.ShowStudyResentCases.TabIndex = 8;
            this.ShowStudyResentCases.Text = "View";
            this.ShowStudyResentCases.UseVisualStyleBackColor = true;
            this.ShowStudyResentCases.Click += new System.EventHandler(this.ShowStudyResentCases_Click);
            // 
            // DeleteEmptyFoldersButton
            // 
            this.DeleteEmptyFoldersButton.Enabled = false;
            this.DeleteEmptyFoldersButton.Location = new System.Drawing.Point(642, 304);
            this.DeleteEmptyFoldersButton.Name = "DeleteEmptyFoldersButton";
            this.DeleteEmptyFoldersButton.Size = new System.Drawing.Size(99, 23);
            this.DeleteEmptyFoldersButton.TabIndex = 8;
            this.DeleteEmptyFoldersButton.Text = "Delete";
            this.DeleteEmptyFoldersButton.UseVisualStyleBackColor = true;
            this.DeleteEmptyFoldersButton.Click += new System.EventHandler(this.DeleteEmptyFoldersButton_Click);
            // 
            // StudyWasResentCount
            // 
            this.StudyWasResentCount.AutoSize = true;
            this.StudyWasResentCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StudyWasResentCount.Location = new System.Drawing.Point(575, 247);
            this.StudyWasResentCount.Name = "StudyWasResentCount";
            this.StudyWasResentCount.Size = new System.Drawing.Size(21, 20);
            this.StudyWasResentCount.TabIndex = 7;
            this.StudyWasResentCount.Text = "...";
            // 
            // StudyDeletedCount
            // 
            this.StudyDeletedCount.AutoSize = true;
            this.StudyDeletedCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StudyDeletedCount.Location = new System.Drawing.Point(575, 216);
            this.StudyDeletedCount.Name = "StudyDeletedCount";
            this.StudyDeletedCount.Size = new System.Drawing.Size(21, 20);
            this.StudyDeletedCount.TabIndex = 7;
            this.StudyDeletedCount.Text = "...";
            // 
            // BackupOrTempOnlyCount
            // 
            this.BackupOrTempOnlyCount.AutoSize = true;
            this.BackupOrTempOnlyCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackupOrTempOnlyCount.Location = new System.Drawing.Point(575, 185);
            this.BackupOrTempOnlyCount.Name = "BackupOrTempOnlyCount";
            this.BackupOrTempOnlyCount.Size = new System.Drawing.Size(21, 20);
            this.BackupOrTempOnlyCount.TabIndex = 7;
            this.BackupOrTempOnlyCount.Text = "...";
            // 
            // EmptyCount
            // 
            this.EmptyCount.AutoSize = true;
            this.EmptyCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EmptyCount.Location = new System.Drawing.Point(575, 307);
            this.EmptyCount.Name = "EmptyCount";
            this.EmptyCount.Size = new System.Drawing.Size(21, 20);
            this.EmptyCount.TabIndex = 7;
            this.EmptyCount.Text = "...";
            // 
            // UnidentifiedCount
            // 
            this.UnidentifiedCount.AutoSize = true;
            this.UnidentifiedCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UnidentifiedCount.Location = new System.Drawing.Point(575, 277);
            this.UnidentifiedCount.Name = "UnidentifiedCount";
            this.UnidentifiedCount.Size = new System.Drawing.Size(21, 20);
            this.UnidentifiedCount.TabIndex = 7;
            this.UnidentifiedCount.Text = "...";
            // 
            // Orphanned
            // 
            this.Orphanned.AutoSize = true;
            this.Orphanned.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Orphanned.Location = new System.Drawing.Point(575, 145);
            this.Orphanned.Name = "Orphanned";
            this.Orphanned.Size = new System.Drawing.Size(21, 20);
            this.Orphanned.TabIndex = 7;
            this.Orphanned.Text = "...";
            // 
            // InSIQ
            // 
            this.InSIQ.AutoSize = true;
            this.InSIQ.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InSIQ.Location = new System.Drawing.Point(163, 198);
            this.InSIQ.Name = "InSIQ";
            this.InSIQ.Size = new System.Drawing.Size(21, 20);
            this.InSIQ.TabIndex = 7;
            this.InSIQ.Text = "...";
            // 
            // Skipped
            // 
            this.Skipped.AutoSize = true;
            this.Skipped.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Skipped.Location = new System.Drawing.Point(163, 172);
            this.Skipped.Name = "Skipped";
            this.Skipped.Size = new System.Drawing.Size(21, 20);
            this.Skipped.TabIndex = 7;
            this.Skipped.Text = "...";
            // 
            // TotalScanned
            // 
            this.TotalScanned.AutoSize = true;
            this.TotalScanned.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalScanned.Location = new System.Drawing.Point(163, 144);
            this.TotalScanned.Name = "TotalScanned";
            this.TotalScanned.Size = new System.Drawing.Size(0, 20);
            this.TotalScanned.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(35, 172);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(110, 20);
            this.label4.TabIndex = 6;
            this.label4.Text = "Skipped (new)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(332, 247);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(215, 20);
            this.label9.TabIndex = 6;
            this.label9.Text = "Study Was Resent Afterward";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(332, 216);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(219, 20);
            this.label7.TabIndex = 6;
            this.label7.Text = "Study Was Deleted Afterward";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(35, 17);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(141, 16);
            this.label10.TabIndex = 6;
            this.label10.Text = "Reconcile Folder Path";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(332, 185);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(179, 20);
            this.label6.TabIndex = 6;
            this.label6.Text = "Only Backup/Temp Files";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(332, 307);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 20);
            this.label5.TabIndex = 6;
            this.label5.Text = "Empty";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(332, 277);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(190, 20);
            this.label8.TabIndex = 6;
            this.label8.Text = "Good/Unknown Problems";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(294, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(225, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Folders that are not In SIQ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(35, 198);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Found In SIQ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(35, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Total Scanned";
            // 
            // ResultsTab
            // 
            this.ResultsTab.Controls.Add(this.dataGridView1);
            this.ResultsTab.Location = new System.Drawing.Point(4, 22);
            this.ResultsTab.Name = "ResultsTab";
            this.ResultsTab.Padding = new System.Windows.Forms.Padding(3);
            this.ResultsTab.Size = new System.Drawing.Size(760, 460);
            this.ResultsTab.TabIndex = 0;
            this.ResultsTab.Text = "Results";
            this.ResultsTab.UseVisualStyleBackColor = true;
            // 
            // ReconcileCleanupUtils
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 509);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.progressBar1);
            this.Name = "ReconcileCleanupUtils";
            this.Text = "ReconcileCleanupUtils";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ReconcileCleanupUtils_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.SummaryTab.ResumeLayout(false);
            this.SummaryTab.PerformLayout();
            this.ResultsTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox Path;
        private System.Windows.Forms.Button ScanButton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button StopButton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage ResultsTab;
        private System.Windows.Forms.TabPage SummaryTab;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label TotalScanned;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label StudyDeletedCount;
        private System.Windows.Forms.Label BackupOrTempOnlyCount;
        private System.Windows.Forms.Label EmptyCount;
        private System.Windows.Forms.Label Orphanned;
        private System.Windows.Forms.Label InSIQ;
        private System.Windows.Forms.Label Skipped;
        private System.Windows.Forms.Label UnidentifiedCount;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label StudyWasResentCount;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button DeleteEmptyFoldersButton;
        private System.Windows.Forms.Button ShowStudyResentCases;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button ShowStudyWasDeletedCases;
        private System.Windows.Forms.Button ShowBackupFilesOnlyCases;
        private System.Windows.Forms.Button ShowUnknownProblemCases;
        private System.Windows.Forms.Label label10;
    }
}