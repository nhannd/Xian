namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    partial class ReceiveQueueApplicationComponentControl
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
			this._receiveTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this.SuspendLayout();
			// 
			// _receiveTable
			// 
			this._receiveTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._receiveTable.Location = new System.Drawing.Point(0, 31);
			this._receiveTable.MenuModel = null;
			this._receiveTable.Name = "_receiveTable";
			this._receiveTable.ReadOnly = false;
			this._receiveTable.Selection = selection1;
			this._receiveTable.Size = new System.Drawing.Size(624, 325);
			this._receiveTable.TabIndex = 4;
			this._receiveTable.Table = null;
			this._receiveTable.ToolbarModel = null;
			this._receiveTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._receiveTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// _titleBar
			// 
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Top;
			this._titleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
			this._titleBar.Location = new System.Drawing.Point(0, 0);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(624, 31);
			this._titleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.Office2003;
			this._titleBar.TabIndex = 5;
			this._titleBar.Text = "Receive Operations";
			// 
			// ReceiveQueueApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._receiveTable);
			this.Controls.Add(this._titleBar);
			this.Name = "ReceiveQueueApplicationComponentControl";
			this.Size = new System.Drawing.Size(624, 356);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _receiveTable;
		private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;

	}
}
