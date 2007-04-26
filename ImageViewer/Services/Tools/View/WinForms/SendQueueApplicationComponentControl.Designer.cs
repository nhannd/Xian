namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    partial class SendQueueApplicationComponentControl
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
			this._sendTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this.SuspendLayout();
			// 
			// _sendTable
			// 
			this._sendTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._sendTable.Location = new System.Drawing.Point(0, 31);
			this._sendTable.MenuModel = null;
			this._sendTable.Name = "_sendTable";
			this._sendTable.ReadOnly = false;
			this._sendTable.Selection = selection1;
			this._sendTable.Size = new System.Drawing.Size(687, 389);
			this._sendTable.TabIndex = 6;
			this._sendTable.Table = null;
			this._sendTable.ToolbarModel = null;
			this._sendTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._sendTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// _titleBar
			// 
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Top;
			this._titleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
			this._titleBar.Location = new System.Drawing.Point(0, 0);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(687, 31);
			this._titleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.Office2007Black;
			this._titleBar.TabIndex = 7;
			this._titleBar.Text = "Send Operations";
			// 
			// SendQueueApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._sendTable);
			this.Controls.Add(this._titleBar);
			this.Name = "SendQueueApplicationComponentControl";
			this.Size = new System.Drawing.Size(687, 420);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _sendTable;
		private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;

	}
}
