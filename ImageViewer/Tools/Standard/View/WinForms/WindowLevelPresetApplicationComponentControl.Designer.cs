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
			((System.ComponentModel.ISupportInitialize)(this._window)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._level)).BeginInit();
			this.SuspendLayout();
			// 
			// _name
			// 
			this._name.LabelText = "Description";
			this._name.Location = new System.Drawing.Point(15, 15);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.Size = new System.Drawing.Size(150, 41);
			this._name.TabIndex = 0;
			this._name.Value = null;
			// 
			// _window
			// 
			this._window.Location = new System.Drawing.Point(170, 33);
			this._window.Name = "_window";
			this._window.Size = new System.Drawing.Size(75, 20);
			this._window.TabIndex = 1;
			// 
			// _level
			// 
			this._level.Location = new System.Drawing.Point(251, 33);
			this._level.Name = "_level";
			this._level.Size = new System.Drawing.Size(75, 20);
			this._level.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(171, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(46, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Window";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(251, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(33, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Level";
			// 
			// WindowLevelPresetApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._level);
			this.Controls.Add(this._window);
			this.Controls.Add(this._name);
			this.Name = "WindowLevelPresetApplicationComponentControl";
			this.Size = new System.Drawing.Size(343, 69);
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
    }
}
