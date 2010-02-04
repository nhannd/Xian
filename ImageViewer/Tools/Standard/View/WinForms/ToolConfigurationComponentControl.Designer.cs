namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms {
	partial class ToolConfigurationComponentControl {
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
			this._optionsGroup = new System.Windows.Forms.GroupBox();
			this._autoCineMultiframes = new System.Windows.Forms.CheckBox();
			this._modality = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this._optionsGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// _optionsGroup
			// 
			this._optionsGroup.Controls.Add(this._autoCineMultiframes);
			this._optionsGroup.Location = new System.Drawing.Point(16, 56);
			this._optionsGroup.Name = "_optionsGroup";
			this._optionsGroup.Size = new System.Drawing.Size(264, 71);
			this._optionsGroup.TabIndex = 2;
			this._optionsGroup.TabStop = false;
			this._optionsGroup.Text = "Options";
			// 
			// _autoCineMultiframes
			// 
			this._autoCineMultiframes.AutoSize = true;
			this._autoCineMultiframes.Location = new System.Drawing.Point(15, 24);
			this._autoCineMultiframes.Name = "_autoCineMultiframes";
			this._autoCineMultiframes.Size = new System.Drawing.Size(204, 17);
			this._autoCineMultiframes.TabIndex = 0;
			this._autoCineMultiframes.Text = "Automatically start cine on multiframes";
			this._autoCineMultiframes.UseVisualStyleBackColor = true;
			// 
			// _modality
			// 
			this._modality.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._modality.FormattingEnabled = true;
			this._modality.Location = new System.Drawing.Point(16, 29);
			this._modality.MaxDropDownItems = 25;
			this._modality.Name = "_modality";
			this._modality.Size = new System.Drawing.Size(86, 21);
			this._modality.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Modality";
			// 
			// ToolConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._optionsGroup);
			this.Controls.Add(this._modality);
			this.Controls.Add(this.label1);
			this.Name = "ToolConfigurationComponentControl";
			this.Size = new System.Drawing.Size(381, 330);
			this._optionsGroup.ResumeLayout(false);
			this._optionsGroup.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox _optionsGroup;
		private System.Windows.Forms.CheckBox _autoCineMultiframes;
		private System.Windows.Forms.ComboBox _modality;
		private System.Windows.Forms.Label label1;
	}
}
