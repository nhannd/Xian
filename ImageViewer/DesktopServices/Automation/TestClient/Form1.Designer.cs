namespace ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient
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
			this.components = new System.ComponentModel.Container();
			this._openStudy = new System.Windows.Forms.Button();
			this._activateViewer = new System.Windows.Forms.Button();
			this._closeViewer = new System.Windows.Forms.Button();
			this._startViewer = new System.Windows.Forms.Button();
			this._studyGrid = new System.Windows.Forms.DataGridView();
			this._studyItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this._activateIfOpen = new System.Windows.Forms.CheckBox();
			this._openSessions = new System.Windows.Forms.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this._refreshAllSessions = new System.Windows.Forms.Button();
			this._getSelectedInfo = new System.Windows.Forms.Button();
			this._requery = new System.Windows.Forms.Button();
			this.patientIdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.patientsNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.studyDescriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.RetrieveAETitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.hasSessionsDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.studyInstanceUidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this._studyGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._studyItemBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// _openStudy
			// 
			this._openStudy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._openStudy.Location = new System.Drawing.Point(786, 223);
			this._openStudy.Name = "_openStudy";
			this._openStudy.Size = new System.Drawing.Size(194, 23);
			this._openStudy.TabIndex = 0;
			this._openStudy.Text = "Open Study";
			this._openStudy.UseVisualStyleBackColor = true;
			this._openStudy.Click += new System.EventHandler(this.OnOpenStudy);
			// 
			// _activateViewer
			// 
			this._activateViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._activateViewer.Location = new System.Drawing.Point(786, 252);
			this._activateViewer.Name = "_activateViewer";
			this._activateViewer.Size = new System.Drawing.Size(194, 23);
			this._activateViewer.TabIndex = 1;
			this._activateViewer.Text = "Activate Viewer";
			this._activateViewer.UseVisualStyleBackColor = true;
			this._activateViewer.Click += new System.EventHandler(this.OnActivateViewer);
			// 
			// _closeViewer
			// 
			this._closeViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._closeViewer.Location = new System.Drawing.Point(786, 281);
			this._closeViewer.Name = "_closeViewer";
			this._closeViewer.Size = new System.Drawing.Size(194, 23);
			this._closeViewer.TabIndex = 2;
			this._closeViewer.Text = "Close Viewer";
			this._closeViewer.UseVisualStyleBackColor = true;
			this._closeViewer.Click += new System.EventHandler(this.OnCloseViewer);
			// 
			// _startViewer
			// 
			this._startViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._startViewer.Location = new System.Drawing.Point(786, 194);
			this._startViewer.Name = "_startViewer";
			this._startViewer.Size = new System.Drawing.Size(194, 23);
			this._startViewer.TabIndex = 3;
			this._startViewer.Text = "Start Viewer";
			this._startViewer.UseVisualStyleBackColor = true;
			this._startViewer.Click += new System.EventHandler(this.OnStartViewer);
			// 
			// _studyGrid
			// 
			this._studyGrid.AllowUserToAddRows = false;
			this._studyGrid.AllowUserToDeleteRows = false;
			this._studyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._studyGrid.AutoGenerateColumns = false;
			this._studyGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this._studyGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this._studyGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this._studyGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.patientIdDataGridViewTextBoxColumn,
            this.patientsNameDataGridViewTextBoxColumn,
            this.studyDescriptionDataGridViewTextBoxColumn,
            this.RetrieveAETitle,
            this.hasSessionsDataGridViewCheckBoxColumn,
            this.studyInstanceUidDataGridViewTextBoxColumn});
			this._studyGrid.DataSource = this._studyItemBindingSource;
			this._studyGrid.Location = new System.Drawing.Point(12, 12);
			this._studyGrid.Name = "_studyGrid";
			this._studyGrid.ReadOnly = true;
			this._studyGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this._studyGrid.Size = new System.Drawing.Size(768, 292);
			this._studyGrid.TabIndex = 5;
			// 
			// _studyItemBindingSource
			// 
			this._studyItemBindingSource.DataSource = typeof(ClearCanvas.ImageViewer.DesktopServices.Automation.TestClient.StudyItem);
			// 
			// _activateIfOpen
			// 
			this._activateIfOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._activateIfOpen.AutoSize = true;
			this._activateIfOpen.Checked = true;
			this._activateIfOpen.CheckState = System.Windows.Forms.CheckState.Checked;
			this._activateIfOpen.Location = new System.Drawing.Point(787, 311);
			this._activateIfOpen.Name = "_activateIfOpen";
			this._activateIfOpen.Size = new System.Drawing.Size(137, 17);
			this._activateIfOpen.TabIndex = 6;
			this._activateIfOpen.Text = "Activate if already open";
			this._activateIfOpen.UseVisualStyleBackColor = true;
			// 
			// _openSessions
			// 
			this._openSessions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._openSessions.FormattingEnabled = true;
			this._openSessions.Location = new System.Drawing.Point(787, 24);
			this._openSessions.Name = "_openSessions";
			this._openSessions.Size = new System.Drawing.Size(193, 160);
			this._openSessions.TabIndex = 7;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(787, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 13);
			this.label1.TabIndex = 8;
			this.label1.Text = "Open Sessions";
			// 
			// _refreshAllSessions
			// 
			this._refreshAllSessions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._refreshAllSessions.Location = new System.Drawing.Point(601, 311);
			this._refreshAllSessions.Name = "_refreshAllSessions";
			this._refreshAllSessions.Size = new System.Drawing.Size(179, 23);
			this._refreshAllSessions.TabIndex = 9;
			this._refreshAllSessions.Text = "Refresh Sessions";
			this._refreshAllSessions.UseVisualStyleBackColor = true;
			this._refreshAllSessions.Click += new System.EventHandler(this.OnRefreshAllSessions);
			// 
			// _getSelectedInfo
			// 
			this._getSelectedInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._getSelectedInfo.Location = new System.Drawing.Point(416, 311);
			this._getSelectedInfo.Name = "_getSelectedInfo";
			this._getSelectedInfo.Size = new System.Drawing.Size(179, 23);
			this._getSelectedInfo.TabIndex = 10;
			this._getSelectedInfo.Text = "Get Additional Info";
			this._getSelectedInfo.UseVisualStyleBackColor = true;
			this._getSelectedInfo.Click += new System.EventHandler(this.OnGetSelectedInfo);
			// 
			// _requery
			// 
			this._requery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._requery.Location = new System.Drawing.Point(12, 311);
			this._requery.Name = "_requery";
			this._requery.Size = new System.Drawing.Size(179, 23);
			this._requery.TabIndex = 11;
			this._requery.Text = "Requery";
			this._requery.UseVisualStyleBackColor = true;
			this._requery.Click += new System.EventHandler(this.OnRequery);
			// 
			// patientIdDataGridViewTextBoxColumn
			// 
			this.patientIdDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.patientIdDataGridViewTextBoxColumn.DataPropertyName = "PatientId";
			this.patientIdDataGridViewTextBoxColumn.HeaderText = "Patient Id";
			this.patientIdDataGridViewTextBoxColumn.Name = "patientIdDataGridViewTextBoxColumn";
			this.patientIdDataGridViewTextBoxColumn.ReadOnly = true;
			this.patientIdDataGridViewTextBoxColumn.Width = 77;
			// 
			// patientsNameDataGridViewTextBoxColumn
			// 
			this.patientsNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.patientsNameDataGridViewTextBoxColumn.DataPropertyName = "PatientsName";
			this.patientsNameDataGridViewTextBoxColumn.HeaderText = "Patient\'s Name";
			this.patientsNameDataGridViewTextBoxColumn.Name = "patientsNameDataGridViewTextBoxColumn";
			this.patientsNameDataGridViewTextBoxColumn.ReadOnly = true;
			this.patientsNameDataGridViewTextBoxColumn.Width = 95;
			// 
			// studyDescriptionDataGridViewTextBoxColumn
			// 
			this.studyDescriptionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.studyDescriptionDataGridViewTextBoxColumn.DataPropertyName = "StudyDescription";
			this.studyDescriptionDataGridViewTextBoxColumn.HeaderText = "Study Description";
			this.studyDescriptionDataGridViewTextBoxColumn.Name = "studyDescriptionDataGridViewTextBoxColumn";
			this.studyDescriptionDataGridViewTextBoxColumn.ReadOnly = true;
			this.studyDescriptionDataGridViewTextBoxColumn.Width = 106;
			// 
			// RetrieveAETitle
			// 
			this.RetrieveAETitle.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.RetrieveAETitle.DataPropertyName = "RetrieveAETitle";
			this.RetrieveAETitle.HeaderText = "Retrieve AE";
			this.RetrieveAETitle.Name = "RetrieveAETitle";
			this.RetrieveAETitle.ReadOnly = true;
			this.RetrieveAETitle.Width = 82;
			// 
			// hasSessionsDataGridViewCheckBoxColumn
			// 
			this.hasSessionsDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
			this.hasSessionsDataGridViewCheckBoxColumn.DataPropertyName = "HasSessions";
			this.hasSessionsDataGridViewCheckBoxColumn.HeaderText = "Has Sessions";
			this.hasSessionsDataGridViewCheckBoxColumn.Name = "hasSessionsDataGridViewCheckBoxColumn";
			this.hasSessionsDataGridViewCheckBoxColumn.ReadOnly = true;
			this.hasSessionsDataGridViewCheckBoxColumn.Width = 69;
			// 
			// studyInstanceUidDataGridViewTextBoxColumn
			// 
			this.studyInstanceUidDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.studyInstanceUidDataGridViewTextBoxColumn.DataPropertyName = "StudyInstanceUid";
			this.studyInstanceUidDataGridViewTextBoxColumn.HeaderText = "Study Instance Uid";
			this.studyInstanceUidDataGridViewTextBoxColumn.Name = "studyInstanceUidDataGridViewTextBoxColumn";
			this.studyInstanceUidDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(992, 341);
			this.Controls.Add(this._requery);
			this.Controls.Add(this._getSelectedInfo);
			this.Controls.Add(this._refreshAllSessions);
			this.Controls.Add(this._openSessions);
			this.Controls.Add(this._activateIfOpen);
			this.Controls.Add(this._studyGrid);
			this.Controls.Add(this._startViewer);
			this.Controls.Add(this._closeViewer);
			this.Controls.Add(this._activateViewer);
			this.Controls.Add(this._openStudy);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.OnFormLoad);
			((System.ComponentModel.ISupportInitialize)(this._studyGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._studyItemBindingSource)).EndInit();
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
		private System.Windows.Forms.Button _refreshAllSessions;
		private System.Windows.Forms.Button _getSelectedInfo;
		private System.Windows.Forms.BindingSource _studyItemBindingSource;
		private System.Windows.Forms.Button _requery;
		private System.Windows.Forms.DataGridViewTextBoxColumn patientIdDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn patientsNameDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn studyDescriptionDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn RetrieveAETitle;
		private System.Windows.Forms.DataGridViewCheckBoxColumn hasSessionsDataGridViewCheckBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn studyInstanceUidDataGridViewTextBoxColumn;
	}
}

