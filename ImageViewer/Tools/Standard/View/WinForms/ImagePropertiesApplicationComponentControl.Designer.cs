using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	partial class ImagePropertiesApplicationComponentControl
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
			this._properties = new ClearCanvas.ImageViewer.Tools.Standard.View.WinForms.CustomPropertyGrid();
			this.SuspendLayout();
			// 
			// _properties
			// 
			this._properties.BackColor = System.Drawing.SystemColors.ControlDarkDark;
			this._properties.CategoryForeColor = System.Drawing.SystemColors.ControlLightLight;
			this._properties.CommandsVisibleIfAvailable = false;
			this._properties.Dock = System.Windows.Forms.DockStyle.Fill;
			this._properties.Location = new System.Drawing.Point(0, 0);
			this._properties.Name = "_properties";
			this._properties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this._properties.Size = new System.Drawing.Size(443, 644);
			this._properties.TabIndex = 0;
			this._properties.ToolbarVisible = false;
			this._properties.ViewBackColor = System.Drawing.SystemColors.ControlDarkDark;
			this._properties.ViewForeColor = System.Drawing.SystemColors.ControlLightLight;
			// 
			// ImagePropertiesApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._properties);
			this.Name = "ImagePropertiesApplicationComponentControl";
			this.Size = new System.Drawing.Size(443, 644);
			this.ResumeLayout(false);

		}

		#endregion

		private CustomPropertyGrid _properties;
	}
}
