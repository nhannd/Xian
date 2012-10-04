namespace ClickOncePublisher
{
	partial class OptionsForm
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
			this._certificateFileBrowseButton = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this._certificateFile = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this._baseUrl = new System.Windows.Forms.TextBox();
			this._publishDirectoryBrowseButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this._publishDirectory = new System.Windows.Forms.TextBox();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this._certificatePassword = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// _certificateFileBrowseButton
			// 
			this._certificateFileBrowseButton.Location = new System.Drawing.Point(335, 81);
			this._certificateFileBrowseButton.Name = "_certificateFileBrowseButton";
			this._certificateFileBrowseButton.Size = new System.Drawing.Size(27, 20);
			this._certificateFileBrowseButton.TabIndex = 22;
			this._certificateFileBrowseButton.Text = "...";
			this._certificateFileBrowseButton.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(9, 65);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(73, 13);
			this.label7.TabIndex = 21;
			this.label7.Text = "Certificate File";
			// 
			// _certificateFile
			// 
			this._certificateFile.Location = new System.Drawing.Point(12, 81);
			this._certificateFile.Name = "_certificateFile";
			this._certificateFile.ReadOnly = true;
			this._certificateFile.Size = new System.Drawing.Size(317, 20);
			this._certificateFile.TabIndex = 20;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(9, 170);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(115, 13);
			this.label5.TabIndex = 19;
			this.label5.Text = "Base Deployment URL";
			// 
			// _baseUrl
			// 
			this._baseUrl.Location = new System.Drawing.Point(12, 186);
			this._baseUrl.Name = "_baseUrl";
			this._baseUrl.Size = new System.Drawing.Size(317, 20);
			this._baseUrl.TabIndex = 18;
			// 
			// _publishDirectoryBrowseButton
			// 
			this._publishDirectoryBrowseButton.Location = new System.Drawing.Point(335, 32);
			this._publishDirectoryBrowseButton.Name = "_publishDirectoryBrowseButton";
			this._publishDirectoryBrowseButton.Size = new System.Drawing.Size(27, 20);
			this._publishDirectoryBrowseButton.TabIndex = 16;
			this._publishDirectoryBrowseButton.Text = "...";
			this._publishDirectoryBrowseButton.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(86, 13);
			this.label3.TabIndex = 17;
			this.label3.Text = "Publish Directory";
			// 
			// _publishDirectory
			// 
			this._publishDirectory.Location = new System.Drawing.Point(12, 32);
			this._publishDirectory.Name = "_publishDirectory";
			this._publishDirectory.ReadOnly = true;
			this._publishDirectory.Size = new System.Drawing.Size(317, 20);
			this._publishDirectory.TabIndex = 15;
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(113, 232);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 23;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(194, 232);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 24;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 117);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 13);
			this.label1.TabIndex = 26;
			this.label1.Text = "Certificate Password";
			// 
			// _certificatePassword
			// 
			this._certificatePassword.Location = new System.Drawing.Point(12, 133);
			this._certificatePassword.Name = "_certificatePassword";
			this._certificatePassword.PasswordChar = '*';
			this._certificatePassword.Size = new System.Drawing.Size(317, 20);
			this._certificatePassword.TabIndex = 25;
			// 
			// OptionsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(382, 287);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._certificatePassword);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._certificateFileBrowseButton);
			this.Controls.Add(this.label7);
			this.Controls.Add(this._certificateFile);
			this.Controls.Add(this.label5);
			this.Controls.Add(this._baseUrl);
			this.Controls.Add(this._publishDirectoryBrowseButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._publishDirectory);
			this.Name = "OptionsForm";
			this.Text = "Options";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _certificateFileBrowseButton;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox _certificateFile;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox _baseUrl;
		private System.Windows.Forms.Button _publishDirectoryBrowseButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _publishDirectory;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _certificatePassword;
	}
}