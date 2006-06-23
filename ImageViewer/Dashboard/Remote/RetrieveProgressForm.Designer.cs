namespace ClearCanvas.Workstation.Dashboard.Remote
{
    partial class RetrieveProgressForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._labelDescription = new System.Windows.Forms.Label();
            this._progressRetrieve = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // _labelDescription
            // 
            this._labelDescription.AutoSize = true;
            this._labelDescription.Location = new System.Drawing.Point(13, 13);
            this._labelDescription.Name = "_labelDescription";
            this._labelDescription.Size = new System.Drawing.Size(46, 17);
            this._labelDescription.TabIndex = 0;
            this._labelDescription.Text = "label1";
            // 
            // _progressRetrieve
            // 
            this._progressRetrieve.Location = new System.Drawing.Point(16, 51);
            this._progressRetrieve.Name = "_progressRetrieve";
            this._progressRetrieve.Size = new System.Drawing.Size(643, 23);
            this._progressRetrieve.TabIndex = 1;
            // 
            // RetrieveProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 85);
            this.Controls.Add(this._progressRetrieve);
            this.Controls.Add(this._labelDescription);
            this.Name = "RetrieveProgressForm";
            this.Text = "Retrieve Progress";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _labelDescription;
        private System.Windows.Forms.ProgressBar _progressRetrieve;
    }
}