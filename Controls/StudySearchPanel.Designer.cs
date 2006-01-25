namespace ClearCanvas.Workstation.Dashboard.Local
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
			this._studyGridView = new ClearCanvas.Controls.StudyGridView();
			this._studySearchForm = new ClearCanvas.Controls.StudySearchForm();
			this.SuspendLayout();
			// 
			// _studyGridView
			// 
			this._studyGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._studyGridView.Location = new System.Drawing.Point(0, 188);
			this._studyGridView.Margin = new System.Windows.Forms.Padding(0);
			this._studyGridView.Name = "_studyGridView";
			this._studyGridView.Size = new System.Drawing.Size(615, 261);
			this._studyGridView.TabIndex = 1;
			// 
			// _studySearchForm
			// 
			this._studySearchForm.AccessionNumber = "";
			this._studySearchForm.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._studySearchForm.FirstName = "";
			this._studySearchForm.HeaderText = "headerStrip1";
			this._studySearchForm.LastName = "";
			this._studySearchForm.Location = new System.Drawing.Point(0, 0);
			this._studySearchForm.Margin = new System.Windows.Forms.Padding(0);
			this._studySearchForm.Name = "_studySearchForm";
			this._studySearchForm.PatientID = "";
			this._studySearchForm.Size = new System.Drawing.Size(614, 188);
			this._studySearchForm.StudyDescription = "";
			this._studySearchForm.TabIndex = 0;
			// 
			// DetailViewControl
			// 
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._studyGridView);
			this.Controls.Add(this._studySearchForm);
			this.Name = "DetailViewControl";
			this.Size = new System.Drawing.Size(613, 448);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Controls.StudySearchForm _studySearchForm;
		private ClearCanvas.Controls.StudyGridView _studyGridView;


	}
}
