namespace ClearCanvas.Controls
{
	partial class StudySearchPanel
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
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._headerStrip = new ClearCanvas.Controls.HeaderStrip();
			this._headerStripLabel = new System.Windows.Forms.ToolStripLabel();
			this._studyGridView = new ClearCanvas.Controls.StudyGridView();
			this._studySearchForm = new ClearCanvas.Controls.StudySearchForm();
			this._tableLayoutPanel.SuspendLayout();
			this._headerStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.ColumnCount = 1;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.Controls.Add(this._headerStrip, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._studyGridView, 0, 2);
			this._tableLayoutPanel.Controls.Add(this._studySearchForm, 0, 1);
			this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 3;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.Size = new System.Drawing.Size(613, 448);
			this._tableLayoutPanel.TabIndex = 2;
			// 
			// _headerStrip
			// 
			this._headerStrip.AutoSize = false;
			this._headerStrip.Font = new System.Drawing.Font("Arial", 13.5F, System.Drawing.FontStyle.Bold);
			this._headerStrip.ForeColor = System.Drawing.Color.White;
			this._headerStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._headerStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._headerStripLabel});
			this._headerStrip.Location = new System.Drawing.Point(0, 0);
			this._headerStrip.Name = "_headerStrip";
			this._headerStrip.Size = new System.Drawing.Size(613, 27);
			this._headerStrip.TabIndex = 0;
			this._headerStrip.Text = "headerStrip1";
			// 
			// _headerStripLabel
			// 
			this._headerStripLabel.Name = "_headerStripLabel";
			this._headerStripLabel.Size = new System.Drawing.Size(0, 24);
			// 
			// _studyGridView
			// 
			this._studyGridView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._studyGridView.Location = new System.Drawing.Point(0, 215);
			this._studyGridView.Margin = new System.Windows.Forms.Padding(0);
			this._studyGridView.Name = "_studyGridView";
			this._studyGridView.Size = new System.Drawing.Size(613, 233);
			this._studyGridView.TabIndex = 1;
			// 
			// _studySearchForm
			// 
			this._studySearchForm.AccessionNumber = "";
			this._studySearchForm.Dock = System.Windows.Forms.DockStyle.Fill;
			this._studySearchForm.FirstName = "";
			this._studySearchForm.LastName = "";
			this._studySearchForm.Location = new System.Drawing.Point(0, 27);
			this._studySearchForm.Margin = new System.Windows.Forms.Padding(0);
			this._studySearchForm.Name = "_studySearchForm";
			this._studySearchForm.PatientId = "";
			this._studySearchForm.Size = new System.Drawing.Size(613, 188);
			this._studySearchForm.StudyDescription = "";
			this._studySearchForm.TabIndex = 0;
			// 
			// StudySearchPanel
			// 
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "StudySearchPanel";
			this.Size = new System.Drawing.Size(613, 448);
			this._tableLayoutPanel.ResumeLayout(false);
			this._headerStrip.ResumeLayout(false);
			this._headerStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Controls.StudySearchForm _studySearchForm;
		private ClearCanvas.Controls.StudyGridView _studyGridView;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private HeaderStrip _headerStrip;
		private System.Windows.Forms.ToolStripLabel _headerStripLabel;


	}
}
