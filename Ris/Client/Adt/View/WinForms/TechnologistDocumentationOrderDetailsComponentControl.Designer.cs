namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class TechnologistDocumentationOrderDetailsComponentControl
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
            this._protocolPanel = new System.Windows.Forms.Panel();
            this._notesPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this._protocolPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._notesPanel, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(521, 389);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _protocolPanel
            // 
            this._protocolPanel.AutoSize = true;
            this._protocolPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._protocolPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._protocolPanel.Location = new System.Drawing.Point(3, 3);
            this._protocolPanel.Name = "_protocolPanel";
            this._protocolPanel.Size = new System.Drawing.Size(515, 188);
            this._protocolPanel.TabIndex = 0;
            // 
            // _notesPanel
            // 
            this._notesPanel.AutoSize = true;
            this._notesPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._notesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._notesPanel.Location = new System.Drawing.Point(3, 197);
            this._notesPanel.Name = "_notesPanel";
            this._notesPanel.Size = new System.Drawing.Size(515, 189);
            this._notesPanel.TabIndex = 1;
            // 
            // TechnologistDocumentationOrderDetailsComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "TechnologistDocumentationOrderDetailsComponentControl";
            this.Size = new System.Drawing.Size(521, 389);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel _protocolPanel;
        private System.Windows.Forms.Panel _notesPanel;
    }
}
