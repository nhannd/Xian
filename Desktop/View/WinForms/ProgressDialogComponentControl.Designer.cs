namespace ClearCanvas.Desktop.View.WinForms
{
    partial class ProgressDialogComponentControl
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
            this._message = new System.Windows.Forms.TextBox();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _message
            // 
            this._message.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._message.Location = new System.Drawing.Point(3, 3);
            this._message.Multiline = true;
            this._message.Name = "_message";
            this._message.ReadOnly = true;
            this._message.Size = new System.Drawing.Size(304, 58);
            this._message.TabIndex = 0;
            this._message.Enter += new System.EventHandler(this._message_Enter);
            // 
            // _progressBar
            // 
            this._progressBar.Location = new System.Drawing.Point(3, 67);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(222, 23);
            this._progressBar.TabIndex = 1;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(232, 67);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // ProgressDialogComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._progressBar);
            this.Controls.Add(this._message);
            this.Name = "ProgressDialogComponentControl";
            this.Size = new System.Drawing.Size(313, 97);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _message;
        private System.Windows.Forms.ProgressBar _progressBar;
        private System.Windows.Forms.Button _cancelButton;
    }
}
