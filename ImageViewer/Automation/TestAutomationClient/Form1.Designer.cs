namespace TestAutomationClient
{
	partial class Form1
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
			this._openStudy = new System.Windows.Forms.Button();
			this._activateViewer = new System.Windows.Forms.Button();
			this._closeViewer = new System.Windows.Forms.Button();
			this._startViewer = new System.Windows.Forms.Button();
			this._studyGrid = new System.Windows.Forms.DataGridView();
			this._activateIfOpen = new System.Windows.Forms.CheckBox();
			this._openSessions = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.PatientsName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.StudyDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.StudyInstanceUid = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this._studyGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// _openStudy
			// 
			this._openStudy.Location = new System.Drawing.Point(561, 198);
			this._openStudy.Name = "_openStudy";
			this._openStudy.Size = new System.Drawing.Size(194, 23);
			this._openStudy.TabIndex = 0;
			this._openStudy.Text = "Open Study";
			this._openStudy.UseVisualStyleBackColor = true;
			this._openStudy.Click += new System.EventHandler(this.OnOpenStudy);
			// 
			// _activateViewer
			// 
			this._activateViewer.Location = new System.Drawing.Point(561, 227);
			this._activateViewer.Name = "_activateViewer";
			this._activateViewer.Size = new System.Drawing.Size(194, 23);
			this._activateViewer.TabIndex = 1;
			this._activateViewer.Text = "Activate Viewer";
			this._activateViewer.UseVisualStyleBackColor = true;
			this._activateViewer.Click += new System.EventHandler(this.OnActivateViewer);
			// 
			// _closeViewer
			// 
			this._closeViewer.Location = new System.Drawing.Point(561, 256);
			this._closeViewer.Name = "_closeViewer";
			this._closeViewer.Size = new System.Drawing.Size(194, 23);
			this._closeViewer.TabIndex = 2;
			this._closeViewer.Text = "Close Viewer";
			this._closeViewer.UseVisualStyleBackColor = true;
			this._closeViewer.Click += new System.EventHandler(this.OnCloseViewer);
			// 
			// _startViewer
			// 
			this._startViewer.Location = new System.Drawing.Point(561, 169);
			this._startViewer.Name = "_startViewer";
			this._startViewer.Size = new System.Drawing.Size(194, 23);
			this._startViewer.TabIndex = 3;
			this._startViewer.Text = "Start Viewer";
			this._startViewer.UseVisualStyleBackColor = true;
			this._startViewer.Click += new System.EventHandler(this.StartViewer);
			// 
			// _studyGrid
			// 
			this._studyGrid.AllowUserToAddRows = false;
			this._studyGrid.AllowUserToDeleteRows = false;
			this._studyGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this._studyGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PatientsName,
            this.Id,
            this.StudyDescription,
            this.StudyInstanceUid});
			this._studyGrid.Location = new System.Drawing.Point(12, 12);
			this._studyGrid.Name = "_studyGrid";
			this._studyGrid.Size = new System.Drawing.Size(543, 290);
			this._studyGrid.TabIndex = 5;
			// 
			// _activateIfOpen
			// 
			this._activateIfOpen.AutoSize = true;
			this._activateIfOpen.Checked = true;
			this._activateIfOpen.CheckState = System.Windows.Forms.CheckState.Checked;
			this._activateIfOpen.Location = new System.Drawing.Point(562, 286);
			this._activateIfOpen.Name = "_activateIfOpen";
			this._activateIfOpen.Size = new System.Drawing.Size(137, 17);
			this._activateIfOpen.TabIndex = 6;
			this._activateIfOpen.Text = "Activate if already open";
			this._activateIfOpen.UseVisualStyleBackColor = true;
			// 
			// _openSessions
			// 
			this._openSessions.FormattingEnabled = true;
			this._openSessions.Location = new System.Drawing.Point(562, 26);
			this._openSessions.Name = "_openSessions";
			this._openSessions.Size = new System.Drawing.Size(193, 134);
			this._openSessions.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(562, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Open Sessions";
			// 
			// PatientsName
			// 
			this.PatientsName.HeaderText = "Patient Name";
			this.PatientsName.Name = "PatientsName";
			this.PatientsName.ReadOnly = true;
			// 
			// Id
			// 
			this.Id.HeaderText = "Id";
			this.Id.Name = "Id";
			this.Id.ReadOnly = true;
			// 
			// StudyDescription
			// 
			this.StudyDescription.HeaderText = "Study Description";
			this.StudyDescription.Name = "StudyDescription";
			this.StudyDescription.ReadOnly = true;
			// 
			// StudyInstanceUid
			// 
			this.StudyInstanceUid.HeaderText = "Study Uid";
			this.StudyInstanceUid.Name = "StudyInstanceUid";
			this.StudyInstanceUid.ReadOnly = true;
			this.StudyInstanceUid.Width = 200;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(768, 314);
			this.Controls.Add(this._openSessions);
			this.Controls.Add(this._activateIfOpen);
			this.Controls.Add(this._studyGrid);
			this.Controls.Add(this._startViewer);
			this.Controls.Add(this._closeViewer);
			this.Controls.Add(this._activateViewer);
			this.Controls.Add(this._openStudy);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.FormLoad);
			((System.ComponentModel.ISupportInitialize)(this._studyGrid)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _openStudy;
		private System.Windows.Forms.Button _activateViewer;
		private System.Windows.Forms.Button _closeViewer;
		private System.Windows.Forms.Button _startViewer;
		private System.Windows.Forms.DataGridView _studyGrid;
		private System.Windows.Forms.CheckBox _activateIfOpen;
		private System.Windows.Forms.ListBox _openSessions;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridViewTextBoxColumn PatientsName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Id;
		private System.Windows.Forms.DataGridViewTextBoxColumn StudyDescription;
		private System.Windows.Forms.DataGridViewTextBoxColumn StudyInstanceUid;
	}
}

