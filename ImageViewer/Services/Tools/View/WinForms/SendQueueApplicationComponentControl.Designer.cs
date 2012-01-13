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
			DoDispose(disposing);
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SendQueueApplicationComponentControl));
			this._sendTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._titleBar = new ClearCanvas.Desktop.View.WinForms.TitleBar();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._statusBar = new System.Windows.Forms.StatusStrip();
			this._fillerStatusItem = new System.Windows.Forms.ToolStripStatusLabel();
			this._failuresStatusItem = new System.Windows.Forms.ToolStripStatusLabel();
			this._statusBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// _sendTable
			// 
			this._sendTable.ColumnHeaderTooltip = null;
			resources.ApplyResources(this._sendTable, "_sendTable");
			this._sendTable.Name = "_sendTable";
			this._sendTable.ReadOnly = false;
			this._sendTable.SortButtonTooltip = null;
			// 
			// _titleBar
			// 
			resources.ApplyResources(this._titleBar, "_titleBar");
			this._titleBar.Name = "_titleBar";
			// 
			// _toolTip
			// 
			this._toolTip.IsBalloon = true;
			// 
			// _statusBar
			// 
			this._statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._fillerStatusItem,
            this._failuresStatusItem});
			resources.ApplyResources(this._statusBar, "_statusBar");
			this._statusBar.Name = "_statusBar";
			this._statusBar.SizingGrip = false;
			// 
			// _fillerStatusItem
			// 
			resources.ApplyResources(this._fillerStatusItem, "_fillerStatusItem");
			this._fillerStatusItem.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this._fillerStatusItem.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this._fillerStatusItem.Name = "_fillerStatusItem";
			this._fillerStatusItem.Spring = true;
			// 
			// _failuresStatusItem
			// 
			this._failuresStatusItem.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this._failuresStatusItem.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this._failuresStatusItem.Name = "_failuresStatusItem";
			resources.ApplyResources(this._failuresStatusItem, "_failuresStatusItem");
			// 
			// SendQueueApplicationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._sendTable);
			this.Controls.Add(this._statusBar);
			this.Controls.Add(this._titleBar);
			this.Name = "SendQueueApplicationComponentControl";
			this._statusBar.ResumeLayout(false);
			this._statusBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _sendTable;
		private ClearCanvas.Desktop.View.WinForms.TitleBar _titleBar;
		private System.Windows.Forms.ToolTip _toolTip;
		private System.Windows.Forms.StatusStrip _statusBar;
		private System.Windows.Forms.ToolStripStatusLabel _fillerStatusItem;
		private System.Windows.Forms.ToolStripStatusLabel _failuresStatusItem;

	}
}
