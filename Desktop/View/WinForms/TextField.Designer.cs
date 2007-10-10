namespace ClearCanvas.Desktop.View.WinForms
{
    partial class TextField
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
            this._label = new System.Windows.Forms.Label();
            this._textBox = new Clifton.Windows.Forms.NullableMaskedEdit();
            this.SuspendLayout();
            // 
            // _label
            // 
            this._label.AutoSize = true;
            this._label.Location = new System.Drawing.Point(2, 0);
            this._label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(29, 13);
            this._label.TabIndex = 0;
            this._label.Text = "label";
            // 
            // _textBox
            // 
            this._textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._textBox.Location = new System.Drawing.Point(3, 18);
            this._textBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._textBox.Name = "_textBox";
            this._textBox.Size = new System.Drawing.Size(145, 20);
            this._textBox.TabIndex = 1;
            this._textBox.AutoAdvance = true;
            this._textBox.EditMask = "";
            this._textBox.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this._textBox.NullTextDisplayValue = null;
            this._textBox.NullTextReturnValue = null;
            this._textBox.NullValue = null;
            this._textBox.SelectGroup = true;
            this._textBox.SkipLiterals = false;
            this._textBox.Text = null;
            this._textBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this._textBox.Value = null;
            // 
            // TextField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._textBox);
            this.Controls.Add(this._label);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "TextField";
            this.Size = new System.Drawing.Size(150, 41);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _label;
        private Clifton.Windows.Forms.NullableMaskedEdit _textBox;
    }
}
