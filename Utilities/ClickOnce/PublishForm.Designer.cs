namespace ClickOncePublisher
{
	partial class PublishForm
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
			this._productsPath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._publishPath = new System.Windows.Forms.TextBox();
			this._product = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this._productPathBrowseButton = new System.Windows.Forms.Button();
			this._publishPathBrowseButton = new System.Windows.Forms.Button();
			this._publishButton = new System.Windows.Forms.Button();
			this._version = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// _productPath
			// 
			this._productsPath.Location = new System.Drawing.Point(12, 133);
			this._productsPath.Name = "_productPath";
			this._productsPath.ReadOnly = true;
			this._productsPath.Size = new System.Drawing.Size(317, 20);
			this._productsPath.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 117);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Product Path";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Product";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 169);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(66, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "Publish Path";
			// 
			// _publishPath
			// 
			this._publishPath.Location = new System.Drawing.Point(12, 185);
			this._publishPath.Name = "_publishPath";
			this._publishPath.ReadOnly = true;
			this._publishPath.Size = new System.Drawing.Size(317, 20);
			this._publishPath.TabIndex = 4;
			// 
			// _product
			// 
			this._product.FormattingEnabled = true;
			this._product.Location = new System.Drawing.Point(12, 25);
			this._product.Name = "_product";
			this._product.Size = new System.Drawing.Size(195, 21);
			this._product.TabIndex = 0;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(9, 61);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(42, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Version";
			// 
			// _productPathBrowseButton
			// 
			this._productPathBrowseButton.Location = new System.Drawing.Point(335, 133);
			this._productPathBrowseButton.Name = "_productPathBrowseButton";
			this._productPathBrowseButton.Size = new System.Drawing.Size(27, 20);
			this._productPathBrowseButton.TabIndex = 3;
			this._productPathBrowseButton.Text = "...";
			this._productPathBrowseButton.UseVisualStyleBackColor = true;
			// 
			// _publishPathBrowseButton
			// 
			this._publishPathBrowseButton.Location = new System.Drawing.Point(335, 185);
			this._publishPathBrowseButton.Name = "_publishPathBrowseButton";
			this._publishPathBrowseButton.Size = new System.Drawing.Size(27, 20);
			this._publishPathBrowseButton.TabIndex = 5;
			this._publishPathBrowseButton.Text = "...";
			this._publishPathBrowseButton.UseVisualStyleBackColor = true;
			// 
			// _publishButton
			// 
			this._publishButton.Location = new System.Drawing.Point(287, 230);
			this._publishButton.Name = "_publishButton";
			this._publishButton.Size = new System.Drawing.Size(75, 23);
			this._publishButton.TabIndex = 6;
			this._publishButton.Text = "Publish";
			this._publishButton.UseVisualStyleBackColor = true;
			// 
			// _version
			// 
			this._version.FormattingEnabled = true;
			this._version.Location = new System.Drawing.Point(12, 77);
			this._version.Name = "_version";
			this._version.Size = new System.Drawing.Size(195, 21);
			this._version.TabIndex = 1;
			// 
			// PublishForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(379, 278);
			this.Controls.Add(this._version);
			this.Controls.Add(this._publishButton);
			this.Controls.Add(this._publishPathBrowseButton);
			this.Controls.Add(this._productPathBrowseButton);
			this.Controls.Add(this.label4);
			this.Controls.Add(this._product);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._publishPath);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._productsPath);
			this.Name = "PublishForm";
			this.Text = "ClearCanvas ClickOnce Publisher";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _productsPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _publishPath;
		private System.Windows.Forms.ComboBox _product;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button _productPathBrowseButton;
		private System.Windows.Forms.Button _publishPathBrowseButton;
		private System.Windows.Forms.Button _publishButton;
		private System.Windows.Forms.ComboBox _version;
	}
}

