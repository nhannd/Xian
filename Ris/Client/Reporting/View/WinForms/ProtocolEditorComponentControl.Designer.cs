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
            this._tableLayouOuter = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnReject = new System.Windows.Forms.Button();
            this.btnSuspend = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSkip = new System.Windows.Forms.Button();
            this._protocolNextItem = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this._tableLayoutInner = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.protocolCodesSelector = new ClearCanvas.Desktop.View.WinForms.ListItemSelector();
            this._protocolGroup = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._grpProcedures = new System.Windows.Forms.GroupBox();
            this._procedurePlanSummary = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this._orderNotesPanel = new System.Windows.Forms.Panel();
            this._tableLayouOuter.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this._tableLayoutInner.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this._grpProcedures.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this._tableLayouOuter.Size = new System.Drawing.Size(512, 600);
            this._tableLayouOuter.TabIndex = 0;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.btnAccept);
            this.flowLayoutPanel2.Controls.Add(this.btnReject);
            this.flowLayoutPanel2.Controls.Add(this.btnSuspend);
            this.flowLayoutPanel2.Controls.Add(this.btnSave);
            this.flowLayoutPanel2.Controls.Add(this.btnSkip);
            this.flowLayoutPanel2.Controls.Add(this._protocolNextItem);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 545);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(419, 52);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(3, 3);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnReject
            // 
            this.btnReject.Location = new System.Drawing.Point(84, 3);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new System.Drawing.Size(75, 23);
            this.btnReject.TabIndex = 1;
            this.btnReject.Text = "Reject";
            this.btnReject.UseVisualStyleBackColor = true;
            this.btnReject.Click += new System.EventHandler(this.btnReject_Click);
            // 
            // btnSuspend
            // 
            this.btnSuspend.Location = new System.Drawing.Point(165, 3);
            this.btnSuspend.Name = "btnSuspend";
            this.btnSuspend.Size = new System.Drawing.Size(75, 23);
            this.btnSuspend.TabIndex = 2;
            this.btnSuspend.Text = "Suspend";
            this.btnSuspend.UseVisualStyleBackColor = true;
            this.btnSuspend.Click += new System.EventHandler(this.btnSuspend_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(246, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSkip
            // 
            this.btnSkip.Location = new System.Drawing.Point(327, 3);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(75, 23);
            this.btnSkip.TabIndex = 5;
            this.btnSkip.Text = "Skip";
            this.btnSkip.UseVisualStyleBackColor = true;
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // _protocolNextItem
            // 
            this._protocolNextItem.AutoSize = true;
            this._protocolNextItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this._protocolNextItem.Location = new System.Drawing.Point(3, 32);
            this._protocolNextItem.Name = "_protocolNextItem";
            this._protocolNextItem.Size = new System.Drawing.Size(119, 17);
            this._protocolNextItem.TabIndex = 4;
            this._protocolNextItem.Text = "Protocol Next Order";
            this._protocolNextItem.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel3.Controls.Add(this.btnClose);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(428, 545);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(81, 52);
            this.flowLayoutPanel3.TabIndex = 2;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(3, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // _tableLayoutInner
            // 
            this._tableLayoutInner.ColumnCount = 1;
            this._tableLayouOuter.SetColumnSpan(this._tableLayoutInner, 2);
            this._tableLayoutInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutInner.Controls.Add(this.groupBox1, 0, 1);
            this._tableLayoutInner.Controls.Add(this._grpProcedures, 0, 0);
            this._tableLayoutInner.Controls.Add(this.groupBox2, 0, 2);
            this._tableLayoutInner.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutInner.Location = new System.Drawing.Point(3, 3);
            this._tableLayoutInner.Name = "_tableLayoutInner";
            this._tableLayoutInner.RowCount = 3;
            this._tableLayoutInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this._tableLayoutInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this._tableLayoutInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this._tableLayoutInner.Size = new System.Drawing.Size(506, 536);
            this._tableLayoutInner.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.tableLayoutPanel3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 103);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(500, 255);
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
            this.tableLayoutPanel3.Controls.Add(this.protocolCodesSelector, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this._protocolGroup, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(494, 236);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // protocolCodesSelector
            // 
            this.protocolCodesSelector.AutoSize = true;
            this.protocolCodesSelector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.protocolCodesSelector.AvailableItemsTable = null;
            this.tableLayoutPanel3.SetColumnSpan(this.protocolCodesSelector, 2);
            this.protocolCodesSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.protocolCodesSelector.Location = new System.Drawing.Point(3, 48);
            this.protocolCodesSelector.Name = "protocolCodesSelector";
            this.protocolCodesSelector.SelectedItemsTable = null;
            this.protocolCodesSelector.Size = new System.Drawing.Size(488, 185);
            this.protocolCodesSelector.TabIndex = 0;
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
            this._protocolGroup.Size = new System.Drawing.Size(243, 41);
            this._protocolGroup.TabIndex = 0;
            this._protocolGroup.Value = null;
            // 
            // _grpProcedures
            // 
            this._grpProcedures.AutoSize = true;
            this._grpProcedures.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._grpProcedures.Controls.Add(this._procedurePlanSummary);
            this._grpProcedures.Dock = System.Windows.Forms.DockStyle.Fill;
            this._grpProcedures.Location = new System.Drawing.Point(3, 3);
            this._grpProcedures.Name = "_grpProcedures";
            this._grpProcedures.Size = new System.Drawing.Size(500, 94);
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
            this._procedurePlanSummary.Size = new System.Drawing.Size(494, 75);
            this._procedurePlanSummary.TabIndex = 0;
            this._procedurePlanSummary.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this._orderNotesPanel);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 364);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(500, 169);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Order Notes";
            // 
            // _orderNotesPanel
            // 
            this._orderNotesPanel.AutoSize = true;
            this._orderNotesPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._orderNotesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._orderNotesPanel.Location = new System.Drawing.Point(3, 16);
            this._orderNotesPanel.Name = "_orderNotesPanel";
            this._orderNotesPanel.Size = new System.Drawing.Size(494, 150);
            this._orderNotesPanel.TabIndex = 1;
            // 
            // ProtocolEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._tableLayouOuter);
            this.Name = "ProtocolEditorComponentControl";
            this.Size = new System.Drawing.Size(512, 600);
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
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.ListItemSelector protocolCodesSelector;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _protocolGroup;
        private System.Windows.Forms.TableLayoutPanel _tableLayouOuter;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnReject;
        private System.Windows.Forms.Button btnSuspend;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.CheckBox _protocolNextItem;
        private System.Windows.Forms.Button btnSkip;
        private System.Windows.Forms.GroupBox _grpProcedures;
        private ClearCanvas.Desktop.View.WinForms.TableView _procedurePlanSummary;
        private System.Windows.Forms.TableLayoutPanel _tableLayoutInner;
        private System.Windows.Forms.Panel _orderNotesPanel;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}
