namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	partial class ModalityPicker
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
			this.components = new System.ComponentModel.Container();
			this._showModalityListButton = new System.Windows.Forms.Button();
			this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._modalitiesString = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _showModalityListButton
			// 
			this._showModalityListButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this._showModalityListButton.Location = new System.Drawing.Point(168, 4);
			this._showModalityListButton.Margin = new System.Windows.Forms.Padding(2);
			this._showModalityListButton.Name = "_showModalityListButton";
			this._showModalityListButton.Size = new System.Drawing.Size(25, 18);
			this._showModalityListButton.TabIndex = 2;
			this._showModalityListButton.TabStop = false;
			this._showModalityListButton.UseVisualStyleBackColor = true;
			this._showModalityListButton.Click += new System.EventHandler(this.OnShowModalityListClick);
			// 
			// _contextMenuStrip
			// 
			this._contextMenuStrip.AllowMerge = false;
			this._contextMenuStrip.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
			this._contextMenuStrip.DropShadowEnabled = false;
			this._contextMenuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
			this._contextMenuStrip.Name = "contextMenuStrip1";
			this._contextMenuStrip.ShowImageMargin = false;
			this._contextMenuStrip.ShowItemToolTips = false;
			this._contextMenuStrip.Size = new System.Drawing.Size(128, 26);
			// 
			// _modalitiesString
			// 
			this._modalitiesString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._modalitiesString.AutoEllipsis = true;
			this._modalitiesString.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._modalitiesString.Enabled = false;
			this._modalitiesString.Location = new System.Drawing.Point(3, 4);
			this._modalitiesString.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this._modalitiesString.Name = "_modalitiesString";
			this._modalitiesString.Size = new System.Drawing.Size(161, 18);
			this._modalitiesString.TabIndex = 1;
			// 
			// ModalityPicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._modalitiesString);
			this.Controls.Add(this._showModalityListButton);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "ModalityPicker";
			this.Size = new System.Drawing.Size(195, 28);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _showModalityListButton;
		private System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
		private System.Windows.Forms.Label _modalitiesString;

	}
}
