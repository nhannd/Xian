namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class ProtocollingComponentControl
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
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._tableLayouOuter = new System.Windows.Forms.TableLayoutPanel();
			this._protocolledProcedures = new System.Windows.Forms.Label();
			this._protocolEditorPanel = new System.Windows.Forms.Panel();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this._btnAccept = new System.Windows.Forms.Button();
			this._btnSubmitForApproval = new System.Windows.Forms.Button();
			this._btnReject = new System.Windows.Forms.Button();
			this._btnSave = new System.Windows.Forms.Button();
			this._btnSkip = new System.Windows.Forms.Button();
			this._protocolNextItem = new System.Windows.Forms.CheckBox();
			this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
			this._btnClose = new System.Windows.Forms.Button();
			this._additionalDetailsPanel = new System.Windows.Forms.Panel();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this._orderNotesGroupBox = new System.Windows.Forms.GroupBox();
			this._orderNotesPanel = new System.Windows.Forms.Panel();
			this._additionalDetailsTabControl = new System.Windows.Forms.TabControl();
			this._orderDetailsTabPage = new System.Windows.Forms.TabPage();
			this._priorReportsTabPage = new System.Windows.Forms.TabPage();
			this._orderSummaryPanel = new System.Windows.Forms.Panel();
			this._statusText = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this._tableLayouOuter.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			this.flowLayoutPanel3.SuspendLayout();
			this._additionalDetailsPanel.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this._orderNotesGroupBox.SuspendLayout();
			this._additionalDetailsTabControl.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._orderSummaryPanel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._statusText, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 85F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1128, 729);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 113);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._tableLayouOuter);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._additionalDetailsPanel);
			this.splitContainer1.Size = new System.Drawing.Size(1122, 613);
			this.splitContainer1.SplitterDistance = 562;
			this.splitContainer1.TabIndex = 0;
			// 
			// _tableLayouOuter
			// 
			this._tableLayouOuter.AutoSize = true;
			this._tableLayouOuter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableLayouOuter.ColumnCount = 2;
			this._tableLayouOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayouOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayouOuter.Controls.Add(this._protocolledProcedures, 0, 0);
			this._tableLayouOuter.Controls.Add(this._protocolEditorPanel, 0, 1);
			this._tableLayouOuter.Controls.Add(this.flowLayoutPanel2, 0, 2);
			this._tableLayouOuter.Controls.Add(this.flowLayoutPanel3, 1, 2);
			this._tableLayouOuter.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayouOuter.Location = new System.Drawing.Point(0, 0);
			this._tableLayouOuter.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayouOuter.Name = "_tableLayouOuter";
			this._tableLayouOuter.RowCount = 3;
			this._tableLayouOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayouOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayouOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayouOuter.Size = new System.Drawing.Size(562, 613);
			this._tableLayouOuter.TabIndex = 1;
			// 
			// _protocolledProcedures
			// 
			this._protocolledProcedures.AutoSize = true;
			this._protocolledProcedures.BackColor = System.Drawing.SystemColors.Control;
			this._tableLayouOuter.SetColumnSpan(this._protocolledProcedures, 2);
			this._protocolledProcedures.Dock = System.Windows.Forms.DockStyle.Fill;
			this._protocolledProcedures.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._protocolledProcedures.ForeColor = System.Drawing.SystemColors.ControlText;
			this._protocolledProcedures.Location = new System.Drawing.Point(3, 0);
			this._protocolledProcedures.Name = "_protocolledProcedures";
			this._protocolledProcedures.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
			this._protocolledProcedures.Size = new System.Drawing.Size(556, 19);
			this._protocolledProcedures.TabIndex = 5;
			this._protocolledProcedures.Text = "Protocolled Procedure(s): ";
			// 
			// _protocolEditorPanel
			// 
			this._protocolEditorPanel.AutoSize = true;
			this._protocolEditorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableLayouOuter.SetColumnSpan(this._protocolEditorPanel, 2);
			this._protocolEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._protocolEditorPanel.Location = new System.Drawing.Point(0, 19);
			this._protocolEditorPanel.Margin = new System.Windows.Forms.Padding(0);
			this._protocolEditorPanel.Name = "_protocolEditorPanel";
			this._protocolEditorPanel.Size = new System.Drawing.Size(562, 536);
			this._protocolEditorPanel.TabIndex = 0;
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.AutoSize = true;
			this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel2.Controls.Add(this._btnAccept);
			this.flowLayoutPanel2.Controls.Add(this._btnSubmitForApproval);
			this.flowLayoutPanel2.Controls.Add(this._btnReject);
			this.flowLayoutPanel2.Controls.Add(this._btnSave);
			this.flowLayoutPanel2.Controls.Add(this._btnSkip);
			this.flowLayoutPanel2.Controls.Add(this._protocolNextItem);
			this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 558);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new System.Drawing.Size(469, 52);
			this.flowLayoutPanel2.TabIndex = 1;
			// 
			// _btnAccept
			// 
			this._btnAccept.Location = new System.Drawing.Point(3, 3);
			this._btnAccept.Name = "_btnAccept";
			this._btnAccept.Size = new System.Drawing.Size(75, 23);
			this._btnAccept.TabIndex = 0;
			this._btnAccept.Text = "Verify";
			this._btnAccept.UseVisualStyleBackColor = true;
			this._btnAccept.Click += new System.EventHandler(this._btnAccept_Click);
			// 
			// _btnSubmitForApproval
			// 
			this._btnSubmitForApproval.Location = new System.Drawing.Point(84, 3);
			this._btnSubmitForApproval.Name = "_btnSubmitForApproval";
			this._btnSubmitForApproval.Size = new System.Drawing.Size(75, 23);
			this._btnSubmitForApproval.TabIndex = 1;
			this._btnSubmitForApproval.Text = "For Review";
			this._btnSubmitForApproval.UseVisualStyleBackColor = true;
			this._btnSubmitForApproval.Click += new System.EventHandler(this._btnSubmitForApproval_Click);
			// 
			// _btnReject
			// 
			this._btnReject.Location = new System.Drawing.Point(165, 3);
			this._btnReject.Name = "_btnReject";
			this._btnReject.Size = new System.Drawing.Size(75, 23);
			this._btnReject.TabIndex = 2;
			this._btnReject.Text = "Reject";
			this._btnReject.UseVisualStyleBackColor = true;
			this._btnReject.Click += new System.EventHandler(this._btnReject_Click);
			// 
			// _btnSave
			// 
			this._btnSave.Location = new System.Drawing.Point(246, 3);
			this._btnSave.Name = "_btnSave";
			this._btnSave.Size = new System.Drawing.Size(75, 23);
			this._btnSave.TabIndex = 4;
			this._btnSave.Text = "Save";
			this._btnSave.UseVisualStyleBackColor = true;
			this._btnSave.Click += new System.EventHandler(this._btnSave_Click);
			// 
			// _btnSkip
			// 
			this._btnSkip.Location = new System.Drawing.Point(327, 3);
			this._btnSkip.Name = "_btnSkip";
			this._btnSkip.Size = new System.Drawing.Size(75, 23);
			this._btnSkip.TabIndex = 5;
			this._btnSkip.Text = "Skip";
			this._btnSkip.UseVisualStyleBackColor = true;
			this._btnSkip.Click += new System.EventHandler(this._btnSkip_Click);
			// 
			// _protocolNextItem
			// 
			this._protocolNextItem.AutoSize = true;
			this._protocolNextItem.Dock = System.Windows.Forms.DockStyle.Fill;
			this._protocolNextItem.Location = new System.Drawing.Point(3, 32);
			this._protocolNextItem.Name = "_protocolNextItem";
			this._protocolNextItem.Size = new System.Drawing.Size(119, 17);
			this._protocolNextItem.TabIndex = 6;
			this._protocolNextItem.Text = "Go To Next Item";
			this._protocolNextItem.UseVisualStyleBackColor = true;
			// 
			// flowLayoutPanel3
			// 
			this.flowLayoutPanel3.AutoSize = true;
			this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flowLayoutPanel3.Controls.Add(this._btnClose);
			this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel3.Location = new System.Drawing.Point(478, 558);
			this.flowLayoutPanel3.Name = "flowLayoutPanel3";
			this.flowLayoutPanel3.Size = new System.Drawing.Size(81, 52);
			this.flowLayoutPanel3.TabIndex = 2;
			// 
			// _btnClose
			// 
			this._btnClose.Location = new System.Drawing.Point(3, 3);
			this._btnClose.Name = "_btnClose";
			this._btnClose.Size = new System.Drawing.Size(75, 23);
			this._btnClose.TabIndex = 0;
			this._btnClose.Text = "Close";
			this._btnClose.UseVisualStyleBackColor = true;
			this._btnClose.Click += new System.EventHandler(this._btnClose_Click);
			// 
			// _additionalDetailsPanel
			// 
			this._additionalDetailsPanel.AutoSize = true;
			this._additionalDetailsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._additionalDetailsPanel.Controls.Add(this.splitContainer2);
			this._additionalDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._additionalDetailsPanel.Location = new System.Drawing.Point(0, 0);
			this._additionalDetailsPanel.Name = "_additionalDetailsPanel";
			this._additionalDetailsPanel.Size = new System.Drawing.Size(556, 613);
			this._additionalDetailsPanel.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this._orderNotesGroupBox);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this._additionalDetailsTabControl);
			this.splitContainer2.Size = new System.Drawing.Size(556, 613);
			this.splitContainer2.SplitterDistance = 200;
			this.splitContainer2.TabIndex = 1;
			// 
			// _orderNotesGroupBox
			// 
			this._orderNotesGroupBox.Controls.Add(this._orderNotesPanel);
			this._orderNotesGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderNotesGroupBox.Location = new System.Drawing.Point(0, 0);
			this._orderNotesGroupBox.Name = "_orderNotesGroupBox";
			this._orderNotesGroupBox.Size = new System.Drawing.Size(556, 200);
			this._orderNotesGroupBox.TabIndex = 0;
			this._orderNotesGroupBox.TabStop = false;
			this._orderNotesGroupBox.Text = "Order Notes";
			// 
			// _orderNotesPanel
			// 
			this._orderNotesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderNotesPanel.Location = new System.Drawing.Point(3, 16);
			this._orderNotesPanel.Name = "_orderNotesPanel";
			this._orderNotesPanel.Size = new System.Drawing.Size(550, 181);
			this._orderNotesPanel.TabIndex = 0;
			// 
			// _additionalDetailsTabControl
			// 
			this._additionalDetailsTabControl.Controls.Add(this._orderDetailsTabPage);
			this._additionalDetailsTabControl.Controls.Add(this._priorReportsTabPage);
			this._additionalDetailsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._additionalDetailsTabControl.Location = new System.Drawing.Point(0, 0);
			this._additionalDetailsTabControl.Name = "_additionalDetailsTabControl";
			this._additionalDetailsTabControl.SelectedIndex = 0;
			this._additionalDetailsTabControl.Size = new System.Drawing.Size(556, 409);
			this._additionalDetailsTabControl.TabIndex = 0;
			// 
			// _orderDetailsTabPage
			// 
			this._orderDetailsTabPage.Location = new System.Drawing.Point(4, 22);
			this._orderDetailsTabPage.Name = "_orderDetailsTabPage";
			this._orderDetailsTabPage.Padding = new System.Windows.Forms.Padding(3);
			this._orderDetailsTabPage.Size = new System.Drawing.Size(548, 383);
			this._orderDetailsTabPage.TabIndex = 1;
			this._orderDetailsTabPage.Text = "Order Details";
			this._orderDetailsTabPage.UseVisualStyleBackColor = true;
			// 
			// _priorReportsTabPage
			// 
			this._priorReportsTabPage.Location = new System.Drawing.Point(4, 22);
			this._priorReportsTabPage.Name = "_priorReportsTabPage";
			this._priorReportsTabPage.Padding = new System.Windows.Forms.Padding(3);
			this._priorReportsTabPage.Size = new System.Drawing.Size(548, 383);
			this._priorReportsTabPage.TabIndex = 0;
			this._priorReportsTabPage.Text = "Prior Reports";
			this._priorReportsTabPage.UseVisualStyleBackColor = true;
			// 
			// _orderSummaryPanel
			// 
			this._orderSummaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderSummaryPanel.Location = new System.Drawing.Point(0, 0);
			this._orderSummaryPanel.Margin = new System.Windows.Forms.Padding(0);
			this._orderSummaryPanel.Name = "_orderSummaryPanel";
			this._orderSummaryPanel.Size = new System.Drawing.Size(1128, 85);
			this._orderSummaryPanel.TabIndex = 0;
			// 
			// _statusText
			// 
			this._statusText.AutoSize = true;
			this._statusText.BackColor = System.Drawing.Color.LightSteelBlue;
			this._statusText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._statusText.Dock = System.Windows.Forms.DockStyle.Fill;
			this._statusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._statusText.ForeColor = System.Drawing.SystemColors.ControlText;
			this._statusText.Location = new System.Drawing.Point(3, 88);
			this._statusText.Margin = new System.Windows.Forms.Padding(3);
			this._statusText.Name = "_statusText";
			this._statusText.Padding = new System.Windows.Forms.Padding(3, 3, 3, 1);
			this._statusText.Size = new System.Drawing.Size(1122, 19);
			this._statusText.TabIndex = 1;
			this._statusText.Text = "Protocolling from X worklist - Y items available - Z items completed";
			// 
			// ProtocollingComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ProtocollingComponentControl";
			this.Size = new System.Drawing.Size(1128, 729);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this._tableLayouOuter.ResumeLayout(false);
			this._tableLayouOuter.PerformLayout();
			this.flowLayoutPanel2.ResumeLayout(false);
			this.flowLayoutPanel2.PerformLayout();
			this.flowLayoutPanel3.ResumeLayout(false);
			this._additionalDetailsPanel.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this._orderNotesGroupBox.ResumeLayout(false);
			this._additionalDetailsTabControl.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel _protocolEditorPanel;
        private System.Windows.Forms.Panel _additionalDetailsPanel;
        private System.Windows.Forms.TabControl _additionalDetailsTabControl;
        private System.Windows.Forms.TabPage _priorReportsTabPage;
        private System.Windows.Forms.Panel _orderSummaryPanel;
        private System.Windows.Forms.TabPage _orderDetailsTabPage;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox _orderNotesGroupBox;
        private System.Windows.Forms.Panel _orderNotesPanel;
		private System.Windows.Forms.Label _statusText;
		private System.Windows.Forms.TableLayoutPanel _tableLayouOuter;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
		private System.Windows.Forms.Button _btnAccept;
		private System.Windows.Forms.Button _btnSubmitForApproval;
		private System.Windows.Forms.Button _btnReject;
		private System.Windows.Forms.Button _btnSave;
		private System.Windows.Forms.Button _btnSkip;
		private System.Windows.Forms.CheckBox _protocolNextItem;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
		private System.Windows.Forms.Button _btnClose;
		private System.Windows.Forms.Label _protocolledProcedures;
    }
}
