namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    partial class DicomFileImportApplicationComponentControl
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
			ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
			this._layoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._importTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._importProgressControl = new ClearCanvas.ImageViewer.Services.Tools.View.WinForms.ImportProgressControl();
			this._layoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _layoutPanel
			// 
			this._layoutPanel.ColumnCount = 1;
			this._layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutPanel.Controls.Add(this._importTable, 0, 0);
			this._layoutPanel.Controls.Add(this._importProgressControl, 0, 1);
			this._layoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._layoutPanel.Location = new System.Drawing.Point(0, 0);
			this._layoutPanel.Name = "_layoutPanel";
			this._layoutPanel.RowCount = 2;
			this._layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._layoutPanel.Size = new System.Drawing.Size(777, 385);
			this._layoutPanel.TabIndex = 0;
			// 
			// _importTable
			// 
			this._importTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._importTable.Location = new System.Drawing.Point(3, 3);
			this._importTable.MenuModel = null;
			this._importTable.MultiSelect = false;
			this._importTable.Name = "_importTable";
			this._importTable.ReadOnly = false;
			this._importTable.Selection = selection2;
			this._importTable.Size = new System.Drawing.Size(771, 178);
			this._importTable.TabIndex = 7;
			this._importTable.Table = null;
			this._importTable.ToolbarModel = null;
			this._importTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._importTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
			// 
			// _importProgressControl
			// 
			this._importProgressControl.AutoSize = true;
			this._importProgressControl.AvailableCount = 0;
			this._importProgressControl.CancelEnabled = true;
			this._importProgressControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._importProgressControl.FailedSteps = 0;
			this._importProgressControl.Location = new System.Drawing.Point(3, 187);
			this._importProgressControl.Name = "_importProgressControl";
			this._importProgressControl.Size = new System.Drawing.Size(771, 195);
			this._importProgressControl.StatusMessage = "Status:";
			this._importProgressControl.TabIndex = 8;
			this._importProgressControl.TotalProcessed = 0;
			this._importProgressControl.TotalToProcess = 100;
			// 
			// DicomFileImportApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._layoutPanel);
			this.Name = "DicomFileImportApplicationComponentControl";
			this.Size = new System.Drawing.Size(777, 385);
			this._layoutPanel.ResumeLayout(false);
			this._layoutPanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel _layoutPanel;
		private ClearCanvas.Desktop.View.WinForms.TableView _importTable;
		private ImportProgressControl _importProgressControl;
    }
}
