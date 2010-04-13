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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActionModelConfigurationComponentControl));
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._actionModelTree = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
			this.SuspendLayout();
			// 
			// _toolStrip
			// 
			this._toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			resources.ApplyResources(this._toolStrip, "_toolStrip");
			this._toolStrip.Name = "_toolStrip";
			// 
			// _actionModelTree
			// 
			this._actionModelTree.AllowDrop = true;
			this._actionModelTree.AllowDropToIndex = true;
			this._actionModelTree.CheckBoxStyle = ClearCanvas.Desktop.View.WinForms.CheckBoxStyle.TriState;
			resources.ApplyResources(this._actionModelTree, "_actionModelTree");
			this._actionModelTree.IconColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._actionModelTree.IconResourceSize = ClearCanvas.Desktop.IconSize.Small;
			this._actionModelTree.IconSize = new System.Drawing.Size(24, 24);
			this._actionModelTree.Name = "_actionModelTree";
			this._actionModelTree.ShowToolbar = false;
			this._actionModelTree.TreeBackColor = System.Drawing.SystemColors.Window;
			this._actionModelTree.TreeForeColor = System.Drawing.SystemColors.WindowText;
			this._actionModelTree.TreeLineColor = System.Drawing.Color.Black;
			this._actionModelTree.SelectionChanged += new System.EventHandler(this.OnActionModelTreeSelectionChanged);
			this._actionModelTree.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this.OnBindingTreeViewItemDrag);
			// 
			// ActionModelConfigurationComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._actionModelTree);
			this.Controls.Add(this._toolStrip);
			this.Name = "ActionModelConfigurationComponentControl";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.BindingTreeView _actionModelTree;
		private System.Windows.Forms.ToolStrip _toolStrip;
	}
}
