namespace ClearCanvas.Desktop.View.WinForms
{
	partial class RichTextField
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
			this._richTextBox = new System.Windows.Forms.RichTextBox();
			this._label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _richTextBox
			// 
			this._richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._richTextBox.Location = new System.Drawing.Point(2, 16);
			this._richTextBox.Margin = new System.Windows.Forms.Padding(2);
			this._richTextBox.Name = "_richTextBox";
			this._richTextBox.Size = new System.Drawing.Size(184, 105);
			this._richTextBox.TabIndex = 0;
			this._richTextBox.Text = "";
			// 
			// _label
			// 
			this._label.AutoSize = true;
			this._label.Location = new System.Drawing.Point(2, 0);
			this._label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this._label.Name = "_label";
			this._label.Size = new System.Drawing.Size(29, 13);
			this._label.TabIndex = 1;
			this._label.Text = "label";
			// 
			// RichTextField
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._label);
			this.Controls.Add(this._richTextBox);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "RichTextField";
			this.Size = new System.Drawing.Size(188, 123);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this._richTextBox_DragDrop);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this._richTextBox_DragEnter);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RichTextBox _richTextBox;
		private System.Windows.Forms.Label _label;
	}
}
