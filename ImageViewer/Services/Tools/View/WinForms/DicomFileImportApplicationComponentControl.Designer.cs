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
			ClearCanvas.Desktop.Selection selection4 = new ClearCanvas.Desktop.Selection();
			this._layoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._importTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this._importProgressControl = new ClearCanvas.ImageViewer.Services.Tools.View.WinForms.ImportProgressControl();
			this._layoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _layoutPanel
			// 
			this._layoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
			this._layoutPanel.ColumnCount = 1;
			this._layoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._layoutPanel.Controls.Add(this._importTable, 0, 1);
			this._layoutPanel.Controls.Add(this._titleBar, 0, 0);
			this._layoutPanel.Controls.Add(this._importProgressControl, 0, 2);
			this._layoutPanel.Location = new System.Drawing.Point(0, 0);
			this._layoutPanel.Name = "_layoutPanel";
			this._layoutPanel.RowCount = 3;
			this._layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
			this._layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._layoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._layoutPanel.Size = new System.Drawing.Size(777, 380);
			this._layoutPanel.TabIndex = 0;
			// 
			// _importTable
			// 
			this._importTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._importTable.Location = new System.Drawing.Point(5, 38);
			this._importTable.MenuModel = null;
			this._importTable.MultiSelect = false;
			this._importTable.Name = "_importTable";
			this._importTable.ReadOnly = false;
			this._importTable.Selection = selection4;
			this._importTable.Size = new System.Drawing.Size(767, 178);
			this._importTable.TabIndex = 7;
			this._importTable.Table = null;
			this._importTable.ToolbarModel = null;
			this._importTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._importTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
			// 
			// _titleBar
			// 
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this._titleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
			this._titleBar.Location = new System.Drawing.Point(5, 5);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(767, 25);
			this._titleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.Office2003;
			this._titleBar.TabIndex = 6;
			this._titleBar.Text = "Import Activity";
			// 
			// _importProgressControl
			// 
			this._importProgressControl.AutoSize = true;
			this._importProgressControl.AvailableCount = 0;
			this._importProgressControl.CancelEnabled = true;
			this._importProgressControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._importProgressControl.FailedSteps = 0;
			this._importProgressControl.Location = new System.Drawing.Point(5, 224);
			this._importProgressControl.Name = "_importProgressControl";
			this._importProgressControl.Size = new System.Drawing.Size(767, 151);
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
			this.Size = new System.Drawing.Size(777, 381);
			this._layoutPanel.ResumeLayout(false);
			this._layoutPanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel _layoutPanel;
		private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;
		private ClearCanvas.Desktop.View.WinForms.TableView _importTable;
		private ImportProgressControl _importProgressControl;
    }
}
