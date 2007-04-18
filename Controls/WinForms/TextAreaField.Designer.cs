namespace ClearCanvas.Controls.WinForms
{
    partial class TextAreaField
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
			this._textBox = new System.Windows.Forms.TextBox();
			this._label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _textBox
			// 
			this._textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._textBox.Location = new System.Drawing.Point(2, 16);
			this._textBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this._textBox.Multiline = true;
			this._textBox.Name = "_textBox";
			this._textBox.Size = new System.Drawing.Size(184, 105);
			this._textBox.TabIndex = 0;
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
			// TextAreaField
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._textBox);
			this.Controls.Add(this._label);
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.Name = "TextAreaField";
			this.Size = new System.Drawing.Size(188, 123);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _textBox;
        private System.Windows.Forms.Label _label;
    }
}
