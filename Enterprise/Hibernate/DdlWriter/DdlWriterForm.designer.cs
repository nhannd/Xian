namespace ClearCanvas.Enterprise.Hibernate.DdlWriter
{
    partial class DdlWriterForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DdlWriterForm));
            this._btnStart = new System.Windows.Forms.Button();
            this._lblCreate = new System.Windows.Forms.Label();
            this._lblDrop = new System.Windows.Forms.Label();
            this._btnExit = new System.Windows.Forms.Button();
            this._txtCreate = new System.Windows.Forms.TextBox();
            this._txtDrop = new System.Windows.Forms.TextBox();
            this._btnCreateFile = new System.Windows.Forms.Button();
            this._txtStatus = new System.Windows.Forms.TextBox();
            this._btnDropFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _btnStart
            // 
            this._btnStart.Location = new System.Drawing.Point(148, 424);
            this._btnStart.Name = "_btnStart";
            this._btnStart.Size = new System.Drawing.Size(75, 23);
            this._btnStart.TabIndex = 0;
            this._btnStart.Text = "Start";
            this._btnStart.UseVisualStyleBackColor = true;
            this._btnStart.Click += new System.EventHandler(this._btnStart_Click);
            // 
            // _lblCreate
            // 
            this._lblCreate.AutoSize = true;
            this._lblCreate.Location = new System.Drawing.Point(33, 313);
            this._lblCreate.Name = "_lblCreate";
            this._lblCreate.Size = new System.Drawing.Size(129, 13);
            this._lblCreate.TabIndex = 1;
            this._lblCreate.Text = "Create Schema Script File";
            // 
            // _lblDrop
            // 
            this._lblDrop.AutoSize = true;
            this._lblDrop.Location = new System.Drawing.Point(33, 360);
            this._lblDrop.Name = "_lblDrop";
            this._lblDrop.Size = new System.Drawing.Size(121, 13);
            this._lblDrop.TabIndex = 2;
            this._lblDrop.Text = "Drop Schema Script File";
            // 
            // _btnExit
            // 
            this._btnExit.Location = new System.Drawing.Point(281, 424);
            this._btnExit.Name = "_btnExit";
            this._btnExit.Size = new System.Drawing.Size(75, 23);
            this._btnExit.TabIndex = 3;
            this._btnExit.Text = "Exit";
            this._btnExit.UseVisualStyleBackColor = true;
            this._btnExit.Click += new System.EventHandler(this._btnExit_Click);
            // 
            // _txtCreate
            // 
            this._txtCreate.Location = new System.Drawing.Point(36, 329);
            this._txtCreate.Name = "_txtCreate";
            this._txtCreate.Size = new System.Drawing.Size(375, 20);
            this._txtCreate.TabIndex = 4;
            // 
            // _txtDrop
            // 
            this._txtDrop.Location = new System.Drawing.Point(36, 376);
            this._txtDrop.Name = "_txtDrop";
            this._txtDrop.Size = new System.Drawing.Size(375, 20);
            this._txtDrop.TabIndex = 5;
            // 
            // _btnCreateFile
            // 
            this._btnCreateFile.Image = ((System.Drawing.Image)(resources.GetObject("_btnCreateFile.Image")));
            this._btnCreateFile.Location = new System.Drawing.Point(417, 329);
            this._btnCreateFile.Name = "_btnCreateFile";
            this._btnCreateFile.Size = new System.Drawing.Size(27, 20);
            this._btnCreateFile.TabIndex = 6;
            this._btnCreateFile.UseVisualStyleBackColor = true;
            this._btnCreateFile.Click += new System.EventHandler(this._btnCreateFile_Click);
            // 
            // _txtStatus
            // 
            this._txtStatus.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this._txtStatus.Location = new System.Drawing.Point(36, 21);
            this._txtStatus.Multiline = true;
            this._txtStatus.Name = "_txtStatus";
            this._txtStatus.ReadOnly = true;
            this._txtStatus.Size = new System.Drawing.Size(408, 269);
            this._txtStatus.TabIndex = 7;
            // 
            // _btnDropFile
            // 
            this._btnDropFile.Image = ((System.Drawing.Image)(resources.GetObject("_btnDropFile.Image")));
            this._btnDropFile.Location = new System.Drawing.Point(417, 376);
            this._btnDropFile.Name = "_btnDropFile";
            this._btnDropFile.Size = new System.Drawing.Size(27, 20);
            this._btnDropFile.TabIndex = 8;
            this._btnDropFile.UseVisualStyleBackColor = true;
            this._btnDropFile.Click += new System.EventHandler(this._btnDropFile_Click);
            // 
            // DDLWriterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(483, 469);
            this.Controls.Add(this._btnDropFile);
            this.Controls.Add(this._txtStatus);
            this.Controls.Add(this._btnCreateFile);
            this.Controls.Add(this._txtDrop);
            this.Controls.Add(this._txtCreate);
            this.Controls.Add(this._btnExit);
            this.Controls.Add(this._lblDrop);
            this.Controls.Add(this._lblCreate);
            this.Controls.Add(this._btnStart);
            this.Name = "DDLWriterForm";
            this.Text = "DDL Script Writer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _btnStart;
        private System.Windows.Forms.Label _lblCreate;
        private System.Windows.Forms.Label _lblDrop;
        private System.Windows.Forms.Button _btnExit;
        private System.Windows.Forms.TextBox _txtCreate;
        private System.Windows.Forms.TextBox _txtDrop;
        private System.Windows.Forms.Button _btnCreateFile;
        private System.Windows.Forms.TextBox _txtStatus;
        private System.Windows.Forms.Button _btnDropFile;
    }
}