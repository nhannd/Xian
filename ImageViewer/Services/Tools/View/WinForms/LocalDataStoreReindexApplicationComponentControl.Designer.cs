namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    partial class LocalDataStoreReindexApplicationComponentControl
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
			this._reindexProgressControl = new ClearCanvas.ImageViewer.Services.Tools.View.WinForms.ImportProgressControl();
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._reindexProgressControl, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this._titleBar, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(777, 204);
			this.tableLayoutPanel1.TabIndex = 19;
			// 
			// _reindexProgressControl
			// 
			this._reindexProgressControl.AutoSize = true;
			this._reindexProgressControl.AvailableCount = 0;
			this._reindexProgressControl.CancelEnabled = true;
			this._reindexProgressControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reindexProgressControl.FailedSteps = 0;
			this._reindexProgressControl.Location = new System.Drawing.Point(5, 38);
			this._reindexProgressControl.Name = "_reindexProgressControl";
			this._reindexProgressControl.Size = new System.Drawing.Size(767, 161);
			this._reindexProgressControl.StatusMessage = "Status:";
			this._reindexProgressControl.TabIndex = 9;
			this._reindexProgressControl.TotalProcessed = 0;
			this._reindexProgressControl.TotalToProcess = 100;
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
			this._titleBar.TabIndex = 7;
			this._titleBar.Text = "Reindex Local Data Store";
			// 
			// LocalDataStoreReindexApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "LocalDataStoreReindexApplicationComponentControl";
			this.Size = new System.Drawing.Size(777, 205);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;
		private ImportProgressControl _reindexProgressControl;
    }
}
