namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    partial class ProtocolEditorComponentControl
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
			this.components = new System.ComponentModel.Container();
			this._tableLayouOuter = new System.Windows.Forms.TableLayoutPanel();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this._btnAccept = new System.Windows.Forms.Button();
			this._btnReject = new System.Windows.Forms.Button();
			this._btnSuspend = new System.Windows.Forms.Button();
			this._btnSave = new System.Windows.Forms.Button();
			this._btnSkip = new System.Windows.Forms.Button();
			this._protocolNextItem = new System.Windows.Forms.CheckBox();
			this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
			this._btnClose = new System.Windows.Forms.Button();
			this._tableLayoutInner = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._protocolCodesSelector = new ClearCanvas.Desktop.View.WinForms.ListItemSelector();
			this._protocolGroup = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._btnSetDefault = new System.Windows.Forms.Button();
			this._grpProcedures = new System.Windows.Forms.GroupBox();
			this._procedurePlanSummary = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._urgency = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this._btnSubmitForApproval = new System.Windows.Forms.Button();
			this._tableLayouOuter.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel3.SuspendLayout();
			this._tableLayoutInner.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this._grpProcedures.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayouOuter
			// 
			this._tableLayouOuter.AutoSize = true;
			this._tableLayouOuter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableLayouOuter.ColumnCount = 2;
			this._tableLayouOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayouOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayouOuter.Controls.Add(this.flowLayoutPanel2, 0, 1);
			this._tableLayouOuter.Controls.Add(this.flowLayoutPanel3, 1, 1);
			this._tableLayouOuter.Controls.Add(this._tableLayoutInner, 0, 0);
			this._tableLayouOuter.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayouOuter.Location = new System.Drawing.Point(0, 0);
			this._tableLayouOuter.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayouOuter.Name = "_tableLayouOuter";
			this._tableLayouOuter.RowCount = 2;
			this._tableLayouOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayouOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayouOuter.Size = new System.Drawing.Size(527, 695);
			this._tableLayouOuter.TabIndex = 0;
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.AutoSize = true;
			this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel2.Controls.Add(this._btnAccept);
			this.flowLayoutPanel2.Controls.Add(this._btnSubmitForApproval);
			this.flowLayoutPanel2.Controls.Add(this._btnReject);
			this.flowLayoutPanel2.Controls.Add(this._btnSuspend);
			this.flowLayoutPanel2.Controls.Add(this._btnSave);
			this.flowLayoutPanel2.Controls.Add(this._btnSkip);
			this.flowLayoutPanel2.Controls.Add(this._protocolNextItem);
			this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 634);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new System.Drawing.Size(434, 58);
			this.flowLayoutPanel2.TabIndex = 0;
			// 
			// _btnAccept
			// 
			this._btnAccept.Location = new System.Drawing.Point(3, 3);
			this._btnAccept.Name = "_btnAccept";
			this._btnAccept.Size = new System.Drawing.Size(75, 23);
			this._btnAccept.TabIndex = 0;
			this._btnAccept.Text = "Accept";
			this._btnAccept.UseVisualStyleBackColor = true;
			this._btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
			// 
			// _btnReject
			// 
			this._btnReject.Location = new System.Drawing.Point(199, 3);
			this._btnReject.Name = "_btnReject";
			this._btnReject.Size = new System.Drawing.Size(75, 23);
			this._btnReject.TabIndex = 1;
			this._btnReject.Text = "Reject";
			this._btnReject.UseVisualStyleBackColor = true;
			this._btnReject.Click += new System.EventHandler(this.btnReject_Click);
			// 
			// _btnSuspend
			// 
			this._btnSuspend.Location = new System.Drawing.Point(280, 3);
			this._btnSuspend.Name = "_btnSuspend";
			this._btnSuspend.Size = new System.Drawing.Size(75, 23);
			this._btnSuspend.TabIndex = 2;
			this._btnSuspend.Text = "Suspend";
			this._btnSuspend.UseVisualStyleBackColor = true;
			this._btnSuspend.Click += new System.EventHandler(this.btnSuspend_Click);
			// 
			// _btnSave
			// 
			this._btnSave.Location = new System.Drawing.Point(3, 32);
			this._btnSave.Name = "_btnSave";
			this._btnSave.Size = new System.Drawing.Size(75, 23);
			this._btnSave.TabIndex = 3;
			this._btnSave.Text = "Save";
			this._btnSave.UseVisualStyleBackColor = true;
			this._btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// _btnSkip
			// 
			this._btnSkip.Location = new System.Drawing.Point(84, 32);
			this._btnSkip.Name = "_btnSkip";
			this._btnSkip.Size = new System.Drawing.Size(75, 23);
			this._btnSkip.TabIndex = 5;
			this._btnSkip.Text = "Skip";
			this._btnSkip.UseVisualStyleBackColor = true;
			this._btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
			// 
			// _protocolNextItem
			// 
			this._protocolNextItem.AutoSize = true;
			this._protocolNextItem.Dock = System.Windows.Forms.DockStyle.Fill;
			this._protocolNextItem.Location = new System.Drawing.Point(165, 32);
			this._protocolNextItem.Name = "_protocolNextItem";
			this._protocolNextItem.Size = new System.Drawing.Size(119, 23);
			this._protocolNextItem.TabIndex = 4;
			this._protocolNextItem.Text = "Protocol Next Order";
			this._protocolNextItem.UseVisualStyleBackColor = true;
			// 
			// flowLayoutPanel3
			// 
			this.flowLayoutPanel3.AutoSize = true;
			this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel3.Controls.Add(this._btnClose);
			this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel3.Location = new System.Drawing.Point(443, 634);
			this.flowLayoutPanel3.Name = "flowLayoutPanel3";
			this.flowLayoutPanel3.Size = new System.Drawing.Size(81, 58);
			this.flowLayoutPanel3.TabIndex = 2;
			// 
			// _btnClose
			// 
			this._btnClose.Location = new System.Drawing.Point(3, 3);
			this._btnClose.Name = "_btnClose";
			this._btnClose.Size = new System.Drawing.Size(75, 23);
			this._btnClose.TabIndex = 1;
			this._btnClose.Text = "Close";
			this._btnClose.UseVisualStyleBackColor = true;
			this._btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// _tableLayoutInner
			// 
			this._tableLayoutInner.ColumnCount = 1;
			this._tableLayouOuter.SetColumnSpan(this._tableLayoutInner, 2);
			this._tableLayoutInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutInner.Controls.Add(this.groupBox1, 0, 2);
			this._tableLayoutInner.Controls.Add(this._grpProcedures, 0, 0);
			this._tableLayoutInner.Controls.Add(this._urgency, 0, 1);
			this._tableLayoutInner.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutInner.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutInner.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayoutInner.Name = "_tableLayoutInner";
			this._tableLayoutInner.RowCount = 3;
			this._tableLayoutInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this._tableLayoutInner.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutInner.Size = new System.Drawing.Size(527, 631);
			this._tableLayoutInner.TabIndex = 3;
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox1.Controls.Add(this.tableLayoutPanel3);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 145);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(527, 486);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Codes";
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.Controls.Add(this._protocolCodesSelector, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this._protocolGroup, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this._btnSetDefault, 1, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(521, 467);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// _protocolCodesSelector
			// 
			this._protocolCodesSelector.AutoSize = true;
			this._protocolCodesSelector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._protocolCodesSelector.AvailableItemsTable = null;
			this.tableLayoutPanel3.SetColumnSpan(this._protocolCodesSelector, 2);
			this._protocolCodesSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this._protocolCodesSelector.Location = new System.Drawing.Point(3, 48);
			this._protocolCodesSelector.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
			this._protocolCodesSelector.Name = "_protocolCodesSelector";
			this._protocolCodesSelector.SelectedItemsTable = null;
			this._protocolCodesSelector.ShowColumnHeading = false;
			this._protocolCodesSelector.ShowToolbars = false;
			this._protocolCodesSelector.Size = new System.Drawing.Size(498, 416);
			this._protocolCodesSelector.TabIndex = 0;
			// 
			// _protocolGroup
			// 
			this._protocolGroup.AutoSize = true;
			this._protocolGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._protocolGroup.DataSource = null;
			this._protocolGroup.DisplayMember = "";
			this._protocolGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this._protocolGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._protocolGroup.LabelText = "Protocol Group";
			this._protocolGroup.Location = new System.Drawing.Point(2, 2);
			this._protocolGroup.Margin = new System.Windows.Forms.Padding(2);
			this._protocolGroup.Name = "_protocolGroup";
			this._protocolGroup.Size = new System.Drawing.Size(256, 41);
			this._protocolGroup.TabIndex = 0;
			this._protocolGroup.Value = null;
			// 
			// _btnSetDefault
			// 
			this._btnSetDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._btnSetDefault.AutoSize = true;
			this._btnSetDefault.Location = new System.Drawing.Point(263, 19);
			this._btnSetDefault.Name = "_btnSetDefault";
			this._btnSetDefault.Size = new System.Drawing.Size(85, 23);
			this._btnSetDefault.TabIndex = 1;
			this._btnSetDefault.Text = "Set As Default";
			this._btnSetDefault.UseVisualStyleBackColor = true;
			this._btnSetDefault.Click += new System.EventHandler(this._btnSetDefault_Click);
			// 
			// _grpProcedures
			// 
			this._grpProcedures.AutoSize = true;
			this._grpProcedures.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._grpProcedures.Controls.Add(this._procedurePlanSummary);
			this._grpProcedures.Dock = System.Windows.Forms.DockStyle.Fill;
			this._grpProcedures.Location = new System.Drawing.Point(0, 0);
			this._grpProcedures.Margin = new System.Windows.Forms.Padding(0);
			this._grpProcedures.Name = "_grpProcedures";
			this._grpProcedures.Size = new System.Drawing.Size(527, 100);
			this._grpProcedures.TabIndex = 0;
			this._grpProcedures.TabStop = false;
			this._grpProcedures.Text = "Procedures";
			// 
			// _procedurePlanSummary
			// 
			this._procedurePlanSummary.AutoSize = true;
			this._procedurePlanSummary.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._procedurePlanSummary.Dock = System.Windows.Forms.DockStyle.Fill;
			this._procedurePlanSummary.Location = new System.Drawing.Point(3, 16);
			this._procedurePlanSummary.MultiSelect = false;
			this._procedurePlanSummary.Name = "_procedurePlanSummary";
			this._procedurePlanSummary.ReadOnly = false;
			this._procedurePlanSummary.ShowToolbar = false;
			this._procedurePlanSummary.Size = new System.Drawing.Size(521, 81);
			this._procedurePlanSummary.TabIndex = 0;
			this._procedurePlanSummary.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			// 
			// _urgency
			// 
			this._urgency.AutoSize = true;
			this._urgency.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._urgency.DataSource = null;
			this._urgency.DisplayMember = "";
			this._urgency.Dock = System.Windows.Forms.DockStyle.Fill;
			this._urgency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._urgency.LabelText = "Urgency";
			this._urgency.Location = new System.Drawing.Point(2, 102);
			this._urgency.Margin = new System.Windows.Forms.Padding(2, 2, 20, 2);
			this._urgency.Name = "_urgency";
			this._urgency.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this._urgency.Size = new System.Drawing.Size(505, 41);
			this._urgency.TabIndex = 1;
			this._urgency.Value = null;
			// 
			// _btnSubmitForApproval
			// 
			this._btnSubmitForApproval.AutoSize = true;
			this._btnSubmitForApproval.Location = new System.Drawing.Point(84, 3);
			this._btnSubmitForApproval.Name = "_btnSubmitForApproval";
			this._btnSubmitForApproval.Size = new System.Drawing.Size(109, 23);
			this._btnSubmitForApproval.TabIndex = 6;
			this._btnSubmitForApproval.Text = "Submit for Approval";
			this._btnSubmitForApproval.UseVisualStyleBackColor = true;
			this._btnSubmitForApproval.Click += new System.EventHandler(this._btnSubmitForApproval_Click);
			// 
			// ProtocolEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._tableLayouOuter);
			this.Name = "ProtocolEditorComponentControl";
			this.Size = new System.Drawing.Size(527, 695);
			this._tableLayouOuter.ResumeLayout(false);
			this._tableLayouOuter.PerformLayout();
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel2.PerformLayout();
			this.flowLayoutPanel3.ResumeLayout(false);
			this._tableLayoutInner.ResumeLayout(false);
			this._tableLayoutInner.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this._grpProcedures.ResumeLayout(false);
			this._grpProcedures.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.ListItemSelector _protocolCodesSelector;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _protocolGroup;
        private System.Windows.Forms.TableLayoutPanel _tableLayouOuter;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button _btnAccept;
        private System.Windows.Forms.Button _btnReject;
        private System.Windows.Forms.Button _btnSuspend;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button _btnClose;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.CheckBox _protocolNextItem;
        private System.Windows.Forms.Button _btnSkip;
        private System.Windows.Forms.GroupBox _grpProcedures;
        private ClearCanvas.Desktop.View.WinForms.TableView _procedurePlanSummary;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutInner;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button _btnSetDefault;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _urgency;
		private System.Windows.Forms.Button _btnSubmitForApproval;
    }
}
