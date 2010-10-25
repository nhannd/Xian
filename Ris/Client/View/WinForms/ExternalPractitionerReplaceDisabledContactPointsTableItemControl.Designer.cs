namespace ClearCanvas.Ris.Client.View.WinForms
{
	partial class ExternalPractitionerReplaceDisabledContactPointsTableItemControl
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
			this._replacedWith = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._oldContactPointInfo = new System.Windows.Forms.Label();
			this._newContactPointInfo = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._affectedOrderCount = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _replacedWith
			// 
			this._replacedWith.DataSource = null;
			this._replacedWith.DisplayMember = "";
			this._replacedWith.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._replacedWith.LabelText = "Replace With";
			this._replacedWith.Location = new System.Drawing.Point(302, 2);
			this._replacedWith.Margin = new System.Windows.Forms.Padding(2);
			this._replacedWith.Name = "_replacedWith";
			this._replacedWith.Size = new System.Drawing.Size(207, 40);
			this._replacedWith.TabIndex = 4;
			this._replacedWith.Value = null;
			// 
			// _oldContactPointInfo
			// 
			this._oldContactPointInfo.AutoSize = true;
			this._oldContactPointInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._oldContactPointInfo.Location = new System.Drawing.Point(3, 3);
			this._oldContactPointInfo.Margin = new System.Windows.Forms.Padding(3);
			this._oldContactPointInfo.Name = "_oldContactPointInfo";
			this.tableLayoutPanel1.SetRowSpan(this._oldContactPointInfo, 2);
			this._oldContactPointInfo.Size = new System.Drawing.Size(294, 115);
			this._oldContactPointInfo.TabIndex = 5;
			this._oldContactPointInfo.Text = "Old Contact Point Info";
			// 
			// _newContactPointInfo
			// 
			this._newContactPointInfo.AutoSize = true;
			this._newContactPointInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._newContactPointInfo.Location = new System.Drawing.Point(303, 47);
			this._newContactPointInfo.Margin = new System.Windows.Forms.Padding(3);
			this._newContactPointInfo.Name = "_newContactPointInfo";
			this._newContactPointInfo.Size = new System.Drawing.Size(294, 71);
			this._newContactPointInfo.TabIndex = 6;
			this._newContactPointInfo.Text = "New Contact Point Info";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this._replacedWith, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this._newContactPointInfo, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this._oldContactPointInfo, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._affectedOrderCount, 0, 2);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 134);
			this.tableLayoutPanel1.TabIndex = 9;
			// 
			// _affectedOrderCount
			// 
			this._affectedOrderCount.AutoSize = true;
			this.tableLayoutPanel1.SetColumnSpan(this._affectedOrderCount, 2);
			this._affectedOrderCount.Dock = System.Windows.Forms.DockStyle.Fill;
			this._affectedOrderCount.Location = new System.Drawing.Point(3, 121);
			this._affectedOrderCount.Name = "_affectedOrderCount";
			this._affectedOrderCount.Size = new System.Drawing.Size(594, 13);
			this._affectedOrderCount.TabIndex = 7;
			this._affectedOrderCount.Text = "XXX Orders will be updated";
			// 
			// ExternalPractitionerReplaceDisabledContactPointsTableItemControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExternalPractitionerReplaceDisabledContactPointsTableItemControl";
			this.Size = new System.Drawing.Size(600, 134);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _replacedWith;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label _oldContactPointInfo;
		private System.Windows.Forms.Label _newContactPointInfo;
		private System.Windows.Forms.Label _affectedOrderCount;
	}
}
