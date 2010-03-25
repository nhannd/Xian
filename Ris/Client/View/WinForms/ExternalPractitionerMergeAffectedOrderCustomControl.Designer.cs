namespace ClearCanvas.Ris.Client.View.WinForms
{
	partial class ExternalPractitionerMergeAffectedOrderCustomControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._oldContactPointInfo = new System.Windows.Forms.Label();
			this._newContactPointInfo = new System.Windows.Forms.Label();
			this._orderFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this._orderLink = new System.Windows.Forms.LinkLabel();
			this._practitionerFlowPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._practitionerRole = new System.Windows.Forms.Label();
			this._practitionerLink = new System.Windows.Forms.LinkLabel();
			this.tableLayoutPanel1.SuspendLayout();
			this._orderFlowPanel.SuspendLayout();
			this._practitionerFlowPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _replacedWith
			// 
			this._replacedWith.DataSource = null;
			this._replacedWith.DisplayMember = "";
			this._replacedWith.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._replacedWith.LabelText = "Replace With";
			this._replacedWith.Location = new System.Drawing.Point(391, 2);
			this._replacedWith.Margin = new System.Windows.Forms.Padding(2);
			this._replacedWith.Name = "_replacedWith";
			this.tableLayoutPanel1.SetRowSpan(this._replacedWith, 2);
			this._replacedWith.Size = new System.Drawing.Size(207, 40);
			this._replacedWith.TabIndex = 4;
			this._replacedWith.Value = null;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.83334F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.16667F));
			this.tableLayoutPanel1.Controls.Add(this._replacedWith, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this._oldContactPointInfo, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._newContactPointInfo, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this._orderFlowPanel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._practitionerFlowPanel, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(600, 134);
			this.tableLayoutPanel1.TabIndex = 8;
			// 
			// _oldContactPointInfo
			// 
			this._oldContactPointInfo.AutoSize = true;
			this._oldContactPointInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._oldContactPointInfo.Location = new System.Drawing.Point(3, 47);
			this._oldContactPointInfo.Margin = new System.Windows.Forms.Padding(3);
			this._oldContactPointInfo.Name = "_oldContactPointInfo";
			this._oldContactPointInfo.Size = new System.Drawing.Size(383, 84);
			this._oldContactPointInfo.TabIndex = 5;
			this._oldContactPointInfo.Text = "Old Contact Point Info";
			// 
			// _newContactPointInfo
			// 
			this._newContactPointInfo.AutoSize = true;
			this._newContactPointInfo.Dock = System.Windows.Forms.DockStyle.Fill;
			this._newContactPointInfo.Location = new System.Drawing.Point(392, 47);
			this._newContactPointInfo.Margin = new System.Windows.Forms.Padding(3);
			this._newContactPointInfo.Name = "_newContactPointInfo";
			this._newContactPointInfo.Size = new System.Drawing.Size(205, 84);
			this._newContactPointInfo.TabIndex = 6;
			this._newContactPointInfo.Text = "New Contact Point Info";
			// 
			// _orderFlowPanel
			// 
			this._orderFlowPanel.Controls.Add(this.label1);
			this._orderFlowPanel.Controls.Add(this._orderLink);
			this._orderFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderFlowPanel.Location = new System.Drawing.Point(0, 0);
			this._orderFlowPanel.Margin = new System.Windows.Forms.Padding(0);
			this._orderFlowPanel.Name = "_orderFlowPanel";
			this._orderFlowPanel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this._orderFlowPanel.Size = new System.Drawing.Size(389, 22);
			this._orderFlowPanel.TabIndex = 8;
			this._orderFlowPanel.WrapContents = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 5);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(33, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Order";
			// 
			// _orderLink
			// 
			this._orderLink.AutoSize = true;
			this._orderLink.Location = new System.Drawing.Point(42, 5);
			this._orderLink.Name = "_orderLink";
			this._orderLink.Size = new System.Drawing.Size(33, 13);
			this._orderLink.TabIndex = 1;
			this._orderLink.TabStop = true;
			this._orderLink.Text = "Order";
			// 
			// _practitionerFlowPanel
			// 
			this._practitionerFlowPanel.Controls.Add(this._practitionerRole);
			this._practitionerFlowPanel.Controls.Add(this._practitionerLink);
			this._practitionerFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._practitionerFlowPanel.Location = new System.Drawing.Point(0, 22);
			this._practitionerFlowPanel.Margin = new System.Windows.Forms.Padding(0);
			this._practitionerFlowPanel.Name = "_practitionerFlowPanel";
			this._practitionerFlowPanel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
			this._practitionerFlowPanel.Size = new System.Drawing.Size(389, 22);
			this._practitionerFlowPanel.TabIndex = 9;
			this._practitionerFlowPanel.WrapContents = false;
			// 
			// _practitionerRole
			// 
			this._practitionerRole.AutoSize = true;
			this._practitionerRole.Location = new System.Drawing.Point(3, 5);
			this._practitionerRole.Name = "_practitionerRole";
			this._practitionerRole.Size = new System.Drawing.Size(29, 13);
			this._practitionerRole.TabIndex = 0;
			this._practitionerRole.Text = "Role";
			// 
			// _practitionerLink
			// 
			this._practitionerLink.AutoSize = true;
			this._practitionerLink.Location = new System.Drawing.Point(38, 5);
			this._practitionerLink.Name = "_practitionerLink";
			this._practitionerLink.Size = new System.Drawing.Size(60, 13);
			this._practitionerLink.TabIndex = 1;
			this._practitionerLink.TabStop = true;
			this._practitionerLink.Text = "Practitioner";
			// 
			// ExternalPractitionerMergeAffectedOrderCustomControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ExternalPractitionerMergeAffectedOrderCustomControl";
			this.Size = new System.Drawing.Size(600, 134);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this._orderFlowPanel.ResumeLayout(false);
			this._orderFlowPanel.PerformLayout();
			this._practitionerFlowPanel.ResumeLayout(false);
			this._practitionerFlowPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _replacedWith;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label _oldContactPointInfo;
		private System.Windows.Forms.Label _newContactPointInfo;
		private System.Windows.Forms.LinkLabel _orderLink;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.FlowLayoutPanel _orderFlowPanel;
		private System.Windows.Forms.FlowLayoutPanel _practitionerFlowPanel;
		private System.Windows.Forms.Label _practitionerRole;
		private System.Windows.Forms.LinkLabel _practitionerLink;

	}
}
