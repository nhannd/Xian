namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    partial class LocalDataStoreReindexApplicationComponentControl
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
			this._reindexProgressControl = new ClearCanvas.ImageViewer.Services.Tools.View.WinForms.ImportProgressControl();
			this.SuspendLayout();
			// 
			// _reindexProgressControl
			// 
			this._reindexProgressControl.AutoSize = true;
			this._reindexProgressControl.AvailableCount = 0;
			this._reindexProgressControl.CancelEnabled = true;
			this._reindexProgressControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reindexProgressControl.FailedSteps = 0;
			this._reindexProgressControl.Location = new System.Drawing.Point(0, 0);
			this._reindexProgressControl.Name = "_reindexProgressControl";
			this._reindexProgressControl.Size = new System.Drawing.Size(777, 201);
			this._reindexProgressControl.StatusMessage = "Status:";
			this._reindexProgressControl.TabIndex = 10;
			this._reindexProgressControl.TotalProcessed = 0;
			this._reindexProgressControl.TotalToProcess = 100;
			// 
			// LocalDataStoreReindexApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._reindexProgressControl);
			this.Name = "LocalDataStoreReindexApplicationComponentControl";
			this.Size = new System.Drawing.Size(777, 201);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ImportProgressControl _reindexProgressControl;


	}
}
