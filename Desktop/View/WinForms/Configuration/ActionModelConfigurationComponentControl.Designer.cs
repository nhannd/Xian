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
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this._actionModelTree = new ClearCanvas.Desktop.View.WinForms.BindingTreeView();
			this.SuspendLayout();
			// 
			// _toolStrip
			// 
			this._toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._toolStrip.Location = new System.Drawing.Point(0, 0);
			this._toolStrip.Name = "_toolStrip";
			this._toolStrip.Size = new System.Drawing.Size(492, 25);
			this._toolStrip.TabIndex = 3;
			this._toolStrip.Text = "toolStrip1";
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
			this._actionModelTree.Location = new System.Drawing.Point(0, 25);
			this._actionModelTree.Margin = new System.Windows.Forms.Padding(2);
			this._actionModelTree.Name = "_actionModelTree";
			this._actionModelTree.ShowToolbar = false;
			this._actionModelTree.Size = new System.Drawing.Size(492, 362);
			this._actionModelTree.TabIndex = 1;
			this._actionModelTree.TreeBackColor = System.Drawing.SystemColors.Window;
			this._actionModelTree.TreeForeColor = System.Drawing.SystemColors.WindowText;
			this._actionModelTree.TreeLineColor = System.Drawing.Color.Black;
			this._actionModelTree.SelectionChanged += new System.EventHandler(this.OnActionModelTreeSelectionChanged);
			this._actionModelTree.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this.OnBindingTreeViewItemDrag);
			// 
			// ActionModelConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._actionModelTree);
			this.Controls.Add(this._toolStrip);
			this.Name = "ActionModelConfigurationComponentControl";
			this.Size = new System.Drawing.Size(492, 387);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.BindingTreeView _actionModelTree;
		private System.Windows.Forms.ToolStrip _toolStrip;
	}
}
