namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class DicomServerEditComponentControl
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this._serverName = new System.Windows.Forms.TextBox();
			this._ae = new System.Windows.Forms.TextBox();
			this._host = new System.Windows.Forms.TextBox();
			this._port = new System.Windows.Forms.TextBox();
			this._location = new System.Windows.Forms.TextBox();
			this._btnAccept = new System.Windows.Forms.Button();
			this._btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Server Name";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(47, 55);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "AE Title";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(63, 81);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Host";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(66, 107);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(26, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Port";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(43, 133);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(48, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Location";
			// 
			// _serverName
			// 
			this._serverName.Location = new System.Drawing.Point(98, 25);
			this._serverName.Name = "_serverName";
			this._serverName.Size = new System.Drawing.Size(216, 20);
			this._serverName.TabIndex = 5;
			// 
			// _ae
			// 
			this._ae.Location = new System.Drawing.Point(98, 52);
			this._ae.Name = "_ae";
			this._ae.Size = new System.Drawing.Size(216, 20);
			this._ae.TabIndex = 6;
			// 
			// _host
			// 
			this._host.Location = new System.Drawing.Point(98, 78);
			this._host.Name = "_host";
			this._host.Size = new System.Drawing.Size(216, 20);
			this._host.TabIndex = 7;
			// 
			// _port
			// 
			this._port.Location = new System.Drawing.Point(98, 104);
			this._port.Name = "_port";
			this._port.Size = new System.Drawing.Size(216, 20);
			this._port.TabIndex = 8;
			// 
			// _location
			// 
			this._location.Location = new System.Drawing.Point(98, 130);
			this._location.Name = "_location";
			this._location.Size = new System.Drawing.Size(216, 20);
			this._location.TabIndex = 9;
			// 
			// _btnAccept
			// 
			this._btnAccept.Location = new System.Drawing.Point(97, 168);
			this._btnAccept.Name = "_btnAccept";
			this._btnAccept.Size = new System.Drawing.Size(75, 23);
			this._btnAccept.TabIndex = 10;
			this._btnAccept.Text = "OK";
			this._btnAccept.UseVisualStyleBackColor = true;
			// 
			// _btnCancel
			// 
			this._btnCancel.Location = new System.Drawing.Point(178, 168);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 11;
			this._btnCancel.Text = "Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			// 
			// DicomServerEditComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._btnCancel);
			this.Controls.Add(this._btnAccept);
			this.Controls.Add(this._location);
			this.Controls.Add(this._port);
			this.Controls.Add(this._host);
			this.Controls.Add(this._ae);
			this.Controls.Add(this._serverName);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Name = "DicomServerEditComponentControl";
			this.Size = new System.Drawing.Size(351, 214);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox _serverName;
        private System.Windows.Forms.TextBox _ae;
        private System.Windows.Forms.TextBox _host;
        private System.Windows.Forms.TextBox _port;
        private System.Windows.Forms.TextBox _location;
        private System.Windows.Forms.Button _btnAccept;
        private System.Windows.Forms.Button _btnCancel;
    }
}
