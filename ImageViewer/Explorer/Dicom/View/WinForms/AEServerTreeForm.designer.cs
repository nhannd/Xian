namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class AEServerTreeForm
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
            this._aeserverTree = new System.Windows.Forms.TreeView();
            this._serverName = new System.Windows.Forms.TextBox();
            this._serverDesc = new System.Windows.Forms.TextBox();
            this._serverHost = new System.Windows.Forms.TextBox();
            this._lblDesc = new System.Windows.Forms.Label();
            this._lblName = new System.Windows.Forms.Label();
            this._lblAE = new System.Windows.Forms.Label();
            this._lblHost = new System.Windows.Forms.Label();
            this._lblPort = new System.Windows.Forms.Label();
            this._serverAE = new System.Windows.Forms.TextBox();
            this._serverPort = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _aeserverTree
            // 
            this._aeserverTree.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._aeserverTree.Location = new System.Drawing.Point(4, 4);
            this._aeserverTree.Name = "_aeserverTree";
            this._aeserverTree.Size = new System.Drawing.Size(237, 209);
            this._aeserverTree.TabIndex = 0;
            // 
            // _serverName
            // 
            this._serverName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._serverName.Location = new System.Drawing.Point(69, 229);
            this._serverName.Name = "_serverName";
            this._serverName.Size = new System.Drawing.Size(172, 20);
            this._serverName.TabIndex = 1;
            // 
            // _serverDesc
            // 
            this._serverDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._serverDesc.Location = new System.Drawing.Point(69, 256);
            this._serverDesc.Name = "_serverDesc";
            this._serverDesc.Size = new System.Drawing.Size(172, 20);
            this._serverDesc.TabIndex = 2;
            // 
            // _serverHost
            // 
            this._serverHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._serverHost.Location = new System.Drawing.Point(69, 310);
            this._serverHost.Name = "_serverHost";
            this._serverHost.Size = new System.Drawing.Size(172, 20);
            this._serverHost.TabIndex = 4;
            // 
            // _lblDesc
            // 
            this._lblDesc.AutoSize = true;
            this._lblDesc.Location = new System.Drawing.Point(3, 259);
            this._lblDesc.Name = "_lblDesc";
            this._lblDesc.Size = new System.Drawing.Size(60, 13);
            this._lblDesc.TabIndex = 6;
            this._lblDesc.Text = "Description";
            // 
            // _lblName
            // 
            this._lblName.AutoSize = true;
            this._lblName.Location = new System.Drawing.Point(28, 232);
            this._lblName.Name = "_lblName";
            this._lblName.Size = new System.Drawing.Size(35, 13);
            this._lblName.TabIndex = 7;
            this._lblName.Text = "Name";
            // 
            // _lblAE
            // 
            this._lblAE.AutoSize = true;
            this._lblAE.Location = new System.Drawing.Point(19, 286);
            this._lblAE.Name = "_lblAE";
            this._lblAE.Size = new System.Drawing.Size(44, 13);
            this._lblAE.TabIndex = 8;
            this._lblAE.Text = "AE Title";
            // 
            // _lblHost
            // 
            this._lblHost.AutoSize = true;
            this._lblHost.Location = new System.Drawing.Point(34, 313);
            this._lblHost.Name = "_lblHost";
            this._lblHost.Size = new System.Drawing.Size(29, 13);
            this._lblHost.TabIndex = 9;
            this._lblHost.Text = "Host";
            // 
            // _lblPort
            // 
            this._lblPort.AutoSize = true;
            this._lblPort.Location = new System.Drawing.Point(37, 340);
            this._lblPort.Name = "_lblPort";
            this._lblPort.Size = new System.Drawing.Size(26, 13);
            this._lblPort.TabIndex = 10;
            this._lblPort.Text = "Port";
            // 
            // _serverAE
            // 
            this._serverAE.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._serverAE.Location = new System.Drawing.Point(69, 282);
            this._serverAE.Name = "_serverAE";
            this._serverAE.Size = new System.Drawing.Size(172, 20);
            this._serverAE.TabIndex = 12;
            // 
            // _serverPort
            // 
            this._serverPort.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._serverPort.Location = new System.Drawing.Point(70, 337);
            this._serverPort.Name = "_serverPort";
            this._serverPort.Size = new System.Drawing.Size(171, 20);
            this._serverPort.TabIndex = 13;
            // 
            // AEServerTreeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._serverPort);
            this.Controls.Add(this._serverAE);
            this.Controls.Add(this._lblPort);
            this.Controls.Add(this._lblHost);
            this.Controls.Add(this._lblAE);
            this.Controls.Add(this._lblName);
            this.Controls.Add(this._lblDesc);
            this.Controls.Add(this._serverHost);
            this.Controls.Add(this._serverDesc);
            this.Controls.Add(this._serverName);
            this.Controls.Add(this._aeserverTree);
            this.Name = "AEServerTreeForm";
            this.Size = new System.Drawing.Size(244, 370);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView _aeserverTree;
        private System.Windows.Forms.TextBox _serverName;
        private System.Windows.Forms.TextBox _serverDesc;
        private System.Windows.Forms.TextBox _serverHost;
        private System.Windows.Forms.Label _lblDesc;
        private System.Windows.Forms.Label _lblName;
        private System.Windows.Forms.Label _lblAE;
        private System.Windows.Forms.Label _lblHost;
        private System.Windows.Forms.Label _lblPort;
        private System.Windows.Forms.TextBox _serverAE;
        private System.Windows.Forms.TextBox _serverPort;
    }
}
