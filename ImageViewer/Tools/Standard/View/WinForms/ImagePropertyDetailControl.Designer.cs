namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	partial class ImagePropertyDetailControl
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
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._description = new System.Windows.Forms.Label();
			this._richText = new System.Windows.Forms.RichTextBox();
			this._name = new System.Windows.Forms.Label();
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.ColumnCount = 1;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.Controls.Add(this._description, 0, 2);
			this._tableLayoutPanel.Controls.Add(this._richText, 0, 1);
			this._tableLayoutPanel.Controls.Add(this._name, 0, 0);
			this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 3;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 19F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
			this._tableLayoutPanel.Size = new System.Drawing.Size(339, 138);
			this._tableLayoutPanel.TabIndex = 0;
			// 
			// _description
			// 
			this._description.AutoEllipsis = true;
			this._description.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this._description.Location = new System.Drawing.Point(3, 101);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(333, 37);
			this._description.TabIndex = 6;
			this._description.Text = "label1";
			// 
			// _richText
			// 
			this._richText.DetectUrls = false;
			this._richText.Dock = System.Windows.Forms.DockStyle.Fill;
			this._richText.Location = new System.Drawing.Point(3, 22);
			this._richText.Name = "_richText";
			this._richText.ReadOnly = true;
			this._richText.Size = new System.Drawing.Size(333, 76);
			this._richText.TabIndex = 4;
			this._richText.Text = "";
			// 
			// _name
			// 
			this._name.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._name.AutoEllipsis = true;
			this._name.AutoSize = true;
			this._name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._name.Location = new System.Drawing.Point(3, 6);
			this._name.Name = "_name";
			this._name.Size = new System.Drawing.Size(41, 13);
			this._name.TabIndex = 5;
			this._name.Text = "label2";
			// 
			// ImagePropertyDetailControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._tableLayoutPanel);
			this.Name = "ImagePropertyDetailControl";
			this.Size = new System.Drawing.Size(339, 138);
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private System.Windows.Forms.Label _name;
		private System.Windows.Forms.Label _description;
		private System.Windows.Forms.RichTextBox _richText;



	}
}
