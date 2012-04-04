namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class StudyBrowserBannerControl
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
            this.components = new System.ComponentModel.Container();
            this._resultsTitleBar = new ClearCanvas.Desktop.View.WinForms.TitleBar();
            this._notificationText = new System.Windows.Forms.Label();
            this._notificationDetailsTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // _resultsTitleBar
            // 
            this._resultsTitleBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._resultsTitleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold);
            this._resultsTitleBar.Location = new System.Drawing.Point(0, 0);
            this._resultsTitleBar.Margin = new System.Windows.Forms.Padding(0);
            this._resultsTitleBar.Name = "_resultsTitleBar";
            this._resultsTitleBar.Size = new System.Drawing.Size(582, 30);
            this._resultsTitleBar.TabIndex = 5;
            this._resultsTitleBar.Text = "10 results found on server";
            this._resultsTitleBar.TextAlignment = System.Drawing.StringAlignment.Near;
            // 
            // _notificationText
            // 
            this._notificationText.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this._notificationText.AutoEllipsis = true;
            this._notificationText.BackColor = System.Drawing.Color.Firebrick;
            this._notificationText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._notificationText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._notificationText.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._notificationText.ForeColor = System.Drawing.Color.White;
            this._notificationText.Location = new System.Drawing.Point(413, 3);
            this._notificationText.Name = "_notificationText";
            this._notificationText.Size = new System.Drawing.Size(165, 24);
            this._notificationText.TabIndex = 6;
            this._notificationText.Text = "Re-index not running";
            this._notificationText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // StudyBrowserBannerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._notificationText);
            this.Controls.Add(this._resultsTitleBar);
            this.Name = "StudyBrowserBannerControl";
            this.Size = new System.Drawing.Size(582, 30);
            this.ResumeLayout(false);

        }

        #endregion

        private Desktop.View.WinForms.TitleBar _resultsTitleBar;
        private System.Windows.Forms.Label _notificationText;
        private System.Windows.Forms.ToolTip _notificationDetailsTooltip;
    }
}
