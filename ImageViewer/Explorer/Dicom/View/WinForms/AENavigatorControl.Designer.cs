namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	partial class AENavigatorControl
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
            this._aeserverTreeForm1 = new ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms.AEServerTreeForm();
            this.SuspendLayout();
            // 
            // _aeserverTreeForm1
            // 
            this._aeserverTreeForm1.Location = new System.Drawing.Point(4, 15);
            this._aeserverTreeForm1.Name = "_aeserverTreeForm1";
            this._aeserverTreeForm1.Size = new System.Drawing.Size(244, 388);
            this._aeserverTreeForm1.TabIndex = 0;
            // 
            // AENavigatorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this._aeserverTreeForm1);
            this.Name = "AENavigatorControl";
            this.Size = new System.Drawing.Size(399, 492);
            this.ResumeLayout(false);

		}

		#endregion

        private AEServerTreeForm _aeserverTreeForm1;


    }
}
