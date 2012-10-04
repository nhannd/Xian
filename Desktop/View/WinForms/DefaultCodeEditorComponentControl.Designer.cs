namespace ClearCanvas.Desktop.View.WinForms
{
    partial class DefaultCodeEditorComponentControl
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
            this._editor = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // _editor
            // 
            this._editor.AcceptsTab = true;
            this._editor.AutoWordSelection = true;
            this._editor.DetectUrls = false;
            this._editor.Dock = System.Windows.Forms.DockStyle.Fill;
            this._editor.Location = new System.Drawing.Point(0, 0);
            this._editor.Name = "_editor";
            this._editor.Size = new System.Drawing.Size(310, 217);
            this._editor.TabIndex = 0;
            this._editor.Text = "";
            this._editor.WordWrap = false;
            // 
            // DefaultCodeEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._editor);
            this.Name = "DefaultCodeEditorComponentControl";
            this.Size = new System.Drawing.Size(310, 217);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox _editor;
    }
}
