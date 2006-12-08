namespace ClearCanvas.Controls.WinForms
{
    partial class MaskedTextField
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
            this._label = new System.Windows.Forms.Label();
            this._nullableMaskedEdit = new Clifton.Windows.Forms.NullableMaskedEdit();
            this.SuspendLayout();
            // 
            // _label
            // 
            this._label.AutoSize = true;
            this._label.Location = new System.Drawing.Point(2, 0);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(29, 13);
            this._label.TabIndex = 0;
            this._label.Text = "label";
            // 
            // _nullableMaskedEdit
            // 
            this._nullableMaskedEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._nullableMaskedEdit.AutoAdvance = true;
            this._nullableMaskedEdit.EditMask = "";
            this._nullableMaskedEdit.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
            this._nullableMaskedEdit.Location = new System.Drawing.Point(3, 18);
            this._nullableMaskedEdit.Name = "_nullableMaskedEdit";
            this._nullableMaskedEdit.NullTextDisplayValue = null;
            this._nullableMaskedEdit.NullTextReturnValue = null;
            this._nullableMaskedEdit.NullValue = null;
            this._nullableMaskedEdit.SelectGroup = true;
            this._nullableMaskedEdit.Size = new System.Drawing.Size(145, 20);
            this._nullableMaskedEdit.SkipLiterals = false;
            this._nullableMaskedEdit.TabIndex = 1;
            this._nullableMaskedEdit.Text = null;
            this._nullableMaskedEdit.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this._nullableMaskedEdit.Value = null;
            // 
            // MaskedTextField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._nullableMaskedEdit);
            this.Controls.Add(this._label);
            this.Name = "MaskedTextField";
            this.Size = new System.Drawing.Size(150, 41);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _label;
        private Clifton.Windows.Forms.NullableMaskedEdit _nullableMaskedEdit;
    }
}
