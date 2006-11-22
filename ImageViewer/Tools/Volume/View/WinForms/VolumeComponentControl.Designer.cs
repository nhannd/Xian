namespace ClearCanvas.ImageViewer.Tools.Volume.View.WinForms
{
    partial class VolumeComponentControl
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
			this.CreateVolumeButton = new System.Windows.Forms.Button();
			this._tabControl = new System.Windows.Forms.TabControl();
			this.SuspendLayout();
			// 
			// CreateVolumeButton
			// 
			this.CreateVolumeButton.AutoSize = true;
			this.CreateVolumeButton.Location = new System.Drawing.Point(14, 13);
			this.CreateVolumeButton.Name = "CreateVolumeButton";
			this.CreateVolumeButton.Size = new System.Drawing.Size(111, 28);
			this.CreateVolumeButton.TabIndex = 0;
			this.CreateVolumeButton.Text = "Create Volume";
			this.CreateVolumeButton.UseVisualStyleBackColor = true;
			// 
			// _tabControl
			// 
			this._tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._tabControl.Location = new System.Drawing.Point(14, 61);
			this._tabControl.Name = "_tabControl";
			this._tabControl.SelectedIndex = 0;
			this._tabControl.Size = new System.Drawing.Size(385, 344);
			this._tabControl.TabIndex = 1;
			// 
			// VolumeComponentControl
			// 
			this.AutoSize = true;
			this.Controls.Add(this._tabControl);
			this.Controls.Add(this.CreateVolumeButton);
			this.Name = "VolumeComponentControl";
			this.Size = new System.Drawing.Size(422, 423);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button CreateVolumeButton;
		private System.Windows.Forms.TabControl _tabControl;
    }
}
