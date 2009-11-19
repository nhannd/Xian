namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms {
	partial class ExportComponentPanel {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this._studyDescription = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._patientId = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._accessionNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._patientsName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._dateOfBirth = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._studyDate = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this._studyId = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._browse = new System.Windows.Forms.Button();
			this._outputPath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _studyDescription
			// 
			this._studyDescription.LabelText = "Study Description";
			this._studyDescription.Location = new System.Drawing.Point(15, 156);
			this._studyDescription.Margin = new System.Windows.Forms.Padding(2);
			this._studyDescription.Mask = "";
			this._studyDescription.Name = "_studyDescription";
			this._studyDescription.PasswordChar = '\0';
			this._studyDescription.Size = new System.Drawing.Size(229, 41);
			this._studyDescription.TabIndex = 3;
			this._studyDescription.ToolTip = null;
			this._studyDescription.Value = null;
			// 
			// _patientId
			// 
			this._patientId.LabelText = "Patient ID";
			this._patientId.Location = new System.Drawing.Point(15, 18);
			this._patientId.Margin = new System.Windows.Forms.Padding(2);
			this._patientId.Mask = "";
			this._patientId.Name = "_patientId";
			this._patientId.PasswordChar = '\0';
			this._patientId.Size = new System.Drawing.Size(229, 41);
			this._patientId.TabIndex = 0;
			this._patientId.ToolTip = null;
			this._patientId.Value = null;
			// 
			// _accessionNumber
			// 
			this._accessionNumber.LabelText = "Accession Number";
			this._accessionNumber.Location = new System.Drawing.Point(15, 248);
			this._accessionNumber.Margin = new System.Windows.Forms.Padding(2);
			this._accessionNumber.Mask = "";
			this._accessionNumber.Name = "_accessionNumber";
			this._accessionNumber.PasswordChar = '\0';
			this._accessionNumber.Size = new System.Drawing.Size(150, 41);
			this._accessionNumber.TabIndex = 5;
			this._accessionNumber.ToolTip = null;
			this._accessionNumber.Value = null;
			// 
			// _patientsName
			// 
			this._patientsName.LabelText = "Patient\'s Name";
			this._patientsName.Location = new System.Drawing.Point(15, 64);
			this._patientsName.Margin = new System.Windows.Forms.Padding(2);
			this._patientsName.Mask = "";
			this._patientsName.Name = "_patientsName";
			this._patientsName.PasswordChar = '\0';
			this._patientsName.Size = new System.Drawing.Size(229, 41);
			this._patientsName.TabIndex = 1;
			this._patientsName.ToolTip = null;
			this._patientsName.Value = null;
			// 
			// _dateOfBirth
			// 
			this._dateOfBirth.LabelText = "Date of Birth";
			this._dateOfBirth.Location = new System.Drawing.Point(15, 110);
			this._dateOfBirth.Margin = new System.Windows.Forms.Padding(2);
			this._dateOfBirth.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._dateOfBirth.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._dateOfBirth.Name = "_dateOfBirth";
			this._dateOfBirth.Nullable = true;
			this._dateOfBirth.Size = new System.Drawing.Size(150, 41);
			this._dateOfBirth.TabIndex = 2;
			this._dateOfBirth.Value = new System.DateTime(2008, 4, 21, 9, 11, 13, 140);
			// 
			// _studyDate
			// 
			this._studyDate.LabelText = "Study Date";
			this._studyDate.Location = new System.Drawing.Point(15, 294);
			this._studyDate.Margin = new System.Windows.Forms.Padding(2);
			this._studyDate.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
			this._studyDate.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
			this._studyDate.Name = "_studyDate";
			this._studyDate.Nullable = true;
			this._studyDate.Size = new System.Drawing.Size(150, 41);
			this._studyDate.TabIndex = 6;
			this._studyDate.Value = new System.DateTime(2008, 4, 21, 9, 14, 8, 984);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(171, 410);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 10;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(90, 410);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 9;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _studyId
			// 
			this._studyId.LabelText = "Study ID";
			this._studyId.Location = new System.Drawing.Point(15, 202);
			this._studyId.Margin = new System.Windows.Forms.Padding(2);
			this._studyId.Mask = "";
			this._studyId.Name = "_studyId";
			this._studyId.PasswordChar = '\0';
			this._studyId.Size = new System.Drawing.Size(229, 41);
			this._studyId.TabIndex = 4;
			this._studyId.ToolTip = null;
			this._studyId.Value = null;
			// 
			// _browse
			// 
			this._browse.Location = new System.Drawing.Point(186, 365);
			this._browse.Name = "_browse";
			this._browse.Size = new System.Drawing.Size(56, 20);
			this._browse.TabIndex = 8;
			this._browse.Text = "Browse";
			this._browse.UseVisualStyleBackColor = true;
			this._browse.Click += new System.EventHandler(this._browse_Click);
			// 
			// _outputPath
			// 
			this._outputPath.Location = new System.Drawing.Point(17, 365);
			this._outputPath.Name = "_outputPath";
			this._outputPath.Size = new System.Drawing.Size(163, 20);
			this._outputPath.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(17, 349);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 13);
			this.label1.TabIndex = 12;
			this.label1.Text = "Output Path";
			// 
			// ExportComponentPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._outputPath);
			this.Controls.Add(this._browse);
			this.Controls.Add(this._studyId);
			this.Controls.Add(this._studyDescription);
			this.Controls.Add(this._patientId);
			this.Controls.Add(this._accessionNumber);
			this.Controls.Add(this._patientsName);
			this.Controls.Add(this._dateOfBirth);
			this.Controls.Add(this._studyDate);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this.label1);
			this.Name = "ExportComponentPanel";
			this.Size = new System.Drawing.Size(264, 450);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _studyDescription;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientId;
		private ClearCanvas.Desktop.View.WinForms.TextField _accessionNumber;
		private ClearCanvas.Desktop.View.WinForms.TextField _patientsName;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _dateOfBirth;
		private ClearCanvas.Desktop.View.WinForms.DateTimeField _studyDate;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
		private ClearCanvas.Desktop.View.WinForms.TextField _studyId;
		private System.Windows.Forms.Button _browse;
		private System.Windows.Forms.TextBox _outputPath;
		private System.Windows.Forms.Label label1;
	}
}
