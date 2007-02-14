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
            this._startTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._endTime = new ClearCanvas.Controls.WinForms.DateTimeField();
            this.SuspendLayout();
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(128, 116);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 0;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(47, 116);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 1;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _startTime
            // 
            this._startTime.LabelText = "Start Time:";
            this._startTime.Location = new System.Drawing.Point(12, 11);
            this._startTime.Margin = new System.Windows.Forms.Padding(2);
            this._startTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._startTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._startTime.Name = "_startTime";
            this._startTime.Nullable = true;
            this._startTime.ShowTime = false;
            this._startTime.Size = new System.Drawing.Size(150, 41);
            this._startTime.TabIndex = 2;
            this._startTime.Value = null;
            // 
            // _endTime
            // 
            this._endTime.LabelText = "End Time:";
            this._endTime.Location = new System.Drawing.Point(12, 56);
            this._endTime.Margin = new System.Windows.Forms.Padding(2);
            this._endTime.Maximum = new System.DateTime(9998, 12, 31, 0, 0, 0, 0);
            this._endTime.Minimum = new System.DateTime(1753, 1, 1, 0, 0, 0, 0);
            this._endTime.Name = "_endTime";
            this._endTime.Nullable = true;
            this._endTime.ShowTime = false;
            this._endTime.Size = new System.Drawing.Size(150, 41);
            this._endTime.TabIndex = 3;
            this._endTime.Value = null;
            // 
            // FolderOptionComponentControl
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this._endTime);
            this.Controls.Add(this._startTime);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.Name = "FolderOptionComponentControl";
            this.Size = new System.Drawing.Size(206, 142);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
        private ClearCanvas.Controls.WinForms.DateTimeField _startTime;
        private ClearCanvas.Controls.WinForms.DateTimeField _endTime;
    }
}
