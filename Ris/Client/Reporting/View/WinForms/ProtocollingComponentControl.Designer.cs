namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
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
            this._protocolEditorPanel = new System.Windows.Forms.Panel();
            this._additionalDetailsPanel = new System.Windows.Forms.Panel();
            this._additionalDetailsTabControl = new System.Windows.Forms.TabControl();
            this._priorReportsTabPage = new System.Windows.Forms.TabPage();
            this._orderSummaryPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this._additionalDetailsPanel.SuspendLayout();
            this._additionalDetailsTabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._orderSummaryPanel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1128, 729);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 103);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._protocolEditorPanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._additionalDetailsPanel);
            this.splitContainer1.Size = new System.Drawing.Size(1122, 623);
            this.splitContainer1.SplitterDistance = 562;
            this.splitContainer1.TabIndex = 0;
            // 
            // _protocolEditorPanel
            // 
            this._protocolEditorPanel.AutoSize = true;
            this._protocolEditorPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._protocolEditorPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._protocolEditorPanel.Location = new System.Drawing.Point(0, 0);
            this._protocolEditorPanel.Name = "_protocolEditorPanel";
            this._protocolEditorPanel.Size = new System.Drawing.Size(562, 623);
            this._protocolEditorPanel.TabIndex = 0;
            // 
            // _additionalDetailsPanel
            // 
            this._additionalDetailsPanel.AutoSize = true;
            this._additionalDetailsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._additionalDetailsPanel.Controls.Add(this._additionalDetailsTabControl);
            this._additionalDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._additionalDetailsPanel.Location = new System.Drawing.Point(0, 0);
            this._additionalDetailsPanel.Name = "_additionalDetailsPanel";
            this._additionalDetailsPanel.Size = new System.Drawing.Size(556, 623);
            this._additionalDetailsPanel.TabIndex = 0;
            // 
            // _additionalDetailsTabControl
            // 
            this._additionalDetailsTabControl.Controls.Add(this._priorReportsTabPage);
            this._additionalDetailsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._additionalDetailsTabControl.Location = new System.Drawing.Point(0, 0);
            this._additionalDetailsTabControl.Name = "_additionalDetailsTabControl";
            this._additionalDetailsTabControl.SelectedIndex = 0;
            this._additionalDetailsTabControl.Size = new System.Drawing.Size(556, 623);
            this._additionalDetailsTabControl.TabIndex = 0;
            // 
            // _priorReportsTabPage
            // 
            this._priorReportsTabPage.Location = new System.Drawing.Point(4, 22);
            this._priorReportsTabPage.Name = "_priorReportsTabPage";
            this._priorReportsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._priorReportsTabPage.Size = new System.Drawing.Size(548, 597);
            this._priorReportsTabPage.TabIndex = 0;
            this._priorReportsTabPage.Text = "Prior Reports";
            this._priorReportsTabPage.UseVisualStyleBackColor = true;
            // 
            // _orderSummaryPanel
            // 
            this._orderSummaryPanel.AutoSize = true;
            this._orderSummaryPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._orderSummaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._orderSummaryPanel.Location = new System.Drawing.Point(3, 3);
            this._orderSummaryPanel.Name = "_orderSummaryPanel";
            this._orderSummaryPanel.Size = new System.Drawing.Size(1122, 94);
            this._orderSummaryPanel.TabIndex = 1;
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
            this._additionalDetailsPanel.ResumeLayout(false);
            this._additionalDetailsTabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel _protocolEditorPanel;
        private System.Windows.Forms.Panel _additionalDetailsPanel;
        private System.Windows.Forms.TabControl _additionalDetailsTabControl;
        private System.Windows.Forms.TabPage _priorReportsTabPage;
        private System.Windows.Forms.Panel _orderSummaryPanel;
    }
}
