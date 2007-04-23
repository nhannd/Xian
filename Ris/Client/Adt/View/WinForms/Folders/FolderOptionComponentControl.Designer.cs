namespace ClearCanvas.Ris.Client.Adt.Folders.View.WinForms
{
    partial class FolderOptionComponentControl
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
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this._refreshTime = new ClearCanvas.Controls.WinForms.TextField();
            this.SuspendLayout();
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(85, 50);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 0;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(4, 50);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 1;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _refreshTime
            // 
            this._refreshTime.LabelText = "Refresh Time (ms)";
            this._refreshTime.Location = new System.Drawing.Point(4, 4);
            this._refreshTime.Margin = new System.Windows.Forms.Padding(2);
            this._refreshTime.Mask = "";
            this._refreshTime.Name = "_refreshTime";
            this._refreshTime.Size = new System.Drawing.Size(150, 41);
            this._refreshTime.TabIndex = 2;
            this._refreshTime.Value = null;
            // 
            // FolderOptionComponentControl
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this._refreshTime);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.Name = "FolderOptionComponentControl";
            this.Size = new System.Drawing.Size(169, 82);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
        private ClearCanvas.Controls.WinForms.TextField _refreshTime;
    }
}
