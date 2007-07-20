namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class DHtmlComponentControl
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
            this._webBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // _webBrowser
            // 
            this._webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._webBrowser.Location = new System.Drawing.Point(0, 0);
            this._webBrowser.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._webBrowser.MinimumSize = new System.Drawing.Size(15, 16);
            this._webBrowser.Name = "_webBrowser";
            this._webBrowser.Size = new System.Drawing.Size(398, 384);
            this._webBrowser.TabIndex = 0;
            // 
            // DHtmlComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._webBrowser);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "DHtmlComponentControl";
            this.Size = new System.Drawing.Size(398, 384);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser _webBrowser;
    }
}
