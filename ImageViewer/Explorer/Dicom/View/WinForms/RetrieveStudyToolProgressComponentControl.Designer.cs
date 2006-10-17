namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class RetrieveStudyToolProgressComponentControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._progressGroupBox = new System.Windows.Forms.GroupBox();
            this._retrieveProgressBar = new System.Windows.Forms.ProgressBar();
            this._studyDescriptionGroupBox = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this._description = new System.Windows.Forms.TextBox();
            this._studyDate = new System.Windows.Forms.TextBox();
            this._patient = new System.Windows.Forms.TextBox();
            this._retrieveSourceGroupBox = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._port = new System.Windows.Forms.TextBox();
            this._host = new System.Windows.Forms.TextBox();
            this._aeTitle = new System.Windows.Forms.TextBox();
            this._progressToolTip = new System.Windows.Forms.ToolTip(this.components);
            this._totalObjectsReceivedLabel = new System.Windows.Forms.Label();
            this._progressGroupBox.SuspendLayout();
            this._studyDescriptionGroupBox.SuspendLayout();
            this._retrieveSourceGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _progressGroupBox
            // 
            this._progressGroupBox.Controls.Add(this._totalObjectsReceivedLabel);
            this._progressGroupBox.Controls.Add(this._retrieveProgressBar);
            this._progressGroupBox.Location = new System.Drawing.Point(16, 19);
            this._progressGroupBox.Name = "_progressGroupBox";
            this._progressGroupBox.Size = new System.Drawing.Size(425, 78);
            this._progressGroupBox.TabIndex = 2;
            this._progressGroupBox.TabStop = false;
            this._progressGroupBox.Text = "Progress";
            // 
            // _retrieveProgressBar
            // 
            this._retrieveProgressBar.Location = new System.Drawing.Point(15, 39);
            this._retrieveProgressBar.Name = "_retrieveProgressBar";
            this._retrieveProgressBar.Size = new System.Drawing.Size(346, 23);
            this._retrieveProgressBar.TabIndex = 1;
            // 
            // _studyDescriptionGroupBox
            // 
            this._studyDescriptionGroupBox.Controls.Add(this.label7);
            this._studyDescriptionGroupBox.Controls.Add(this.label6);
            this._studyDescriptionGroupBox.Controls.Add(this.label5);
            this._studyDescriptionGroupBox.Controls.Add(this._description);
            this._studyDescriptionGroupBox.Controls.Add(this._studyDate);
            this._studyDescriptionGroupBox.Controls.Add(this._patient);
            this._studyDescriptionGroupBox.Location = new System.Drawing.Point(16, 205);
            this._studyDescriptionGroupBox.Name = "_studyDescriptionGroupBox";
            this._studyDescriptionGroupBox.Size = new System.Drawing.Size(425, 135);
            this._studyDescriptionGroupBox.TabIndex = 3;
            this._studyDescriptionGroupBox.TabStop = false;
            this._studyDescriptionGroupBox.Text = "Current study being retrieved";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 79);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 17);
            this.label7.TabIndex = 5;
            this.label7.Text = "Description";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(298, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 17);
            this.label6.TabIndex = 4;
            this.label6.Text = "Study Date";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 17);
            this.label5.TabIndex = 3;
            this.label5.Text = "Patient";
            // 
            // _description
            // 
            this._description.Location = new System.Drawing.Point(15, 100);
            this._description.Name = "_description";
            this._description.ReadOnly = true;
            this._description.Size = new System.Drawing.Size(395, 22);
            this._description.TabIndex = 2;
            // 
            // _studyDate
            // 
            this._studyDate.Location = new System.Drawing.Point(298, 50);
            this._studyDate.Name = "_studyDate";
            this._studyDate.ReadOnly = true;
            this._studyDate.Size = new System.Drawing.Size(112, 22);
            this._studyDate.TabIndex = 1;
            // 
            // _patient
            // 
            this._patient.Location = new System.Drawing.Point(15, 50);
            this._patient.Name = "_patient";
            this._patient.ReadOnly = true;
            this._patient.Size = new System.Drawing.Size(277, 22);
            this._patient.TabIndex = 0;
            // 
            // _retrieveSourceGroupBox
            // 
            this._retrieveSourceGroupBox.Controls.Add(this.label3);
            this._retrieveSourceGroupBox.Controls.Add(this.label2);
            this._retrieveSourceGroupBox.Controls.Add(this.label1);
            this._retrieveSourceGroupBox.Controls.Add(this._port);
            this._retrieveSourceGroupBox.Controls.Add(this._host);
            this._retrieveSourceGroupBox.Controls.Add(this._aeTitle);
            this._retrieveSourceGroupBox.Location = new System.Drawing.Point(16, 107);
            this._retrieveSourceGroupBox.Name = "_retrieveSourceGroupBox";
            this._retrieveSourceGroupBox.Size = new System.Drawing.Size(425, 87);
            this._retrieveSourceGroupBox.TabIndex = 0;
            this._retrieveSourceGroupBox.TabStop = false;
            this._retrieveSourceGroupBox.Text = "Current retrieval source";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(344, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(211, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Host";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "AE Title";
            // 
            // _port
            // 
            this._port.Location = new System.Drawing.Point(343, 48);
            this._port.Name = "_port";
            this._port.ReadOnly = true;
            this._port.Size = new System.Drawing.Size(67, 22);
            this._port.TabIndex = 5;
            // 
            // _host
            // 
            this._host.Location = new System.Drawing.Point(211, 48);
            this._host.Name = "_host";
            this._host.ReadOnly = true;
            this._host.Size = new System.Drawing.Size(126, 22);
            this._host.TabIndex = 2;
            // 
            // _aeTitle
            // 
            this._aeTitle.Location = new System.Drawing.Point(15, 48);
            this._aeTitle.Name = "_aeTitle";
            this._aeTitle.ReadOnly = true;
            this._aeTitle.Size = new System.Drawing.Size(190, 22);
            this._aeTitle.TabIndex = 1;
            // 
            // _totalObjectsReceivedLabel
            // 
            this._totalObjectsReceivedLabel.AutoSize = true;
            this._totalObjectsReceivedLabel.Location = new System.Drawing.Point(368, 39);
            this._totalObjectsReceivedLabel.Name = "_totalObjectsReceivedLabel";
            this._totalObjectsReceivedLabel.Size = new System.Drawing.Size(0, 17);
            this._totalObjectsReceivedLabel.TabIndex = 2;
            // 
            // RetrieveStudyToolProgressComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._studyDescriptionGroupBox);
            this.Controls.Add(this._progressGroupBox);
            this.Controls.Add(this._retrieveSourceGroupBox);
            this.Name = "RetrieveStudyToolProgressComponentControl";
            this.Size = new System.Drawing.Size(458, 364);
            this._progressGroupBox.ResumeLayout(false);
            this._progressGroupBox.PerformLayout();
            this._studyDescriptionGroupBox.ResumeLayout(false);
            this._studyDescriptionGroupBox.PerformLayout();
            this._retrieveSourceGroupBox.ResumeLayout(false);
            this._retrieveSourceGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _progressGroupBox;
        private System.Windows.Forms.ProgressBar _retrieveProgressBar;
        private System.Windows.Forms.GroupBox _studyDescriptionGroupBox;
        private System.Windows.Forms.GroupBox _retrieveSourceGroupBox;
        private System.Windows.Forms.ToolTip _progressToolTip;
        private System.Windows.Forms.TextBox _port;
        private System.Windows.Forms.TextBox _host;
        private System.Windows.Forms.TextBox _aeTitle;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _description;
        private System.Windows.Forms.TextBox _studyDate;
        private System.Windows.Forms.TextBox _patient;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label _totalObjectsReceivedLabel;
    }
}
