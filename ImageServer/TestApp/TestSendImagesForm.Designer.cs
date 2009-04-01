namespace ClearCanvas.ImageServer.TestApp
{
    partial class TestSendImagesForm
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
            this.LoadSamples = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.NewStudy = new System.Windows.Forms.Button();
            this.RandomPatient = new System.Windows.Forms.Button();
            this.GenerateImages = new System.Windows.Forms.Button();
            this.ServerAE = new System.Windows.Forms.TextBox();
            this.ServerPort = new System.Windows.Forms.TextBox();
            this.ServerHost = new System.Windows.Forms.TextBox();
            this.LocalAE = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.StudyDate = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.StudyInstanceUid = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.AccessionNumber = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.PatientsName = new System.Windows.Forms.TextBox();
            this.PatientsId = new System.Windows.Forms.TextBox();
            this.IssuerOfPatientsId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.PatientsSex = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PatientsBirthdate = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // LoadSamples
            // 
            this.LoadSamples.Location = new System.Drawing.Point(489, 37);
            this.LoadSamples.Name = "LoadSamples";
            this.LoadSamples.Size = new System.Drawing.Size(105, 38);
            this.LoadSamples.TabIndex = 0;
            this.LoadSamples.Text = "Load Samples ...";
            this.LoadSamples.UseVisualStyleBackColor = true;
            this.LoadSamples.Click += new System.EventHandler(this.LoadSamples_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ServerAE);
            this.panel1.Controls.Add(this.ServerPort);
            this.panel1.Controls.Add(this.ServerHost);
            this.panel1.Controls.Add(this.LocalAE);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(703, 78);
            this.panel1.TabIndex = 10;
            // 
            // NewStudy
            // 
            this.NewStudy.Location = new System.Drawing.Point(489, 145);
            this.NewStudy.Name = "NewStudy";
            this.NewStudy.Size = new System.Drawing.Size(100, 38);
            this.NewStudy.TabIndex = 10;
            this.NewStudy.Text = "New Study";
            this.NewStudy.UseVisualStyleBackColor = true;
            this.NewStudy.Click += new System.EventHandler(this.NewStudy_Click);
            // 
            // RandomPatient
            // 
            this.RandomPatient.Location = new System.Drawing.Point(489, 92);
            this.RandomPatient.Name = "RandomPatient";
            this.RandomPatient.Size = new System.Drawing.Size(100, 38);
            this.RandomPatient.TabIndex = 10;
            this.RandomPatient.Text = "New Patient";
            this.RandomPatient.UseVisualStyleBackColor = true;
            this.RandomPatient.Click += new System.EventHandler(this.RandomPatient_Click);
            // 
            // GenerateImages
            // 
            this.GenerateImages.Location = new System.Drawing.Point(489, 202);
            this.GenerateImages.Name = "GenerateImages";
            this.GenerateImages.Size = new System.Drawing.Size(103, 38);
            this.GenerateImages.TabIndex = 9;
            this.GenerateImages.Text = "Create Images && Send";
            this.GenerateImages.UseVisualStyleBackColor = true;
            this.GenerateImages.Click += new System.EventHandler(this.SendRandom_Click);
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
            // panel2
            // 
            this.panel2.Controls.Add(this.NewStudy);
            this.panel2.Controls.Add(this.RandomPatient);
            this.panel2.Controls.Add(this.StudyDate);
            this.panel2.Controls.Add(this.GenerateImages);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.StudyInstanceUid);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.LoadSamples);
            this.panel2.Controls.Add(this.AccessionNumber);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.PatientsName);
            this.panel2.Controls.Add(this.PatientsId);
            this.panel2.Controls.Add(this.IssuerOfPatientsId);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.PatientsSex);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.PatientsBirthdate);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 78);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(703, 316);
            this.panel2.TabIndex = 31;
            // 
            // StudyDate
            // 
            this.StudyDate.Location = new System.Drawing.Point(138, 215);
            this.StudyDate.Name = "StudyDate";
            this.StudyDate.Size = new System.Drawing.Size(125, 20);
            this.StudyDate.TabIndex = 43;
            this.StudyDate.Text = "19661221";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 190);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 13);
            this.label6.TabIndex = 45;
            this.label6.Text = "Accession Number";
            // 
            // StudyInstanceUid
            // 
            this.StudyInstanceUid.Location = new System.Drawing.Point(138, 245);
            this.StudyInstanceUid.Name = "StudyInstanceUid";
            this.StudyInstanceUid.Size = new System.Drawing.Size(247, 20);
            this.StudyInstanceUid.TabIndex = 41;
            this.StudyInstanceUid.Text = "1.2.3.5.6.4.3";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 248);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(97, 13);
            this.label8.TabIndex = 46;
            this.label8.Text = "Study Instance Uid";
            // 
            // AccessionNumber
            // 
            this.AccessionNumber.Location = new System.Drawing.Point(138, 187);
            this.AccessionNumber.Name = "AccessionNumber";
            this.AccessionNumber.Size = new System.Drawing.Size(125, 20);
            this.AccessionNumber.TabIndex = 42;
            this.AccessionNumber.Text = "TGH1029392";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 220);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 13);
            this.label7.TabIndex = 44;
            this.label7.Text = "Study Date";
            // 
            // PatientsName
            // 
            this.PatientsName.Location = new System.Drawing.Point(138, 37);
            this.PatientsName.Name = "PatientsName";
            this.PatientsName.Size = new System.Drawing.Size(147, 20);
            this.PatientsName.TabIndex = 31;
            this.PatientsName.Text = "John^Smith";
            // 
            // PatientsId
            // 
            this.PatientsId.Location = new System.Drawing.Point(138, 67);
            this.PatientsId.Name = "PatientsId";
            this.PatientsId.Size = new System.Drawing.Size(147, 20);
            this.PatientsId.TabIndex = 32;
            this.PatientsId.Text = "PATID-1001";
            // 
            // IssuerOfPatientsId
            // 
            this.IssuerOfPatientsId.Location = new System.Drawing.Point(138, 95);
            this.IssuerOfPatientsId.Name = "IssuerOfPatientsId";
            this.IssuerOfPatientsId.Size = new System.Drawing.Size(147, 20);
            this.IssuerOfPatientsId.TabIndex = 33;
            this.IssuerOfPatientsId.Text = "TGH";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 155);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 36;
            this.label5.Text = "Birthdate";
            // 
            // PatientsSex
            // 
            this.PatientsSex.Location = new System.Drawing.Point(138, 122);
            this.PatientsSex.Name = "PatientsSex";
            this.PatientsSex.Size = new System.Drawing.Size(147, 20);
            this.PatientsSex.TabIndex = 34;
            this.PatientsSex.Text = "M";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 129);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 37;
            this.label4.Text = "Gender";
            // 
            // PatientsBirthdate
            // 
            this.PatientsBirthdate.Location = new System.Drawing.Point(138, 148);
            this.PatientsBirthdate.Name = "PatientsBirthdate";
            this.PatientsBirthdate.Size = new System.Drawing.Size(147, 20);
            this.PatientsBirthdate.TabIndex = 35;
            this.PatientsBirthdate.Text = "19661221";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "Issuer Of Patient Id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 39;
            this.label2.Text = "Patient Id";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 40;
            this.label1.Text = "Patient Name";
            // 
            // TestReconcileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 394);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "TestReconcileForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Random Image Sender";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button LoadSamples;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox ServerAE;
        private System.Windows.Forms.TextBox ServerPort;
        private System.Windows.Forms.TextBox ServerHost;
        private System.Windows.Forms.TextBox LocalAE;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button GenerateImages;
        private System.Windows.Forms.Button NewStudy;
        private System.Windows.Forms.Button RandomPatient;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox StudyDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox StudyInstanceUid;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox AccessionNumber;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox PatientsName;
        private System.Windows.Forms.TextBox PatientsId;
        private System.Windows.Forms.TextBox IssuerOfPatientsId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox PatientsSex;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PatientsBirthdate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}