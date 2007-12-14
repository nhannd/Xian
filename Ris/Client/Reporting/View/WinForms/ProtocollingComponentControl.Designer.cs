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
            this._orderSummaryPanel = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._protocolEditorPanel = new System.Windows.Forms.Panel();
            this._orderDetailsPanel = new System.Windows.Forms.Panel();
            this._orderSummaryPanel.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _orderSummaryPanel
            // 
            this._orderSummaryPanel.AutoSize = true;
            this._orderSummaryPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._orderSummaryPanel.ColumnCount = 1;
            this._orderSummaryPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._orderSummaryPanel.Controls.Add(this.splitContainer1, 0, 1);
            this._orderSummaryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._orderSummaryPanel.Location = new System.Drawing.Point(0, 0);
            this._orderSummaryPanel.Name = "_orderSummaryPanel";
            this._orderSummaryPanel.RowCount = 2;
            this._orderSummaryPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this._orderSummaryPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._orderSummaryPanel.Size = new System.Drawing.Size(1128, 729);
            this._orderSummaryPanel.TabIndex = 0;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 153);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._protocolEditorPanel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._orderDetailsPanel);
            this.splitContainer1.Size = new System.Drawing.Size(1122, 573);
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
            this._protocolEditorPanel.Size = new System.Drawing.Size(562, 573);
            this._protocolEditorPanel.TabIndex = 0;
            // 
            // _orderDetailsPanel
            // 
            this._orderDetailsPanel.AutoSize = true;
            this._orderDetailsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._orderDetailsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._orderDetailsPanel.Location = new System.Drawing.Point(0, 0);
            this._orderDetailsPanel.Name = "_orderDetailsPanel";
            this._orderDetailsPanel.Size = new System.Drawing.Size(556, 573);
            this._orderDetailsPanel.TabIndex = 0;
            // 
            // ProtocollingComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._orderSummaryPanel);
            this.Name = "ProtocollingComponentControl";
            this.Size = new System.Drawing.Size(1128, 729);
            this._orderSummaryPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _orderSummaryPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel _protocolEditorPanel;
        private System.Windows.Forms.Panel _orderDetailsPanel;
    }
}
