namespace ClearCanvas.Desktop.View.WinForms.Configuration {
	partial class ActionModelConfigurationComponentControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this._actionModelTree = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
			this._unavailableActionsTree = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
			this._pnlSplitMain = new System.Windows.Forms.SplitContainer();
			this._pnlSplitMain.Panel1.SuspendLayout();
			this._pnlSplitMain.Panel2.SuspendLayout();
			this._pnlSplitMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// _actionModelTree
			// 
			this._actionModelTree.AllowDrop = true;
			this._actionModelTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this._actionModelTree.IconColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._actionModelTree.IconResourceSize = ClearCanvas.Desktop.IconSize.Small;
			this._actionModelTree.IconSize = new System.Drawing.Size(24, 24);
			this._actionModelTree.Location = new System.Drawing.Point(0, 0);
			this._actionModelTree.Margin = new System.Windows.Forms.Padding(2);
			this._actionModelTree.Name = "_actionModelTree";
			this._actionModelTree.ShowToolbar = false;
			this._actionModelTree.Size = new System.Drawing.Size(209, 410);
			this._actionModelTree.TabIndex = 1;
			this._actionModelTree.TreeBackColor = System.Drawing.SystemColors.Window;
			this._actionModelTree.TreeForeColor = System.Drawing.SystemColors.WindowText;
			this._actionModelTree.TreeLineColor = System.Drawing.Color.Black;
			this._actionModelTree.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this.OnBindingTreeViewItemDrag);
			// 
			// _unavailableActionsTree
			// 
			this._unavailableActionsTree.AllowDrop = true;
			this._unavailableActionsTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this._unavailableActionsTree.IconColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._unavailableActionsTree.IconResourceSize = ClearCanvas.Desktop.IconSize.Small;
			this._unavailableActionsTree.IconSize = new System.Drawing.Size(24, 24);
			this._unavailableActionsTree.Location = new System.Drawing.Point(0, 0);
			this._unavailableActionsTree.Margin = new System.Windows.Forms.Padding(2);
			this._unavailableActionsTree.Name = "_unavailableActionsTree";
			this._unavailableActionsTree.SearchTextBoxWidth = 250;
			this._unavailableActionsTree.ShowToolbar = false;
			this._unavailableActionsTree.Size = new System.Drawing.Size(207, 410);
			this._unavailableActionsTree.TabIndex = 2;
			this._unavailableActionsTree.TreeBackColor = System.Drawing.SystemColors.Window;
			this._unavailableActionsTree.TreeForeColor = System.Drawing.SystemColors.WindowText;
			this._unavailableActionsTree.TreeLineColor = System.Drawing.Color.Black;
			this._unavailableActionsTree.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this.OnBindingTreeViewItemDrag);
			// 
			// _pnlSplitMain
			// 
			this._pnlSplitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this._pnlSplitMain.Location = new System.Drawing.Point(0, 0);
			this._pnlSplitMain.Name = "_pnlSplitMain";
			// 
			// _pnlSplitMain.Panel1
			// 
			this._pnlSplitMain.Panel1.Controls.Add(this._actionModelTree);
			// 
			// _pnlSplitMain.Panel2
			// 
			this._pnlSplitMain.Panel2.Controls.Add(this._unavailableActionsTree);
			this._pnlSplitMain.Size = new System.Drawing.Size(420, 410);
			this._pnlSplitMain.SplitterDistance = 209;
			this._pnlSplitMain.TabIndex = 3;
			// 
			// ActionModelConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._pnlSplitMain);
			this.Name = "ActionModelConfigurationComponentControl";
			this.Size = new System.Drawing.Size(420, 410);
			this._pnlSplitMain.Panel1.ResumeLayout(false);
			this._pnlSplitMain.Panel2.ResumeLayout(false);
			this._pnlSplitMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.BindingTreeView _actionModelTree;
		private ClearCanvas.Desktop.View.WinForms.BindingTreeView _unavailableActionsTree;
		private System.Windows.Forms.SplitContainer _pnlSplitMain;
	}
}
