namespace ClickOncePublisher
{
	partial class ProductProfileForm
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
			this.components = new System.ComponentModel.Container();
			this._productDirectoryBrowseButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._directory = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this._name = new System.Windows.Forms.TextBox();
			this._packages = new System.Windows.Forms.CheckedListBox();
			this.label3 = new System.Windows.Forms.Label();
			this._version = new System.Windows.Forms.TextBox();
			this._applicationUrl = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this._setupUrl = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._nameErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this._versionErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this._directoryErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.label7 = new System.Windows.Forms.Label();
			this._entryPointPath = new System.Windows.Forms.TextBox();
			this._entryPointPathBrowseButton = new System.Windows.Forms.Button();
			this._entryPointPathErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._nameErrorProvider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._versionErrorProvider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._directoryErrorProvider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._entryPointPathErrorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// _productDirectoryBrowseButton
			// 
			this._productDirectoryBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._productDirectoryBrowseButton.Location = new System.Drawing.Point(415, 145);
			this._productDirectoryBrowseButton.Name = "_productDirectoryBrowseButton";
			this._productDirectoryBrowseButton.Size = new System.Drawing.Size(27, 20);
			this._productDirectoryBrowseButton.TabIndex = 3;
			this._productDirectoryBrowseButton.Text = "...";
			this._productDirectoryBrowseButton.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(24, 30);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(75, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Product Name";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(24, 138);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(89, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Product Directory";
			// 
			// _directory
			// 
			this._directory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._directory.Location = new System.Drawing.Point(27, 154);
			this._directory.Name = "_directory";
			this._directory.ReadOnly = true;
			this._directory.Size = new System.Drawing.Size(379, 20);
			this._directory.TabIndex = 2;
			this._directory.Validated += new System.EventHandler(this.OnDirectoryValidated);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(26, 82);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(82, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "Product Version";
			// 
			// _name
			// 
			this._name.Location = new System.Drawing.Point(27, 47);
			this._name.Name = "_name";
			this._name.Size = new System.Drawing.Size(195, 20);
			this._name.TabIndex = 0;
			this._name.Validated += new System.EventHandler(this.OnNameValidated);
			// 
			// _packages
			// 
			this._packages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._packages.FormattingEnabled = true;
			this._packages.Location = new System.Drawing.Point(27, 272);
			this._packages.Name = "_packages";
			this._packages.Size = new System.Drawing.Size(428, 124);
			this._packages.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 247);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(67, 13);
			this.label3.TabIndex = 12;
			this.label3.Text = "Prerequisites";
			// 
			// _version
			// 
			this._version.Location = new System.Drawing.Point(27, 98);
			this._version.Name = "_version";
			this._version.Size = new System.Drawing.Size(195, 20);
			this._version.TabIndex = 1;
			this._version.Validated += new System.EventHandler(this.OnVersionValidated);
			// 
			// _applicationUrl
			// 
			this._applicationUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._applicationUrl.Location = new System.Drawing.Point(27, 479);
			this._applicationUrl.Name = "_applicationUrl";
			this._applicationUrl.ReadOnly = true;
			this._applicationUrl.Size = new System.Drawing.Size(427, 20);
			this._applicationUrl.TabIndex = 13;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(24, 463);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(84, 13);
			this.label5.TabIndex = 14;
			this.label5.Text = "Application URL";
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(24, 509);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(60, 13);
			this.label6.TabIndex = 16;
			this.label6.Text = "Setup URL";
			// 
			// _setupUrl
			// 
			this._setupUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._setupUrl.Location = new System.Drawing.Point(27, 525);
			this._setupUrl.Name = "_setupUrl";
			this._setupUrl.ReadOnly = true;
			this._setupUrl.Size = new System.Drawing.Size(427, 20);
			this._setupUrl.TabIndex = 15;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this._productDirectoryBrowseButton);
			this.groupBox1.Controls.Add(this._entryPointPath);
			this.groupBox1.Controls.Add(this._entryPointPathBrowseButton);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(12, 9);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(460, 421);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Product Profile";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Location = new System.Drawing.Point(13, 442);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(460, 124);
			this.groupBox2.TabIndex = 18;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Deployment URLs";
			// 
			// _nameErrorProvider
			// 
			this._nameErrorProvider.ContainerControl = this;
			// 
			// _versionErrorProvider
			// 
			this._versionErrorProvider.ContainerControl = this;
			// 
			// _directoryErrorProvider
			// 
			this._directoryErrorProvider.ContainerControl = this;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(12, 185);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(87, 13);
			this.label7.TabIndex = 21;
			this.label7.Text = "Entry Point (.exe)";
			// 
			// _entryPointPath
			// 
			this._entryPointPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._entryPointPath.Location = new System.Drawing.Point(15, 201);
			this._entryPointPath.Name = "_entryPointPath";
			this._entryPointPath.ReadOnly = true;
			this._entryPointPath.Size = new System.Drawing.Size(379, 20);
			this._entryPointPath.TabIndex = 19;
			this._entryPointPath.Validated += new System.EventHandler(this.OnEntryPointPathValidated);
			// 
			// _entryPointPathBrowseButton
			// 
			this._entryPointPathBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._entryPointPathBrowseButton.Location = new System.Drawing.Point(415, 200);
			this._entryPointPathBrowseButton.Name = "_entryPointPathBrowseButton";
			this._entryPointPathBrowseButton.Size = new System.Drawing.Size(27, 20);
			this._entryPointPathBrowseButton.TabIndex = 20;
			this._entryPointPathBrowseButton.Text = "...";
			this._entryPointPathBrowseButton.UseVisualStyleBackColor = true;
			// 
			// _entryPointPathErrorProvider
			// 
			this._entryPointPathErrorProvider.ContainerControl = this;
			// 
			// ProductProfileForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(487, 585);
			this.Controls.Add(this.label6);
			this.Controls.Add(this._setupUrl);
			this.Controls.Add(this.label5);
			this.Controls.Add(this._applicationUrl);
			this.Controls.Add(this._version);
			this.Controls.Add(this._packages);
			this.Controls.Add(this._name);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._directory);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox2);
			this.Name = "ProductProfileForm";
			this.Text = "Product";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._nameErrorProvider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._versionErrorProvider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._directoryErrorProvider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._entryPointPathErrorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button _productDirectoryBrowseButton;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _directory;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox _name;
		private System.Windows.Forms.CheckedListBox _packages;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _version;
		private System.Windows.Forms.TextBox _applicationUrl;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox _setupUrl;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ErrorProvider _nameErrorProvider;
		private System.Windows.Forms.ErrorProvider _versionErrorProvider;
		private System.Windows.Forms.ErrorProvider _directoryErrorProvider;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox _entryPointPath;
		private System.Windows.Forms.Button _entryPointPathBrowseButton;
		private System.Windows.Forms.ErrorProvider _entryPointPathErrorProvider;
	}
}