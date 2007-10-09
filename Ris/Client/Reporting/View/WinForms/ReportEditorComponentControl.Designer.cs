namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    partial class ReportEditorComponentControl
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
            this._visitNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._performedDate = new ClearCanvas.Controls.WinForms.TextField();
            this._dateOfBirth = new ClearCanvas.Controls.WinForms.TextField();
            this._accessionNumber = new ClearCanvas.Controls.WinForms.TextField();
            this._mrn = new ClearCanvas.Controls.WinForms.TextField();
            this._performedLocation = new ClearCanvas.Controls.WinForms.TextField();
            this._requestedProcedure = new ClearCanvas.Controls.WinForms.TextField();
            this._patientName = new System.Windows.Forms.Label();
            this._diagnosticService = new ClearCanvas.Controls.WinForms.TextField();
            this._browserPanel = new System.Windows.Forms.Panel();
            this._browserSplitContainer = new System.Windows.Forms.SplitContainer();
            this._editorBrowser = new System.Windows.Forms.WebBrowser();
            this._previewBrowser = new System.Windows.Forms.WebBrowser();
            this.panel3 = new System.Windows.Forms.Panel();
            this._verifyButton = new System.Windows.Forms.Button();
            this._sendToVerifyButton = new System.Windows.Forms.Button();
            this._sendToTranscriptionButton = new System.Windows.Forms.Button();
            this._saveButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._residentPanel = new System.Windows.Forms.Panel();
            this._dictateFor = new ClearCanvas.Controls.WinForms.TextField();
            this._chooseRadiologistButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this._browserPanel.SuspendLayout();
            this._browserSplitContainer.Panel1.SuspendLayout();
            this._browserSplitContainer.Panel2.SuspendLayout();
            this._browserSplitContainer.SuspendLayout();
            this.panel3.SuspendLayout();
            this._residentPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._browserPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._residentPanel, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(661, 606);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._visitNumber);
            this.panel1.Controls.Add(this._performedDate);
            this.panel1.Controls.Add(this._dateOfBirth);
            this.panel1.Controls.Add(this._accessionNumber);
            this.panel1.Controls.Add(this._mrn);
            this.panel1.Controls.Add(this._performedLocation);
            this.panel1.Controls.Add(this._requestedProcedure);
            this.panel1.Controls.Add(this._patientName);
            this.panel1.Controls.Add(this._diagnosticService);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(655, 144);
            this.panel1.TabIndex = 0;
            // 
            // _visitNumber
            // 
            this._visitNumber.LabelText = "Visit Number";
            this._visitNumber.Location = new System.Drawing.Point(310, 42);
            this._visitNumber.Margin = new System.Windows.Forms.Padding(2);
            this._visitNumber.Mask = "";
            this._visitNumber.Name = "_visitNumber";
            this._visitNumber.ReadOnly = true;
            this._visitNumber.Size = new System.Drawing.Size(150, 41);
            this._visitNumber.TabIndex = 8;
            this._visitNumber.TabStop = false;
            this._visitNumber.Value = null;
            // 
            // _performedDate
            // 
            this._performedDate.LabelText = "Performed Date";
            this._performedDate.Location = new System.Drawing.Point(464, 87);
            this._performedDate.Margin = new System.Windows.Forms.Padding(2);
            this._performedDate.Mask = "";
            this._performedDate.Name = "_performedDate";
            this._performedDate.ReadOnly = true;
            this._performedDate.Size = new System.Drawing.Size(150, 41);
            this._performedDate.TabIndex = 7;
            this._performedDate.Value = null;
            // 
            // _dateOfBirth
            // 
            this._dateOfBirth.LabelText = "Date Of Birth";
            this._dateOfBirth.Location = new System.Drawing.Point(464, 42);
            this._dateOfBirth.Margin = new System.Windows.Forms.Padding(2);
            this._dateOfBirth.Mask = "";
            this._dateOfBirth.Name = "_dateOfBirth";
            this._dateOfBirth.ReadOnly = true;
            this._dateOfBirth.Size = new System.Drawing.Size(150, 41);
            this._dateOfBirth.TabIndex = 6;
            this._dateOfBirth.Value = null;
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
            this._accessionNumber.TabIndex = 2;
            this._accessionNumber.TabStop = false;
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
            this._mrn.TabIndex = 1;
            this._mrn.TabStop = false;
            this._mrn.Value = null;
            // 
            // _performedLocation
            // 
            this._performedLocation.LabelText = "Performed Location";
            this._performedLocation.Location = new System.Drawing.Point(310, 87);
            this._performedLocation.Margin = new System.Windows.Forms.Padding(2);
            this._performedLocation.Mask = "";
            this._performedLocation.Name = "_performedLocation";
            this._performedLocation.ReadOnly = true;
            this._performedLocation.Size = new System.Drawing.Size(150, 41);
            this._performedLocation.TabIndex = 5;
            this._performedLocation.TabStop = false;
            this._performedLocation.Value = null;
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
            this._requestedProcedure.TabIndex = 4;
            this._requestedProcedure.TabStop = false;
            this._requestedProcedure.Value = null;
            // 
            // _patientName
            // 
            this._patientName.AutoSize = true;
            this._patientName.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._patientName.Location = new System.Drawing.Point(3, 0);
            this._patientName.Name = "_patientName";
            this._patientName.Size = new System.Drawing.Size(190, 31);
            this._patientName.TabIndex = 0;
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
            this._diagnosticService.TabIndex = 3;
            this._diagnosticService.TabStop = false;
            this._diagnosticService.Value = null;
            // 
            // _browserPanel
            // 
            this._browserPanel.Controls.Add(this._browserSplitContainer);
            this._browserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._browserPanel.Location = new System.Drawing.Point(3, 153);
            this._browserPanel.Name = "_browserPanel";
            this._browserPanel.Size = new System.Drawing.Size(655, 350);
            this._browserPanel.TabIndex = 1;
            // 
            // _browserSplitContainer
            // 
            this._browserSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._browserSplitContainer.Location = new System.Drawing.Point(0, 0);
            this._browserSplitContainer.Name = "_browserSplitContainer";
            this._browserSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _browserSplitContainer.Panel1
            // 
            this._browserSplitContainer.Panel1.Controls.Add(this._editorBrowser);
            // 
            // _browserSplitContainer.Panel2
            // 
            this._browserSplitContainer.Panel2.Controls.Add(this._previewBrowser);
            this._browserSplitContainer.Size = new System.Drawing.Size(655, 350);
            this._browserSplitContainer.SplitterDistance = 87;
            this._browserSplitContainer.TabIndex = 0;
            // 
            // _editorBrowser
            // 
            this._editorBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._editorBrowser.Location = new System.Drawing.Point(0, 0);
            this._editorBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this._editorBrowser.Name = "_editorBrowser";
            this._editorBrowser.Size = new System.Drawing.Size(655, 87);
            this._editorBrowser.TabIndex = 0;
            // 
            // _previewBrowser
            // 
            this._previewBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._previewBrowser.Location = new System.Drawing.Point(0, 0);
            this._previewBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this._previewBrowser.Name = "_previewBrowser";
            this._previewBrowser.Size = new System.Drawing.Size(655, 259);
            this._previewBrowser.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this._verifyButton);
            this.panel3.Controls.Add(this._sendToVerifyButton);
            this.panel3.Controls.Add(this._sendToTranscriptionButton);
            this.panel3.Controls.Add(this._saveButton);
            this.panel3.Controls.Add(this._cancelButton);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 559);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(655, 44);
            this.panel3.TabIndex = 1;
            // 
            // _verifyButton
            // 
            this._verifyButton.Location = new System.Drawing.Point(3, 3);
            this._verifyButton.Name = "_verifyButton";
            this._verifyButton.Size = new System.Drawing.Size(84, 37);
            this._verifyButton.TabIndex = 0;
            this._verifyButton.Text = "Verify";
            this._verifyButton.UseVisualStyleBackColor = true;
            this._verifyButton.Click += new System.EventHandler(this._verifyButton_Click);
            // 
            // _sendToVerifyButton
            // 
            this._sendToVerifyButton.Location = new System.Drawing.Point(93, 3);
            this._sendToVerifyButton.Name = "_sendToVerifyButton";
            this._sendToVerifyButton.Size = new System.Drawing.Size(84, 37);
            this._sendToVerifyButton.TabIndex = 1;
            this._sendToVerifyButton.Text = "To be Verified";
            this._sendToVerifyButton.UseVisualStyleBackColor = true;
            this._sendToVerifyButton.Click += new System.EventHandler(this._sendToVerifyButton_Click);
            // 
            // _sendToTranscriptionButton
            // 
            this._sendToTranscriptionButton.Location = new System.Drawing.Point(183, 3);
            this._sendToTranscriptionButton.Name = "_sendToTranscriptionButton";
            this._sendToTranscriptionButton.Size = new System.Drawing.Size(84, 37);
            this._sendToTranscriptionButton.TabIndex = 2;
            this._sendToTranscriptionButton.Text = "Send to Transcription";
            this._sendToTranscriptionButton.UseVisualStyleBackColor = true;
            this._sendToTranscriptionButton.Click += new System.EventHandler(this._sendToTranscriptionButton_Click);
            // 
            // _saveButton
            // 
            this._saveButton.Location = new System.Drawing.Point(273, 3);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(84, 37);
            this._saveButton.TabIndex = 3;
            this._saveButton.Text = "Save";
            this._saveButton.UseVisualStyleBackColor = true;
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._cancelButton.Location = new System.Drawing.Point(568, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(84, 37);
            this._cancelButton.TabIndex = 4;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _residentPanel
            // 
            this._residentPanel.Controls.Add(this._dictateFor);
            this._residentPanel.Controls.Add(this._chooseRadiologistButton);
            this._residentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._residentPanel.Location = new System.Drawing.Point(3, 509);
            this._residentPanel.Name = "_residentPanel";
            this._residentPanel.Size = new System.Drawing.Size(655, 44);
            this._residentPanel.TabIndex = 2;
            // 
            // _dictateFor
            // 
            this._dictateFor.LabelText = "Dictate for:";
            this._dictateFor.Location = new System.Drawing.Point(3, 3);
            this._dictateFor.Margin = new System.Windows.Forms.Padding(2);
            this._dictateFor.Mask = "";
            this._dictateFor.Name = "_dictateFor";
            this._dictateFor.ReadOnly = true;
            this._dictateFor.Size = new System.Drawing.Size(150, 41);
            this._dictateFor.TabIndex = 3;
            this._dictateFor.Value = null;
            // 
            // _chooseRadiologistButton
            // 
            this._chooseRadiologistButton.Location = new System.Drawing.Point(158, 18);
            this._chooseRadiologistButton.Name = "_chooseRadiologistButton";
            this._chooseRadiologistButton.Size = new System.Drawing.Size(75, 23);
            this._chooseRadiologistButton.TabIndex = 2;
            this._chooseRadiologistButton.Text = "Choose...";
            this._chooseRadiologistButton.UseVisualStyleBackColor = true;
            this._chooseRadiologistButton.Click += new System.EventHandler(this._chooseRadiologistButton_Click);
            // 
            // ReportEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ReportEditorComponentControl";
            this.Size = new System.Drawing.Size(661, 606);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this._browserPanel.ResumeLayout(false);
            this._browserSplitContainer.Panel1.ResumeLayout(false);
            this._browserSplitContainer.Panel2.ResumeLayout(false);
            this._browserSplitContainer.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this._residentPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private ClearCanvas.Controls.WinForms.TextField _accessionNumber;
        private ClearCanvas.Controls.WinForms.TextField _mrn;
        private ClearCanvas.Controls.WinForms.TextField _performedLocation;
        private ClearCanvas.Controls.WinForms.TextField _requestedProcedure;
        private System.Windows.Forms.Label _patientName;
        private ClearCanvas.Controls.WinForms.TextField _diagnosticService;
        private System.Windows.Forms.Panel _browserPanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button _verifyButton;
        private System.Windows.Forms.Button _sendToVerifyButton;
        private System.Windows.Forms.Button _sendToTranscriptionButton;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.SplitContainer _browserSplitContainer;
        private System.Windows.Forms.WebBrowser _editorBrowser;
        private System.Windows.Forms.WebBrowser _previewBrowser;
        private ClearCanvas.Controls.WinForms.TextField _dateOfBirth;
        private ClearCanvas.Controls.WinForms.TextField _performedDate;
        private ClearCanvas.Controls.WinForms.TextField _visitNumber;
        private System.Windows.Forms.Panel _residentPanel;
        private ClearCanvas.Controls.WinForms.TextField _dictateFor;
        private System.Windows.Forms.Button _chooseRadiologistButton;

    }
}
