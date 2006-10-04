namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class AEServerEditorComponentControl
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
			this._lblServerName = new System.Windows.Forms.Label();
			this._lblAETitle = new System.Windows.Forms.Label();
			this._lblHost = new System.Windows.Forms.Label();
			this._lblPort = new System.Windows.Forms.Label();
			this._lblLocation = new System.Windows.Forms.Label();
			this._serverName = new System.Windows.Forms.TextBox();
			this._aetitle = new System.Windows.Forms.TextBox();
			this._host = new System.Windows.Forms.TextBox();
			this._port = new System.Windows.Forms.TextBox();
			this._location = new System.Windows.Forms.TextBox();
			this._btnAccept = new System.Windows.Forms.Button();
			this._btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _lblServerName
			// 
			this._lblServerName.AutoSize = true;
			this._lblServerName.Location = new System.Drawing.Point(28, 20);
			this._lblServerName.Name = "_lblServerName";
			this._lblServerName.Size = new System.Drawing.Size(69, 13);
			this._lblServerName.TabIndex = 0;
			this._lblServerName.Text = "Server Name";
			// 
			// _lblAETitle
			// 
			this._lblAETitle.AutoSize = true;
			this._lblAETitle.Location = new System.Drawing.Point(53, 49);
			this._lblAETitle.Name = "_lblAETitle";
			this._lblAETitle.Size = new System.Drawing.Size(44, 13);
			this._lblAETitle.TabIndex = 1;
			this._lblAETitle.Text = "AE Title";
			// 
			// _lblHost
			// 
			this._lblHost.AutoSize = true;
			this._lblHost.Location = new System.Drawing.Point(68, 76);
			this._lblHost.Name = "_lblHost";
			this._lblHost.Size = new System.Drawing.Size(29, 13);
			this._lblHost.TabIndex = 2;
			this._lblHost.Text = "Host";
			// 
			// _lblPort
			// 
			this._lblPort.AutoSize = true;
			this._lblPort.Location = new System.Drawing.Point(71, 103);
			this._lblPort.Name = "_lblPort";
			this._lblPort.Size = new System.Drawing.Size(26, 13);
			this._lblPort.TabIndex = 3;
			this._lblPort.Text = "Port";
			// 
			// _lblLocation
			// 
			this._lblLocation.AutoSize = true;
			this._lblLocation.Location = new System.Drawing.Point(49, 129);
			this._lblLocation.Name = "_lblLocation";
			this._lblLocation.Size = new System.Drawing.Size(48, 13);
			this._lblLocation.TabIndex = 4;
			this._lblLocation.Text = "Location";
			// 
			// _serverName
			// 
			this._serverName.Location = new System.Drawing.Point(104, 17);
			this._serverName.Name = "_serverName";
			this._serverName.Size = new System.Drawing.Size(185, 20);
			this._serverName.TabIndex = 5;
			// 
			// _aetitle
			// 
			this._aetitle.Location = new System.Drawing.Point(104, 46);
			this._aetitle.Name = "_aetitle";
			this._aetitle.Size = new System.Drawing.Size(185, 20);
			this._aetitle.TabIndex = 6;
			// 
			// _host
			// 
			this._host.Location = new System.Drawing.Point(104, 73);
			this._host.Name = "_host";
			this._host.Size = new System.Drawing.Size(185, 20);
			this._host.TabIndex = 7;
			// 
			// _port
			// 
			this._port.Location = new System.Drawing.Point(104, 100);
			this._port.Name = "_port";
			this._port.Size = new System.Drawing.Size(185, 20);
			this._port.TabIndex = 8;
			// 
			// _location
			// 
			this._location.Location = new System.Drawing.Point(104, 126);
			this._location.Name = "_location";
			this._location.Size = new System.Drawing.Size(185, 20);
			this._location.TabIndex = 9;
			// 
			// _btnAccept
			// 
			this._btnAccept.Location = new System.Drawing.Point(86, 166);
			this._btnAccept.Name = "_btnAccept";
			this._btnAccept.Size = new System.Drawing.Size(75, 23);
			this._btnAccept.TabIndex = 10;
			this._btnAccept.Text = "Accept";
			this._btnAccept.UseVisualStyleBackColor = true;
			// 
			// _btnCancel
			// 
			this._btnCancel.Location = new System.Drawing.Point(167, 166);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 11;
			this._btnCancel.Text = "Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			// 
			// AEServerEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._btnCancel);
			this.Controls.Add(this._btnAccept);
			this.Controls.Add(this._location);
			this.Controls.Add(this._port);
			this.Controls.Add(this._host);
			this.Controls.Add(this._aetitle);
			this.Controls.Add(this._serverName);
			this.Controls.Add(this._lblLocation);
			this.Controls.Add(this._lblPort);
			this.Controls.Add(this._lblHost);
			this.Controls.Add(this._lblAETitle);
			this.Controls.Add(this._lblServerName);
			this.Name = "AEServerEditorComponentControl";
			this.Size = new System.Drawing.Size(330, 213);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lblServerName;
        private System.Windows.Forms.Label _lblAETitle;
        private System.Windows.Forms.Label _lblHost;
        private System.Windows.Forms.Label _lblPort;
        private System.Windows.Forms.Label _lblLocation;
        private System.Windows.Forms.TextBox _serverName;
        private System.Windows.Forms.TextBox _aetitle;
        private System.Windows.Forms.TextBox _host;
        private System.Windows.Forms.TextBox _port;
        private System.Windows.Forms.TextBox _location;
        private System.Windows.Forms.Button _btnAccept;
        private System.Windows.Forms.Button _btnCancel;
    }
}
