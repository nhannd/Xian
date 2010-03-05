namespace ClearCanvas.Desktop.Configuration.View.WinForms {
	partial class ToolbarConfigurationComponentControl {
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
			this._flowToolbarPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._toolStripSizePanel = new System.Windows.Forms.Panel();
			this._toolbarSize = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this._wrapPanel = new System.Windows.Forms.Panel();
			this._wrapToolbars = new System.Windows.Forms.CheckBox();
			this._flowToolbarPanel.SuspendLayout();
			this._toolStripSizePanel.SuspendLayout();
			this._wrapPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _flowToolbarPanel
			// 
			this._flowToolbarPanel.Controls.Add(this._toolStripSizePanel);
			this._flowToolbarPanel.Controls.Add(this._wrapPanel);
			this._flowToolbarPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._flowToolbarPanel.Location = new System.Drawing.Point(0, 0);
			this._flowToolbarPanel.Name = "_flowToolbarPanel";
			this._flowToolbarPanel.Padding = new System.Windows.Forms.Padding(15);
			this._flowToolbarPanel.Size = new System.Drawing.Size(379, 180);
			this._flowToolbarPanel.TabIndex = 0;
			// 
			// _toolStripSizePanel
			// 
			this._toolStripSizePanel.Controls.Add(this._toolbarSize);
			this._toolStripSizePanel.Controls.Add(this.label1);
			this._toolStripSizePanel.Location = new System.Drawing.Point(18, 18);
			this._toolStripSizePanel.Name = "_toolStripSizePanel";
			this._toolStripSizePanel.Size = new System.Drawing.Size(344, 42);
			this._toolStripSizePanel.TabIndex = 0;
			// 
			// _toolbarSize
			// 
			this._toolbarSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._toolbarSize.FormattingEnabled = true;
			this._toolbarSize.Items.AddRange(new object[] {
            "Small",
            "Medium",
            "Large"});
			this._toolbarSize.Location = new System.Drawing.Point(4, 17);
			this._toolbarSize.Name = "_toolbarSize";
			this._toolbarSize.Size = new System.Drawing.Size(121, 21);
			this._toolbarSize.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Icon size:";
			// 
			// _wrapPanel
			// 
			this._wrapPanel.Controls.Add(this._wrapToolbars);
			this._wrapPanel.Location = new System.Drawing.Point(18, 66);
			this._wrapPanel.Name = "_wrapPanel";
			this._wrapPanel.Size = new System.Drawing.Size(344, 23);
			this._wrapPanel.TabIndex = 1;
			// 
			// _wrapToolbars
			// 
			this._wrapToolbars.AutoSize = true;
			this._wrapToolbars.Location = new System.Drawing.Point(3, 3);
			this._wrapToolbars.Name = "_wrapToolbars";
			this._wrapToolbars.Size = new System.Drawing.Size(115, 17);
			this._wrapToolbars.TabIndex = 0;
			this._wrapToolbars.Text = "Wrap long toolbars";
			this._wrapToolbars.UseVisualStyleBackColor = true;
			// 
			// ToolbarConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._flowToolbarPanel);
			this.Name = "ToolbarConfigurationComponentControl";
			this.Size = new System.Drawing.Size(379, 180);
			this._flowToolbarPanel.ResumeLayout(false);
			this._toolStripSizePanel.ResumeLayout(false);
			this._toolStripSizePanel.PerformLayout();
			this._wrapPanel.ResumeLayout(false);
			this._wrapPanel.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox _wrapToolbars;
		private System.Windows.Forms.Panel _wrapPanel;
		private System.Windows.Forms.Panel _toolStripSizePanel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox _toolbarSize;
		private System.Windows.Forms.FlowLayoutPanel _flowToolbarPanel;
	}
}
