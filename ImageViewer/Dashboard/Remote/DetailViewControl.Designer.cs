namespace ClearCanvas.ImageViewer.Dashboard.Remote
{
	partial class DetailViewControl
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
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.PatientID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PatientName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.StudyDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridView1
			// 
			this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PatientID,
            this.PatientName,
            this.StudyDescription});
			this.dataGridView1.Location = new System.Drawing.Point(0, 248);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.Size = new System.Drawing.Size(550, 150);
			this.dataGridView1.TabIndex = 0;
			// 
			// PatientID
			// 
			this.PatientID.HeaderText = "Patient ID";
			this.PatientID.Name = "PatientID";
			// 
			// PatientName
			// 
			this.PatientName.HeaderText = "Patient Name";
			this.PatientName.Name = "PatientName";
			// 
			// StudyDescription
			// 
			this.StudyDescription.HeaderText = "Study Description";
			this.StudyDescription.Name = "StudyDescription";
			// 
			// DetailViewControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.dataGridView1);
			this.Name = "DetailViewControl";
			this.Size = new System.Drawing.Size(553, 401);
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.DataGridViewTextBoxColumn PatientID;
		private System.Windows.Forms.DataGridViewTextBoxColumn PatientName;
		private System.Windows.Forms.DataGridViewTextBoxColumn StudyDescription;

	}
}
