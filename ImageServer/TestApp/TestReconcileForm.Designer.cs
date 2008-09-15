namespace ClearCanvas.ImageServer.TestApp
{
    partial class TestReconcileForm
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.Images = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.PatientsName = new System.Windows.Forms.TextBox();
            this.PatientsId = new System.Windows.Forms.TextBox();
            this.IssuerOfPatientsId = new System.Windows.Forms.TextBox();
            this.PatientsSex = new System.Windows.Forms.TextBox();
            this.PatientsBirthdate = new System.Windows.Forms.TextBox();
            this.StudyInstanceUid = new System.Windows.Forms.TextBox();
            this.StudyDate = new System.Windows.Forms.TextBox();
            this.AccessionNumber = new System.Windows.Forms.TextBox();
            this.Send = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ServerAE = new System.Windows.Forms.TextBox();
            this.ServerPort = new System.Windows.Forms.TextBox();
            this.ServerHost = new System.Windows.Forms.TextBox();
            this.LocalAE = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SeriesTab = new System.Windows.Forms.TabPage();
            this.SeriesTreeView = new System.Windows.Forms.TreeView();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SeriesTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Images
            // 
            this.Images.Location = new System.Drawing.Point(425, 16);
            this.Images.Name = "Images";
            this.Images.Size = new System.Drawing.Size(105, 23);
            this.Images.TabIndex = 0;
            this.Images.Text = "Load Images ...";
            this.Images.UseVisualStyleBackColor = true;
            this.Images.Click += new System.EventHandler(this.Images_Click);
            // 
            // PatientsName
            // 
            this.PatientsName.Location = new System.Drawing.Point(142, 37);
            this.PatientsName.Name = "PatientsName";
            this.PatientsName.Size = new System.Drawing.Size(100, 20);
            this.PatientsName.TabIndex = 1;
            this.PatientsName.Text = "John^Smith";
            // 
            // PatientsId
            // 
            this.PatientsId.Location = new System.Drawing.Point(142, 64);
            this.PatientsId.Name = "PatientsId";
            this.PatientsId.Size = new System.Drawing.Size(100, 20);
            this.PatientsId.TabIndex = 2;
            this.PatientsId.Text = "PATID-1001";
            // 
            // IssuerOfPatientsId
            // 
            this.IssuerOfPatientsId.Location = new System.Drawing.Point(142, 92);
            this.IssuerOfPatientsId.Name = "IssuerOfPatientsId";
            this.IssuerOfPatientsId.Size = new System.Drawing.Size(100, 20);
            this.IssuerOfPatientsId.TabIndex = 3;
            this.IssuerOfPatientsId.Text = "TGH";
            // 
            // PatientsSex
            // 
            this.PatientsSex.Location = new System.Drawing.Point(142, 119);
            this.PatientsSex.Name = "PatientsSex";
            this.PatientsSex.Size = new System.Drawing.Size(100, 20);
            this.PatientsSex.TabIndex = 4;
            this.PatientsSex.Text = "M";
            // 
            // PatientsBirthdate
            // 
            this.PatientsBirthdate.Location = new System.Drawing.Point(142, 145);
            this.PatientsBirthdate.Name = "PatientsBirthdate";
            this.PatientsBirthdate.Size = new System.Drawing.Size(100, 20);
            this.PatientsBirthdate.TabIndex = 5;
            this.PatientsBirthdate.Text = "19661221";
            // 
            // StudyInstanceUid
            // 
            this.StudyInstanceUid.Location = new System.Drawing.Point(146, 77);
            this.StudyInstanceUid.Name = "StudyInstanceUid";
            this.StudyInstanceUid.Size = new System.Drawing.Size(247, 20);
            this.StudyInstanceUid.TabIndex = 5;
            this.StudyInstanceUid.Text = "1.2.3.5.6.4.3";
            // 
            // StudyDate
            // 
            this.StudyDate.Location = new System.Drawing.Point(146, 51);
            this.StudyDate.Name = "StudyDate";
            this.StudyDate.Size = new System.Drawing.Size(100, 20);
            this.StudyDate.TabIndex = 5;
            this.StudyDate.Text = "19661221";
            // 
            // AccessionNumber
            // 
            this.AccessionNumber.Location = new System.Drawing.Point(146, 20);
            this.AccessionNumber.Name = "AccessionNumber";
            this.AccessionNumber.Size = new System.Drawing.Size(100, 20);
            this.AccessionNumber.TabIndex = 5;
            this.AccessionNumber.Text = "TGH1029392";
            // 
            // Send
            // 
            this.Send.Enabled = false;
            this.Send.Location = new System.Drawing.Point(425, 47);
            this.Send.Name = "Send";
            this.Send.Size = new System.Drawing.Size(96, 23);
            this.Send.TabIndex = 6;
            this.Send.Text = "Send Images";
            this.Send.UseVisualStyleBackColor = true;
            this.Send.Click += new System.EventHandler(this.Send_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Patient Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Patient Id";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Issuer Of Patient Id";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Gender";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(24, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Birthdate";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 23);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Accession Number";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(29, 51);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Study Date";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(29, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Study Instance Uid";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.SeriesTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl1.Location = new System.Drawing.Point(0, 103);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(650, 246);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.PatientsName);
            this.tabPage1.Controls.Add(this.PatientsId);
            this.tabPage1.Controls.Add(this.IssuerOfPatientsId);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.PatientsSex);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.PatientsBirthdate);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(642, 220);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Demographics";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.StudyDate);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.StudyInstanceUid);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.AccessionNumber);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(642, 220);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Study";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ServerAE);
            this.panel1.Controls.Add(this.ServerPort);
            this.panel1.Controls.Add(this.Images);
            this.panel1.Controls.Add(this.ServerHost);
            this.panel1.Controls.Add(this.LocalAE);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.Send);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(650, 100);
            this.panel1.TabIndex = 10;
            // 
            // ServerAE
            // 
            this.ServerAE.Location = new System.Drawing.Point(76, 44);
            this.ServerAE.Name = "ServerAE";
            this.ServerAE.Size = new System.Drawing.Size(100, 20);
            this.ServerAE.TabIndex = 8;
            this.ServerAE.Text = "CLEARCANVAS";
            // 
            // ServerPort
            // 
            this.ServerPort.Location = new System.Drawing.Point(281, 47);
            this.ServerPort.Name = "ServerPort";
            this.ServerPort.Size = new System.Drawing.Size(100, 20);
            this.ServerPort.TabIndex = 8;
            this.ServerPort.Text = "104";
            // 
            // ServerHost
            // 
            this.ServerHost.Location = new System.Drawing.Point(281, 19);
            this.ServerHost.Name = "ServerHost";
            this.ServerHost.Size = new System.Drawing.Size(100, 20);
            this.ServerHost.TabIndex = 8;
            this.ServerHost.Text = "localhost";
            // 
            // LocalAE
            // 
            this.LocalAE.Location = new System.Drawing.Point(76, 19);
            this.LocalAE.Name = "LocalAE";
            this.LocalAE.Size = new System.Drawing.Size(100, 20);
            this.LocalAE.TabIndex = 8;
            this.LocalAE.Text = "TEST";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(212, 47);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 13);
            this.label12.TabIndex = 7;
            this.label12.Text = "Server Port";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(212, 22);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 13);
            this.label11.TabIndex = 7;
            this.label11.Text = "Server Host";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(20, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 13);
            this.label10.TabIndex = 7;
            this.label10.Text = "Server AE";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(20, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(50, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Local AE";
            // 
            // SeriesTab
            // 
            this.SeriesTab.Controls.Add(this.SeriesTreeView);
            this.SeriesTab.Location = new System.Drawing.Point(4, 22);
            this.SeriesTab.Name = "SeriesTab";
            this.SeriesTab.Padding = new System.Windows.Forms.Padding(3);
            this.SeriesTab.Size = new System.Drawing.Size(642, 220);
            this.SeriesTab.TabIndex = 3;
            this.SeriesTab.Text = "Series";
            this.SeriesTab.UseVisualStyleBackColor = true;
            // 
            // SeriesTreeView
            // 
            this.SeriesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SeriesTreeView.Location = new System.Drawing.Point(3, 3);
            this.SeriesTreeView.Name = "SeriesTreeView";
            this.SeriesTreeView.Size = new System.Drawing.Size(636, 214);
            this.SeriesTreeView.TabIndex = 1;
            // 
            // TestReconcileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 349);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tabControl1);
            this.Name = "TestReconcileForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "ReconcileTest";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.SeriesTab.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button Images;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox PatientsName;
        private System.Windows.Forms.TextBox PatientsId;
        private System.Windows.Forms.TextBox IssuerOfPatientsId;
        private System.Windows.Forms.TextBox PatientsSex;
        private System.Windows.Forms.TextBox PatientsBirthdate;
        private System.Windows.Forms.TextBox StudyInstanceUid;
        private System.Windows.Forms.TextBox StudyDate;
        private System.Windows.Forms.TextBox AccessionNumber;
        private System.Windows.Forms.Button Send;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox ServerAE;
        private System.Windows.Forms.TextBox ServerPort;
        private System.Windows.Forms.TextBox ServerHost;
        private System.Windows.Forms.TextBox LocalAE;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TabPage SeriesTab;
        private System.Windows.Forms.TreeView SeriesTreeView;
    }
}