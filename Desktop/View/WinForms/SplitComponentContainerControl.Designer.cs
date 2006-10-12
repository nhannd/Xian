namespace ClearCanvas.Desktop.View.WinForms
{
	partial class SplitComponentContainerControl
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
            this._splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // _splitContainer
            // 
            this._splitContainer.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this._splitContainer.Location = new System.Drawing.Point(0, 0);
            this._splitContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._splitContainer.Name = "_splitContainer";
            // 
            // _splitContainer.Panel1
            // 
            this._splitContainer.Panel1.BackColor = System.Drawing.SystemColors.Control;
            // 
            // _splitContainer.Panel2
            // 
            this._splitContainer.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this._splitContainer.Size = new System.Drawing.Size(993, 597);
            this._splitContainer.SplitterDistance = 248;
            this._splitContainer.SplitterWidth = 3;
            this._splitContainer.TabIndex = 0;
            this._splitContainer.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this._splitContainer_SplitterMoved);
            // 
            // SplitComponentContainerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._splitContainer);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SplitComponentContainerControl";
            this.Size = new System.Drawing.Size(993, 597);
            this.SizeChanged += new System.EventHandler(this.SplitComponentContainerControl_SizeChanged);
            this._splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.SplitContainer _splitContainer;
	}
}
