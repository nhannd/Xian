namespace ClearCanvas.ImageViewer.Dashboard.Remote
{
    partial class ServerModifyForm
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
            this._labelName = new System.Windows.Forms.Label();
            this._textName = new System.Windows.Forms.TextBox();
            this._labelDescription = new System.Windows.Forms.Label();
            this._textDescription = new System.Windows.Forms.TextBox();
            this._labelAETitle = new System.Windows.Forms.Label();
            this._textAETitle = new System.Windows.Forms.TextBox();
            this._labelHost = new System.Windows.Forms.Label();
            this._textHost = new System.Windows.Forms.TextBox();
            this._labelListeningPort = new System.Windows.Forms.Label();
            this._textListeningPort = new System.Windows.Forms.TextBox();
            this._buttonOk = new System.Windows.Forms.Button();
            this._buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _labelName
            // 
            this._labelName.AutoSize = true;
            this._labelName.Location = new System.Drawing.Point(55, 53);
            this._labelName.Name = "_labelName";
            this._labelName.Size = new System.Drawing.Size(45, 17);
            this._labelName.TabIndex = 0;
            this._labelName.Text = "Name";
            // 
            // _textName
            // 
            this._textName.Location = new System.Drawing.Point(106, 50);
            this._textName.Name = "_textName";
            this._textName.Size = new System.Drawing.Size(272, 22);
            this._textName.TabIndex = 1;
            // 
            // _labelDescription
            // 
            this._labelDescription.AutoSize = true;
            this._labelDescription.Location = new System.Drawing.Point(21, 84);
            this._labelDescription.Name = "_labelDescription";
            this._labelDescription.Size = new System.Drawing.Size(79, 17);
            this._labelDescription.TabIndex = 2;
            this._labelDescription.Text = "Description";
            // 
            // _textDescription
            // 
            this._textDescription.Location = new System.Drawing.Point(106, 82);
            this._textDescription.Name = "_textDescription";
            this._textDescription.Size = new System.Drawing.Size(272, 22);
            this._textDescription.TabIndex = 3;
            // 
            // _labelAETitle
            // 
            this._labelAETitle.AutoSize = true;
            this._labelAETitle.Location = new System.Drawing.Point(43, 118);
            this._labelAETitle.Name = "_labelAETitle";
            this._labelAETitle.Size = new System.Drawing.Size(57, 17);
            this._labelAETitle.TabIndex = 4;
            this._labelAETitle.Text = "AE Title";
            // 
            // _textAETitle
            // 
            this._textAETitle.Location = new System.Drawing.Point(106, 115);
            this._textAETitle.Name = "_textAETitle";
            this._textAETitle.Size = new System.Drawing.Size(272, 22);
            this._textAETitle.TabIndex = 5;
            // 
            // _labelHost
            // 
            this._labelHost.AutoSize = true;
            this._labelHost.Location = new System.Drawing.Point(63, 151);
            this._labelHost.Name = "_labelHost";
            this._labelHost.Size = new System.Drawing.Size(37, 17);
            this._labelHost.TabIndex = 6;
            this._labelHost.Text = "Host";
            // 
            // _textHost
            // 
            this._textHost.Location = new System.Drawing.Point(106, 148);
            this._textHost.Name = "_textHost";
            this._textHost.Size = new System.Drawing.Size(151, 22);
            this._textHost.TabIndex = 7;
            // 
            // _labelListeningPort
            // 
            this._labelListeningPort.AutoSize = true;
            this._labelListeningPort.Location = new System.Drawing.Point(5, 182);
            this._labelListeningPort.Name = "_labelListeningPort";
            this._labelListeningPort.Size = new System.Drawing.Size(95, 17);
            this._labelListeningPort.TabIndex = 8;
            this._labelListeningPort.Text = "Listening Port";
            // 
            // _textListeningPort
            // 
            this._textListeningPort.Location = new System.Drawing.Point(106, 179);
            this._textListeningPort.Name = "_textListeningPort";
            this._textListeningPort.Size = new System.Drawing.Size(151, 22);
            this._textListeningPort.TabIndex = 9;
            // 
            // _buttonOk
            // 
            this._buttonOk.Location = new System.Drawing.Point(128, 236);
            this._buttonOk.Name = "_buttonOk";
            this._buttonOk.Size = new System.Drawing.Size(75, 23);
            this._buttonOk.TabIndex = 10;
            this._buttonOk.Text = "OK";
            this._buttonOk.UseVisualStyleBackColor = true;
            this._buttonOk.Click += new System.EventHandler(this._buttonOk_Click);
            // 
            // _buttonCancel
            // 
            this._buttonCancel.Location = new System.Drawing.Point(209, 236);
            this._buttonCancel.Name = "_buttonCancel";
            this._buttonCancel.Size = new System.Drawing.Size(75, 23);
            this._buttonCancel.TabIndex = 11;
            this._buttonCancel.Text = "Cancel";
            this._buttonCancel.UseVisualStyleBackColor = true;
            this._buttonCancel.Click += new System.EventHandler(this._buttonCancel_Click);
            // 
            // ServerModifyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 279);
            this.Controls.Add(this._buttonCancel);
            this.Controls.Add(this._buttonOk);
            this.Controls.Add(this._textListeningPort);
            this.Controls.Add(this._labelListeningPort);
            this.Controls.Add(this._textHost);
            this.Controls.Add(this._labelHost);
            this.Controls.Add(this._textAETitle);
            this.Controls.Add(this._labelAETitle);
            this.Controls.Add(this._textDescription);
            this.Controls.Add(this._labelDescription);
            this.Controls.Add(this._textName);
            this.Controls.Add(this._labelName);
            this.Name = "ServerModifyForm";
            this.Text = "Modify Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _labelName;
        private System.Windows.Forms.TextBox _textName;
        private System.Windows.Forms.Label _labelDescription;
        private System.Windows.Forms.TextBox _textDescription;
        private System.Windows.Forms.Label _labelAETitle;
        private System.Windows.Forms.TextBox _textAETitle;
        private System.Windows.Forms.Label _labelHost;
        private System.Windows.Forms.TextBox _textHost;
        private System.Windows.Forms.Label _labelListeningPort;
        private System.Windows.Forms.TextBox _textListeningPort;
        private System.Windows.Forms.Button _buttonOk;
        private System.Windows.Forms.Button _buttonCancel;
    }
}