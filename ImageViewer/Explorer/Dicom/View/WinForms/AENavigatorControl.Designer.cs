namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	partial class AENavigatorControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AENavigatorControl));
            this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
            this._aeTreeView = new System.Windows.Forms.TreeView();
            this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._imageList = new System.Windows.Forms.ImageList(this.components);
            this._serverTools = new System.Windows.Forms.ToolStrip();
            this.SuspendLayout();
            // 
            // titleBar1
            // 
            this._titleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this._titleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
            this._titleBar.Location = new System.Drawing.Point(0, 0);
            this._titleBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
            this._titleBar.Name = "titleBar1";
            this._titleBar.Size = new System.Drawing.Size(400, 37);
            this._titleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.Office2003;
            this._titleBar.TabIndex = 2;
            this._titleBar.Text = "Servers";
            // 
            // _aeTreeView
            // 
            this._aeTreeView.AllowDrop = true;
            this._aeTreeView.ContextMenuStrip = this._contextMenu;
            this._aeTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._aeTreeView.HideSelection = false;
            this._aeTreeView.ImageIndex = 0;
            this._aeTreeView.ImageList = this._imageList;
            this._aeTreeView.Location = new System.Drawing.Point(0, 62);
            this._aeTreeView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._aeTreeView.Name = "_aeTreeView";
            this._aeTreeView.SelectedImageIndex = 1;
            this._aeTreeView.Size = new System.Drawing.Size(400, 310);
            this._aeTreeView.StateImageList = this._imageList;
            this._aeTreeView.TabIndex = 3;
            this._aeTreeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._aeTreeView_NodeMouseDoubleClick);
            // 
            // _contextMenu
            // 
            this._contextMenu.Name = "_contextMenu";
            this._contextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // _imageList
            // 
            this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
            this._imageList.TransparentColor = System.Drawing.Color.Transparent;
            this._imageList.Images.SetKeyName(0, "MyComputer.png");
            this._imageList.Images.SetKeyName(1, "Server.png");
            this._imageList.Images.SetKeyName(2, "ServerGroup.png");
            // 
            // _serverTools
            // 
            this._serverTools.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._serverTools.Location = new System.Drawing.Point(0, 37);
            this._serverTools.Name = "_serverTools";
            this._serverTools.Size = new System.Drawing.Size(400, 25);
            this._serverTools.TabIndex = 4;
            this._serverTools.Text = "toolStrip2";
            // 
            // AENavigatorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this._aeTreeView);
            this.Controls.Add(this._serverTools);
            this.Controls.Add(this._titleBar);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "AENavigatorControl";
            this.Size = new System.Drawing.Size(400, 372);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;
        private System.Windows.Forms.TreeView _aeTreeView;
		private System.Windows.Forms.ImageList _imageList;
        private System.Windows.Forms.ToolStrip _serverTools;
        private System.Windows.Forms.ContextMenuStrip _contextMenu;


    }
}
