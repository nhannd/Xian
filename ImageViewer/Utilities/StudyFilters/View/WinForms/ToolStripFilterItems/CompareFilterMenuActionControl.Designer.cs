namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems {
	partial class CompareFilterMenuActionControl {
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label spacer;
			System.Windows.Forms.Panel valigner;
			this._txtValue = new System.Windows.Forms.TextBox();
			this._modeToggle = new System.Windows.Forms.Button();
			this._errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this._tooltipProvider = new System.Windows.Forms.ToolTip(this.components);
			spacer = new System.Windows.Forms.Label();
			valigner = new System.Windows.Forms.Panel();
			valigner.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// spacer
			// 
			spacer.Dock = System.Windows.Forms.DockStyle.Left;
			spacer.Location = new System.Drawing.Point(29, 3);
			spacer.Name = "spacer";
			spacer.Size = new System.Drawing.Size(3, 26);
			spacer.TabIndex = 2;
			// 
			// valigner
			// 
			valigner.Controls.Add(this._txtValue);
			valigner.Dock = System.Windows.Forms.DockStyle.Fill;
			valigner.Location = new System.Drawing.Point(32, 3);
			valigner.Name = "valigner";
			valigner.Size = new System.Drawing.Size(172, 26);
			valigner.TabIndex = 3;
			// 
			// _txtValue
			// 
			this._txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this._txtValue.Location = new System.Drawing.Point(0, 3);
			this._txtValue.Name = "_txtValue";
			this._txtValue.Size = new System.Drawing.Size(172, 20);
			this._txtValue.TabIndex = 2;
			this._txtValue.TextChanged += new System.EventHandler(this._txtValue_TextChanged);
			// 
			// _modeToggle
			// 
			this._modeToggle.Dock = System.Windows.Forms.DockStyle.Left;
			this._modeToggle.Location = new System.Drawing.Point(3, 3);
			this._modeToggle.Name = "_modeToggle";
			this._modeToggle.Size = new System.Drawing.Size(26, 26);
			this._modeToggle.TabIndex = 0;
			this._modeToggle.UseVisualStyleBackColor = true;
			this._modeToggle.Click += new System.EventHandler(this.modeToggle_Click);
			// 
			// _errorProvider
			// 
			this._errorProvider.ContainerControl = this;
			// 
			// CompareFilterMenuActionControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(valigner);
			this.Controls.Add(spacer);
			this.Controls.Add(this._modeToggle);
			this.Name = "CompareFilterMenuActionControl";
			this.Padding = new System.Windows.Forms.Padding(3);
			this.Size = new System.Drawing.Size(207, 32);
			valigner.ResumeLayout(false);
			valigner.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button _modeToggle;
		private System.Windows.Forms.TextBox _txtValue;
		private System.Windows.Forms.ErrorProvider _errorProvider;
		private System.Windows.Forms.ToolTip _tooltipProvider;
	}
}
