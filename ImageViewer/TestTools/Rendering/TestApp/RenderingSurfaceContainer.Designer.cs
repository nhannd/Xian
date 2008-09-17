namespace ClearCanvas.ImageViewer.TestTools.Rendering.TestApp
{
	partial class RenderingSurfaceContainer
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
			this._splitContainer = new System.Windows.Forms.SplitContainer();
			this._renderingSurface = new RenderingSurface();
			this._customBackBuffer = new System.Windows.Forms.CheckBox();
			this._comboSource = new System.Windows.Forms.ComboBox();
			this._comboFormat = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._useBufferedGraphics = new System.Windows.Forms.CheckBox();
			this._splitContainer.Panel1.SuspendLayout();
			this._splitContainer.Panel2.SuspendLayout();
			this._splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// _splitContainer
			// 
			this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this._splitContainer.Location = new System.Drawing.Point(0, 0);
			this._splitContainer.Name = "_splitContainer";
			this._splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.Controls.Add(this._renderingSurface);
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.Controls.Add(this._useBufferedGraphics);
			this._splitContainer.Panel2.Controls.Add(this._customBackBuffer);
			this._splitContainer.Panel2.Controls.Add(this._comboSource);
			this._splitContainer.Panel2.Controls.Add(this._comboFormat);
			this._splitContainer.Panel2.Controls.Add(this.label2);
			this._splitContainer.Panel2.Controls.Add(this.label1);
			this._splitContainer.Size = new System.Drawing.Size(549, 359);
			this._splitContainer.SplitterDistance = 254;
			this._splitContainer.TabIndex = 0;
			// 
			// _renderingSurface
			// 
			this._renderingSurface.CustomBackBuffer = true;
			this._renderingSurface.Dock = System.Windows.Forms.DockStyle.Fill;
			this._renderingSurface.Format = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
			this._renderingSurface.Location = new System.Drawing.Point(0, 0);
			this._renderingSurface.Name = "_renderingSurface";
			this._renderingSurface.Size = new System.Drawing.Size(549, 254);
			this._renderingSurface.Source = GraphicsSource.Default;
			this._renderingSurface.TabIndex = 0;
			// 
			// _customBackBuffer
			// 
			this._customBackBuffer.AutoSize = true;
			this._customBackBuffer.Location = new System.Drawing.Point(226, 21);
			this._customBackBuffer.Name = "_customBackBuffer";
			this._customBackBuffer.Size = new System.Drawing.Size(120, 17);
			this._customBackBuffer.TabIndex = 4;
			this._customBackBuffer.Text = "Custom Back Buffer";
			this._customBackBuffer.UseVisualStyleBackColor = true;
			// 
			// _comboSource
			// 
			this._comboSource.FormattingEnabled = true;
			this._comboSource.Location = new System.Drawing.Point(55, 53);
			this._comboSource.Name = "_comboSource";
			this._comboSource.Size = new System.Drawing.Size(121, 21);
			this._comboSource.TabIndex = 3;
			// 
			// _comboFormat
			// 
			this._comboFormat.FormattingEnabled = true;
			this._comboFormat.Location = new System.Drawing.Point(55, 18);
			this._comboFormat.Name = "_comboFormat";
			this._comboFormat.Size = new System.Drawing.Size(121, 21);
			this._comboFormat.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(7, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Source:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Format:";
			// 
			// _useBufferedGraphics
			// 
			this._useBufferedGraphics.AutoSize = true;
			this._useBufferedGraphics.Location = new System.Drawing.Point(226, 55);
			this._useBufferedGraphics.Name = "_useBufferedGraphics";
			this._useBufferedGraphics.Size = new System.Drawing.Size(130, 17);
			this._useBufferedGraphics.TabIndex = 5;
			this._useBufferedGraphics.Text = "Use BufferedGraphics";
			this._useBufferedGraphics.UseVisualStyleBackColor = true;
			// 
			// RenderingSurfaceContainer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._splitContainer);
			this.Name = "RenderingSurfaceContainer";
			this.Size = new System.Drawing.Size(549, 359);
			this._splitContainer.Panel1.ResumeLayout(false);
			this._splitContainer.Panel2.ResumeLayout(false);
			this._splitContainer.Panel2.PerformLayout();
			this._splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer _splitContainer;
		private RenderingSurface _renderingSurface;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox _comboFormat;
		private System.Windows.Forms.ComboBox _comboSource;
		private System.Windows.Forms.CheckBox _customBackBuffer;
		private System.Windows.Forms.CheckBox _useBufferedGraphics;

	}
}
