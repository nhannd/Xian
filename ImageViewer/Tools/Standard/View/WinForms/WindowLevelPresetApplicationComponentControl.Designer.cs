namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    partial class WindowLevelPresetApplicationComponentControl
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
			this._name = new ClearCanvas.Controls.WinForms.TextField();
			this._window = new ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown();
			this._level = new ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._comboKey = new ClearCanvas.Controls.WinForms.ComboBoxField();
			this._ok = new System.Windows.Forms.Button();
			this._cancel = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._window)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._level)).BeginInit();
			this.SuspendLayout();
			// 
			// _name
			// 
			this._name.LabelText = "Description";
			this._name.Location = new System.Drawing.Point(89, 13);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.Size = new System.Drawing.Size(150, 41);
			this._name.TabIndex = 1;
			this._name.Value = null;
			// 
			// _window
			// 
			this._window.Location = new System.Drawing.Point(244, 31);
			this._window.Name = "_window";
			this._window.Size = new System.Drawing.Size(75, 20);
			this._window.TabIndex = 2;
			// 
			// _level
			// 
			this._level.Location = new System.Drawing.Point(325, 31);
			this._level.Name = "_level";
			this._level.Size = new System.Drawing.Size(75, 20);
			this._level.TabIndex = 3;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(245, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Window";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(325, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(33, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Level";
			// 
			// _comboKey
			// 
			this._comboKey.DataSource = null;
			this._comboKey.DisplayMember = "";
			this._comboKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboKey.LabelText = "Key";
			this._comboKey.Location = new System.Drawing.Point(4, 13);
			this._comboKey.Margin = new System.Windows.Forms.Padding(2);
			this._comboKey.Name = "_comboKey";
			this._comboKey.Size = new System.Drawing.Size(81, 41);
			this._comboKey.TabIndex = 0;
			this._comboKey.Value = null;
			// 
			// _ok
			// 
			this._ok.Location = new System.Drawing.Point(244, 66);
			this._ok.Name = "_ok";
			this._ok.Size = new System.Drawing.Size(75, 23);
			this._ok.TabIndex = 5;
			this._ok.Text = "OK";
			this._ok.UseVisualStyleBackColor = true;
			// 
			// _cancel
			// 
			this._cancel.Location = new System.Drawing.Point(325, 66);
			this._cancel.Name = "_cancel";
			this._cancel.Size = new System.Drawing.Size(75, 23);
			this._cancel.TabIndex = 6;
			this._cancel.Text = "Cancel";
			this._cancel.UseVisualStyleBackColor = true;
			// 
			// WindowLevelPresetApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._cancel);
			this.Controls.Add(this._ok);
			this.Controls.Add(this._comboKey);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._level);
			this.Controls.Add(this._window);
			this.Controls.Add(this._name);
			this.Name = "WindowLevelPresetApplicationComponentControl";
			this.Size = new System.Drawing.Size(413, 104);
			((System.ComponentModel.ISupportInitialize)(this._window)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._level)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Controls.WinForms.TextField _name;
		private ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown _window;
		private ClearCanvas.Controls.WinForms.NonEmptyNumericUpDown _level;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private ClearCanvas.Controls.WinForms.ComboBoxField _comboKey;
		private System.Windows.Forms.Button _ok;
		private System.Windows.Forms.Button _cancel;
    }
}
