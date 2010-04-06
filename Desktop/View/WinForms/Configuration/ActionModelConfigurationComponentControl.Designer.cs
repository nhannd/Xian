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
			this._actionModelTree = new ClearCanvas.Desktop.View.WinForms.Configuration.ActionModelConfigurationTreeControl();
			this.SuspendLayout();
			// 
			// _actionModelTree
			// 
			this._actionModelTree.AllowDrop = true;
			this._actionModelTree.AllowDropToIndex = true;
			this._actionModelTree.CheckBoxes = true;
			this._actionModelTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this._actionModelTree.IconColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this._actionModelTree.IconResourceSize = ClearCanvas.Desktop.IconSize.Small;
			this._actionModelTree.IconSize = new System.Drawing.Size(24, 24);
			this._actionModelTree.Location = new System.Drawing.Point(0, 0);
			this._actionModelTree.Margin = new System.Windows.Forms.Padding(2);
			this._actionModelTree.Name = "_actionModelTree";
			this._actionModelTree.ShowToolbar = true;
			this._actionModelTree.Size = new System.Drawing.Size(279, 387);
			this._actionModelTree.TabIndex = 1;
			this._actionModelTree.TreeBackColor = System.Drawing.SystemColors.Window;
			this._actionModelTree.TreeForeColor = System.Drawing.SystemColors.WindowText;
			this._actionModelTree.TreeLineColor = System.Drawing.Color.Black;
			this._actionModelTree.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this.OnBindingTreeViewItemDrag);
			this._actionModelTree.SelectionChanged += new System.EventHandler(this.OnActionModelTreeSelectionChanged);
			// 
			// ActionModelConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._actionModelTree);
			this.Name = "ActionModelConfigurationComponentControl";
			this.Size = new System.Drawing.Size(279, 387);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.Configuration.ActionModelConfigurationTreeControl _actionModelTree;
	}
}
