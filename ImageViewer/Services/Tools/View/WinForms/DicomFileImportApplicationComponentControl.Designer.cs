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
			ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
			this._importTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._importProgressControl = new ClearCanvas.ImageViewer.Services.Tools.View.WinForms.ImportProgressControl();
			this.SuspendLayout();
			// 
			// _importTable
			// 
			this._importTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._importTable.Location = new System.Drawing.Point(0, 0);
			this._importTable.MenuModel = null;
			this._importTable.MultiSelect = false;
			this._importTable.Name = "_importTable";
			this._importTable.ReadOnly = false;
			this._importTable.Selection = selection1;
			this._importTable.Size = new System.Drawing.Size(776, 228);
			this._importTable.TabIndex = 0;
			this._importTable.Table = null;
			this._importTable.ToolbarModel = null;
			this._importTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._importTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// _importProgressControl
			// 
			this._importProgressControl.AcceptButton = null;
			this._importProgressControl.AvailableCount = 0;
			this._importProgressControl.CancelButton = null;
			this._importProgressControl.CancelEnabled = true;
			this._importProgressControl.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._importProgressControl.FailedSteps = 0;
			this._importProgressControl.Location = new System.Drawing.Point(0, 228);
			this._importProgressControl.Name = "_importProgressControl";
			this._importProgressControl.Size = new System.Drawing.Size(776, 165);
			this._importProgressControl.StatusMessage = "Status:";
			this._importProgressControl.TabIndex = 1;
			this._importProgressControl.TotalProcessed = 0;
			this._importProgressControl.TotalToProcess = 100;
			// 
			// DicomFileImportApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._importTable);
			this.Controls.Add(this._importProgressControl);
			this.Name = "DicomFileImportApplicationComponentControl";
			this.Size = new System.Drawing.Size(776, 393);
			this.ResumeLayout(false);

        }

        #endregion

		private ImportProgressControl _importProgressControl;
		private ClearCanvas.Desktop.View.WinForms.TableView _importTable;



	}
}
