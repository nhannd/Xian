namespace ClearCanvas.ImageViewer.View.WinForms
{
    partial class TimedDialogControl
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
            this._message = new System.Windows.Forms.Label();
            this._link = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // _message
            // 
            this._message.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._message.AutoEllipsis = true;
            this._message.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._message.Location = new System.Drawing.Point(0, 0);
            this._message.Name = "_message";
            this._message.Size = new System.Drawing.Size(302, 75);
            this._message.TabIndex = 4;
            this._message.Text = "The re-index has been scheduled.";
            this._message.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _link
            // 
            this._link.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._link.AutoSize = true;
            this._link.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this._link.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this._link.Location = new System.Drawing.Point(80, 75);
            this._link.Name = "_link";
            this._link.Size = new System.Drawing.Size(142, 17);
            this._link.TabIndex = 5;
            this._link.TabStop = true;
            this._link.Text = "Open Activity Monitor";
            this._link.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this._link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // TimedDialogControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this._link);
            this.Controls.Add(this._message);
            this.Name = "TimedDialogControl";
            this.Size = new System.Drawing.Size(302, 106);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _message;
        private System.Windows.Forms.LinkLabel _link;



    }
}
