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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AENavigatorControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this._btnAdd = new System.Windows.Forms.ToolStripButton();
            this._btnDelete = new System.Windows.Forms.ToolStripButton();
            this.titleBar1 = new Crownwood.DotNetMagic.Controls.TitleBar();
            this._aeserverTreeForm1 = new ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms.AEServerTreeForm();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._btnAdd,
            this._btnDelete});
            this.toolStrip1.Location = new System.Drawing.Point(247, 25);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(56, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // _btnAdd
            // 
            this._btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("_btnAdd.Image")));
            this._btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._btnAdd.Name = "_btnAdd";
            this._btnAdd.Size = new System.Drawing.Size(23, 22);
            this._btnAdd.Text = "toolStripButton1";
            // 
            // _btnDelete
            // 
            this._btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("_btnDelete.Image")));
            this._btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._btnDelete.Name = "_btnDelete";
            this._btnDelete.Size = new System.Drawing.Size(23, 22);
            this._btnDelete.Text = "toolStripButton1";
            // 
            // titleBar1
            // 
            this.titleBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.titleBar1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleBar1.Location = new System.Drawing.Point(-1, -1);
            this.titleBar1.MouseOverColor = System.Drawing.Color.Empty;
            this.titleBar1.Name = "titleBar1";
            this.titleBar1.Size = new System.Drawing.Size(304, 23);
            this.titleBar1.TabIndex = 2;
            this.titleBar1.Text = "Servers";
            // 
            // _aeserverTreeForm1
            // 
            this._aeserverTreeForm1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._aeserverTreeForm1.Location = new System.Drawing.Point(2, 44);
            this._aeserverTreeForm1.Name = "_aeserverTreeForm1";
            this._aeserverTreeForm1.Size = new System.Drawing.Size(297, 365);
            this._aeserverTreeForm1.TabIndex = 0;
            // 
            // AENavigatorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.titleBar1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this._aeserverTreeForm1);
            this.Name = "AENavigatorControl";
            this.Size = new System.Drawing.Size(302, 442);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private AEServerTreeForm _aeserverTreeForm1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton _btnAdd;
        private System.Windows.Forms.ToolStripButton _btnDelete;
        private Crownwood.DotNetMagic.Controls.TitleBar titleBar1;


    }
}
