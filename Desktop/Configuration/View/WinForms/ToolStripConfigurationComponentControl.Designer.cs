namespace ClearCanvas.Desktop.Configuration.View.WinForms {
	partial class ToolStripConfigurationComponentControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.Windows.Forms.FlowLayoutPanel _flowGlobalToolbars;
			this._toolStripSizePanel = new System.Windows.Forms.Panel();
			this._toolStripSizes = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this._wrapPanel = new System.Windows.Forms.Panel();
			this._chkWrapGlobalToolbars = new System.Windows.Forms.CheckBox();
			_flowGlobalToolbars = new System.Windows.Forms.FlowLayoutPanel();
			_flowGlobalToolbars.SuspendLayout();
			this._toolStripSizePanel.SuspendLayout();
			this._wrapPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _flowGlobalToolbars
			// 
			_flowGlobalToolbars.Controls.Add(this._toolStripSizePanel);
			_flowGlobalToolbars.Controls.Add(this._wrapPanel);
			_flowGlobalToolbars.Dock = System.Windows.Forms.DockStyle.Fill;
			_flowGlobalToolbars.Location = new System.Drawing.Point(0, 0);
			_flowGlobalToolbars.Name = "_flowGlobalToolbars";
			_flowGlobalToolbars.Padding = new System.Windows.Forms.Padding(15);
			_flowGlobalToolbars.Size = new System.Drawing.Size(379, 180);
			_flowGlobalToolbars.TabIndex = 0;
			// 
			// _toolStripSizePanel
			// 
			this._toolStripSizePanel.Controls.Add(this._toolStripSizes);
			this._toolStripSizePanel.Controls.Add(this.label1);
			this._toolStripSizePanel.Location = new System.Drawing.Point(18, 18);
			this._toolStripSizePanel.Name = "_toolStripSizePanel";
			this._toolStripSizePanel.Size = new System.Drawing.Size(344, 42);
			this._toolStripSizePanel.TabIndex = 0;
			// 
			// _toolStripSizes
			// 
			this._toolStripSizes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._toolStripSizes.FormattingEnabled = true;
			this._toolStripSizes.Items.AddRange(new object[] {
            "Small",
            "Medium",
            "Large"});
			this._toolStripSizes.Location = new System.Drawing.Point(4, 17);
			this._toolStripSizes.Name = "_toolStripSizes";
			this._toolStripSizes.Size = new System.Drawing.Size(121, 21);
			this._toolStripSizes.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Toolbar Size:";
			// 
			// _wrapPanel
			// 
			this._wrapPanel.Controls.Add(this._chkWrapGlobalToolbars);
			this._wrapPanel.Location = new System.Drawing.Point(18, 66);
			this._wrapPanel.Name = "_wrapPanel";
			this._wrapPanel.Size = new System.Drawing.Size(344, 23);
			this._wrapPanel.TabIndex = 1;
			// 
			// _chkWrapGlobalToolbars
			// 
			this._chkWrapGlobalToolbars.AutoSize = true;
			this._chkWrapGlobalToolbars.Location = new System.Drawing.Point(3, 3);
			this._chkWrapGlobalToolbars.Name = "_chkWrapGlobalToolbars";
			this._chkWrapGlobalToolbars.Size = new System.Drawing.Size(147, 17);
			this._chkWrapGlobalToolbars.TabIndex = 0;
			this._chkWrapGlobalToolbars.Text = "Wrap toolbars horizontally";
			this._chkWrapGlobalToolbars.UseVisualStyleBackColor = true;
			// 
			// ToolStripConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(_flowGlobalToolbars);
			this.Name = "ToolStripConfigurationComponentControl";
			this.Size = new System.Drawing.Size(379, 180);
			_flowGlobalToolbars.ResumeLayout(false);
			this._toolStripSizePanel.ResumeLayout(false);
			this._toolStripSizePanel.PerformLayout();
			this._wrapPanel.ResumeLayout(false);
			this._wrapPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkWrapGlobalToolbars;
		private System.Windows.Forms.Panel _wrapPanel;
		private System.Windows.Forms.Panel _toolStripSizePanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox _toolStripSizes;
	}
}
