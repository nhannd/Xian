namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    partial class InterpretationComponentControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this._accessionNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._mrn = new ClearCanvas.Controls.WinForms.TextField();
            this._priority = new ClearCanvas.Controls.WinForms.TextField();
            this._requestedProcedure = new ClearCanvas.Controls.WinForms.TextField();
            this._patientName = new System.Windows.Forms.Label();
            this._diagnosticService = new ClearCanvas.Controls.WinForms.TextField();
            this.panel2 = new System.Windows.Forms.Panel();
            this._report = new ClearCanvas.Controls.WinForms.TextAreaField();
            this.panel3 = new System.Windows.Forms.Panel();
            this._verifyButton = new System.Windows.Forms.Button();
            this._sendToVerifyButton = new System.Windows.Forms.Button();
            this._sendToTranscriptionButton = new System.Windows.Forms.Button();
            this._saveButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(597, 521);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._accessionNumber);
            this.panel1.Controls.Add(this._mrn);
            this.panel1.Controls.Add(this._priority);
            this.panel1.Controls.Add(this._requestedProcedure);
            this.panel1.Controls.Add(this._patientName);
            this.panel1.Controls.Add(this._diagnosticService);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(591, 144);
            this.panel1.TabIndex = 0;
            // 
            // _accessionNumber
            // 
            this._accessionNumber.LabelText = "Accession Number";
            this._accessionNumber.Location = new System.Drawing.Point(156, 42);
            this._accessionNumber.Margin = new System.Windows.Forms.Padding(2);
            this._accessionNumber.Mask = "";
            this._accessionNumber.Name = "_accessionNumber";
            this._accessionNumber.ReadOnly = true;
            this._accessionNumber.Size = new System.Drawing.Size(150, 41);
            this._accessionNumber.TabIndex = 12;
            this._accessionNumber.Value = null;
            // 
            // _mrn
            // 
            this._mrn.LabelText = "Mrn";
            this._mrn.Location = new System.Drawing.Point(2, 42);
            this._mrn.Margin = new System.Windows.Forms.Padding(2);
            this._mrn.Mask = "";
            this._mrn.Name = "_mrn";
            this._mrn.ReadOnly = true;
            this._mrn.Size = new System.Drawing.Size(150, 41);
            this._mrn.TabIndex = 11;
            this._mrn.Value = null;
            // 
            // _priority
            // 
            this._priority.LabelText = "Priority";
            this._priority.Location = new System.Drawing.Point(310, 87);
            this._priority.Margin = new System.Windows.Forms.Padding(2);
            this._priority.Mask = "";
            this._priority.Name = "_priority";
            this._priority.ReadOnly = true;
            this._priority.Size = new System.Drawing.Size(150, 41);
            this._priority.TabIndex = 10;
            this._priority.Value = null;
            // 
            // _requestedProcedure
            // 
            this._requestedProcedure.LabelText = "Requested Procedure";
            this._requestedProcedure.Location = new System.Drawing.Point(156, 87);
            this._requestedProcedure.Margin = new System.Windows.Forms.Padding(2);
            this._requestedProcedure.Mask = "";
            this._requestedProcedure.Name = "_requestedProcedure";
            this._requestedProcedure.ReadOnly = true;
            this._requestedProcedure.Size = new System.Drawing.Size(150, 41);
            this._requestedProcedure.TabIndex = 9;
            this._requestedProcedure.Value = null;
            // 
            // _patientName
            // 
            this._patientName.AutoSize = true;
            this._patientName.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._patientName.Location = new System.Drawing.Point(3, 0);
            this._patientName.Name = "_patientName";
            this._patientName.Size = new System.Drawing.Size(190, 31);
            this._patientName.TabIndex = 8;
            this._patientName.Text = "Patient Name";
            // 
            // _diagnosticService
            // 
            this._diagnosticService.LabelText = "Diagnostic Service";
            this._diagnosticService.Location = new System.Drawing.Point(2, 87);
            this._diagnosticService.Margin = new System.Windows.Forms.Padding(2);
            this._diagnosticService.Mask = "";
            this._diagnosticService.Name = "_diagnosticService";
            this._diagnosticService.ReadOnly = true;
            this._diagnosticService.Size = new System.Drawing.Size(150, 41);
            this._diagnosticService.TabIndex = 7;
            this._diagnosticService.Value = null;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this._report);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 153);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(591, 315);
            this.panel2.TabIndex = 1;
            // 
            // _report
            // 
            this._report.Dock = System.Windows.Forms.DockStyle.Fill;
            this._report.LabelText = "Report";
            this._report.Location = new System.Drawing.Point(0, 0);
            this._report.Margin = new System.Windows.Forms.Padding(2);
            this._report.Name = "_report";
            this._report.Size = new System.Drawing.Size(591, 315);
            this._report.TabIndex = 1;
            this._report.Value = null;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this._verifyButton);
            this.panel3.Controls.Add(this._sendToVerifyButton);
            this.panel3.Controls.Add(this._sendToTranscriptionButton);
            this.panel3.Controls.Add(this._saveButton);
            this.panel3.Controls.Add(this._cancelButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 474);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(591, 44);
            this.panel3.TabIndex = 2;
            // 
            // _verifyButton
            // 
            this._verifyButton.Location = new System.Drawing.Point(3, 3);
            this._verifyButton.Name = "_verifyButton";
            this._verifyButton.Size = new System.Drawing.Size(84, 37);
            this._verifyButton.TabIndex = 20;
            this._verifyButton.Text = "Verify";
            this._verifyButton.UseVisualStyleBackColor = true;
            this._verifyButton.Click += new System.EventHandler(this._verifyButton_Click);
            // 
            // _sendToVerifyButton
            // 
            this._sendToVerifyButton.Location = new System.Drawing.Point(93, 3);
            this._sendToVerifyButton.Name = "_sendToVerifyButton";
            this._sendToVerifyButton.Size = new System.Drawing.Size(84, 37);
            this._sendToVerifyButton.TabIndex = 21;
            this._sendToVerifyButton.Text = "Send to Verify";
            this._sendToVerifyButton.UseVisualStyleBackColor = true;
            this._sendToVerifyButton.Click += new System.EventHandler(this._sendToVerifyButton_Click);
            // 
            // _sendToTranscriptionButton
            // 
            this._sendToTranscriptionButton.Location = new System.Drawing.Point(183, 3);
            this._sendToTranscriptionButton.Name = "_sendToTranscriptionButton";
            this._sendToTranscriptionButton.Size = new System.Drawing.Size(84, 37);
            this._sendToTranscriptionButton.TabIndex = 22;
            this._sendToTranscriptionButton.Text = "Send to Transcription";
            this._sendToTranscriptionButton.UseVisualStyleBackColor = true;
            this._sendToTranscriptionButton.Click += new System.EventHandler(this._sendToTranscriptionButton_Click);
            // 
            // _saveButton
            // 
            this._saveButton.Location = new System.Drawing.Point(273, 3);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(84, 37);
            this._saveButton.TabIndex = 23;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.Location = new System.Drawing.Point(504, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(84, 37);
            this._cancelButton.TabIndex = 24;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // InterpretationComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "InterpretationComponentControl";
            this.Size = new System.Drawing.Size(597, 521);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private ClearCanvas.Controls.WinForms.TextField _accessionNumber;
        private ClearCanvas.Controls.WinForms.TextField _mrn;
        private ClearCanvas.Controls.WinForms.TextField _priority;
        private ClearCanvas.Controls.WinForms.TextField _requestedProcedure;
        private System.Windows.Forms.Label _patientName;
        private ClearCanvas.Controls.WinForms.TextField _diagnosticService;
        private System.Windows.Forms.Panel panel2;
        private ClearCanvas.Controls.WinForms.TextAreaField _report;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button _verifyButton;
        private System.Windows.Forms.Button _sendToVerifyButton;
        private System.Windows.Forms.Button _sendToTranscriptionButton;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.Button _cancelButton;

    }
}
