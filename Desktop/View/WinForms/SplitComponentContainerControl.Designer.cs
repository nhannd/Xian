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
			this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainer.Location = new System.Drawing.Point(0, 0);
			this._splitContainer.Name = "_splitContainer";
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(2, 2, 0, 2);
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(0, 2, 2, 2);
			this._splitContainer.Size = new System.Drawing.Size(745, 485);
			this._splitContainer.SplitterDistance = 248;
			this._splitContainer.SplitterWidth = 2;
			this._splitContainer.TabIndex = 0;
			// 
			// SplitComponentContainerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._splitContainer);
			this.Name = "SplitComponentContainerControl";
			this.Size = new System.Drawing.Size(745, 485);
			this._splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer _splitContainer;
	}
}
