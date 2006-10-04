namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class AEServerGroupEditorComponentControl
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
			this._lblSvrGroupName = new System.Windows.Forms.Label();
			this._serverGroupName = new System.Windows.Forms.TextBox();
			this._btnAccept = new System.Windows.Forms.Button();
			this._btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _lblSvrGroupName
			// 
			this._lblSvrGroupName.AutoSize = true;
			this._lblSvrGroupName.Location = new System.Drawing.Point(14, 23);
			this._lblSvrGroupName.Name = "_lblSvrGroupName";
			this._lblSvrGroupName.Size = new System.Drawing.Size(101, 13);
			this._lblSvrGroupName.TabIndex = 0;
			this._lblSvrGroupName.Text = "Server Group Name";
			// 
			// _serverGroupName
			// 
			this._serverGroupName.Location = new System.Drawing.Point(121, 23);
			this._serverGroupName.Name = "_serverGroupName";
			this._serverGroupName.Size = new System.Drawing.Size(167, 20);
			this._serverGroupName.TabIndex = 1;
			// 
			// _btnAccept
			// 
			this._btnAccept.Location = new System.Drawing.Point(80, 66);
			this._btnAccept.Name = "_btnAccept";
			this._btnAccept.Size = new System.Drawing.Size(75, 23);
			this._btnAccept.TabIndex = 2;
			this._btnAccept.Text = "Accept";
			this._btnAccept.UseVisualStyleBackColor = true;
			// 
			// _btnCancel
			// 
			this._btnCancel.Location = new System.Drawing.Point(161, 66);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 3;
			this._btnCancel.Text = "Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			// 
			// AEServerGroupEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._btnCancel);
			this.Controls.Add(this._btnAccept);
			this.Controls.Add(this._serverGroupName);
			this.Controls.Add(this._lblSvrGroupName);
			this.Name = "AEServerGroupEditorComponentControl";
			this.Size = new System.Drawing.Size(318, 110);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _lblSvrGroupName;
        private System.Windows.Forms.TextBox _serverGroupName;
        private System.Windows.Forms.Button _btnAccept;
        private System.Windows.Forms.Button _btnCancel;
    }
}
