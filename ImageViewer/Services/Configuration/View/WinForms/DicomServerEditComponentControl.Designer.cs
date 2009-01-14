#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.ImageViewer.Services.Configuration.View.WinForms
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
			this._isStreaming = new System.Windows.Forms.CheckBox();
			this._dicom = new System.Windows.Forms.GroupBox();
			this._general = new System.Windows.Forms.GroupBox();
			this._streaming = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this._wadoServicePort = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this._headerServicePort = new System.Windows.Forms.TextBox();
			this._dicom.SuspendLayout();
			this._general.SuspendLayout();
			this._streaming.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(41, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Server Name";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(66, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "AE Title";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(81, 58);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(29, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Host";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(84, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(26, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Port";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(62, 89);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(48, 13);
			this.label5.TabIndex = 4;
			this.label5.Text = "Location";
			// 
			// _serverName
			// 
			this._serverName.Location = new System.Drawing.Point(116, 26);
			this._serverName.Name = "_serverName";
			this._serverName.Size = new System.Drawing.Size(216, 20);
			this._serverName.TabIndex = 3;
			// 
			// _ae
			// 
			this._ae.Location = new System.Drawing.Point(116, 22);
			this._ae.Name = "_ae";
			this._ae.Size = new System.Drawing.Size(216, 20);
			this._ae.TabIndex = 0;
			// 
			// _host
			// 
			this._host.Location = new System.Drawing.Point(116, 55);
			this._host.Name = "_host";
			this._host.Size = new System.Drawing.Size(216, 20);
			this._host.TabIndex = 4;
			// 
			// _port
			// 
			this._port.Location = new System.Drawing.Point(116, 53);
			this._port.Name = "_port";
			this._port.Size = new System.Drawing.Size(216, 20);
			this._port.TabIndex = 1;
			// 
			// _location
			// 
			this._location.Location = new System.Drawing.Point(116, 86);
			this._location.Name = "_location";
			this._location.Size = new System.Drawing.Size(216, 20);
			this._location.TabIndex = 5;
			// 
			// _btnAccept
			// 
			this._btnAccept.Location = new System.Drawing.Point(109, 376);
			this._btnAccept.Name = "_btnAccept";
			this._btnAccept.Size = new System.Drawing.Size(75, 23);
			this._btnAccept.TabIndex = 3;
			this._btnAccept.Text = "OK";
			this._btnAccept.UseVisualStyleBackColor = true;
			// 
			// _btnCancel
			// 
			this._btnCancel.Location = new System.Drawing.Point(190, 376);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 4;
			this._btnCancel.Text = "Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			// 
			// _isStreaming
			// 
			this._isStreaming.AutoSize = true;
			this._isStreaming.Location = new System.Drawing.Point(32, 27);
			this._isStreaming.Name = "_isStreaming";
			this._isStreaming.Size = new System.Drawing.Size(65, 17);
			this._isStreaming.TabIndex = 2;
			this._isStreaming.Text = "Enabled";
			this._isStreaming.UseVisualStyleBackColor = true;
			// 
			// _dicom
			// 
			this._dicom.Controls.Add(this.label2);
			this._dicom.Controls.Add(this._ae);
			this._dicom.Controls.Add(this.label4);
			this._dicom.Controls.Add(this._port);
			this._dicom.Location = new System.Drawing.Point(13, 142);
			this._dicom.Name = "_dicom";
			this._dicom.Size = new System.Drawing.Size(348, 91);
			this._dicom.TabIndex = 1;
			this._dicom.TabStop = false;
			this._dicom.Text = "DICOM";
			// 
			// _general
			// 
			this._general.Controls.Add(this.label1);
			this._general.Controls.Add(this._serverName);
			this._general.Controls.Add(this.label3);
			this._general.Controls.Add(this._host);
			this._general.Controls.Add(this.label5);
			this._general.Controls.Add(this._location);
			this._general.Location = new System.Drawing.Point(13, 12);
			this._general.Name = "_general";
			this._general.Size = new System.Drawing.Size(348, 124);
			this._general.TabIndex = 0;
			this._general.TabStop = false;
			this._general.Text = "General";
			// 
			// _streaming
			// 
			this._streaming.Controls.Add(this._isStreaming);
			this._streaming.Controls.Add(this.label6);
			this._streaming.Controls.Add(this._headerServicePort);
			this._streaming.Controls.Add(this.label7);
			this._streaming.Controls.Add(this._wadoServicePort);
			this._streaming.Location = new System.Drawing.Point(13, 239);
			this._streaming.Name = "_streaming";
			this._streaming.Size = new System.Drawing.Size(348, 123);
			this._streaming.TabIndex = 2;
			this._streaming.TabStop = false;
			this._streaming.Text = "ClearCanvas Image Streaming";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(35, 86);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(97, 13);
			this.label7.TabIndex = 16;
			this.label7.Text = "Image Service Port";
			// 
			// _wadoServicePort
			// 
			this._wadoServicePort.Location = new System.Drawing.Point(138, 83);
			this._wadoServicePort.Name = "_wadoServicePort";
			this._wadoServicePort.Size = new System.Drawing.Size(194, 20);
			this._wadoServicePort.TabIndex = 4;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(29, 53);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(103, 13);
			this.label6.TabIndex = 14;
			this.label6.Text = "Header Service Port";
			// 
			// _headerServicePort
			// 
			this._headerServicePort.Location = new System.Drawing.Point(138, 50);
			this._headerServicePort.Name = "_headerServicePort";
			this._headerServicePort.Size = new System.Drawing.Size(194, 20);
			this._headerServicePort.TabIndex = 3;
			// 
			// DicomServerEditComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._btnCancel);
			this.Controls.Add(this._btnAccept);
			this.Controls.Add(this._general);
			this.Controls.Add(this._dicom);
			this.Controls.Add(this._streaming);
			this.Name = "DicomServerEditComponentControl";
			this.Size = new System.Drawing.Size(374, 414);
			this._dicom.ResumeLayout(false);
			this._dicom.PerformLayout();
			this._general.ResumeLayout(false);
			this._general.PerformLayout();
			this._streaming.ResumeLayout(false);
			this._streaming.PerformLayout();
			this.ResumeLayout(false);

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
		private System.Windows.Forms.CheckBox _isStreaming;
		private System.Windows.Forms.GroupBox _dicom;
		private System.Windows.Forms.GroupBox _general;
		private System.Windows.Forms.GroupBox _streaming;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox _wadoServicePort;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox _headerServicePort;
    }
}
