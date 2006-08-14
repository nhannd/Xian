namespace ClearCanvas.ImageViewer.Dashboard.Remote
{
	partial class MasterViewControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer _components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (_components != null))
			{
				_components.Dispose();
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
            this._serverTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // _serverTree
            // 
            this._serverTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this._serverTree.Location = new System.Drawing.Point(0, 0);
            this._serverTree.Margin = new System.Windows.Forms.Padding(4);
            this._serverTree.Name = "_serverTree";
            this._serverTree.Size = new System.Drawing.Size(200, 185);
            this._serverTree.TabIndex = 0;
            // 
            // MasterViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._serverTree);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MasterViewControl";
            this.Size = new System.Drawing.Size(200, 185);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView _serverTree;
	}
}
