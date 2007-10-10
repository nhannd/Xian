namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	partial class LinearPresetVoiLutApplicatorComponentControl
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
			this._nameField = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._windowCenter = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this._windowWidth = new ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this._windowCenter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this._windowWidth)).BeginInit();
			this.SuspendLayout();
			// 
			// _nameField
			// 
			this._nameField.LabelText = "Name";
			this._nameField.Location = new System.Drawing.Point(2, 2);
			this._nameField.Margin = new System.Windows.Forms.Padding(2);
			this._nameField.Mask = "";
			this._nameField.Name = "_nameField";
			this._nameField.Size = new System.Drawing.Size(235, 41);
			this._nameField.TabIndex = 0;
			this._nameField.Value = null;
			// 
			// _windowCenter
			// 
			this._windowCenter.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this._windowCenter.Location = new System.Drawing.Point(141, 74);
			this._windowCenter.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
			this._windowCenter.Minimum = new decimal(new int[] {
            65536,
            0,
            0,
            -2147483648});
			this._windowCenter.Name = "_windowCenter";
			this._windowCenter.Size = new System.Drawing.Size(96, 20);
			this._windowCenter.TabIndex = 2;
			this._windowCenter.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(58, 50);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Window Width";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(58, 76);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Window Center";
			// 
			// _windowWidth
			// 
			this._windowWidth.CausesValidation = false;
			this._windowWidth.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this._windowWidth.Location = new System.Drawing.Point(141, 48);
			this._windowWidth.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
			this._windowWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._windowWidth.Name = "_windowWidth";
			this._windowWidth.Size = new System.Drawing.Size(96, 20);
			this._windowWidth.TabIndex = 1;
			this._windowWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._windowWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// LinearPresetVoiLutApplicatorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._windowCenter);
			this.Controls.Add(this._windowWidth);
			this.Controls.Add(this._nameField);
			this.Name = "LinearPresetVoiLutApplicatorComponentControl";
			this.Size = new System.Drawing.Size(240, 107);
			((System.ComponentModel.ISupportInitialize)(this._windowCenter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this._windowWidth)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _nameField;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _windowCenter;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private ClearCanvas.Desktop.View.WinForms.NonEmptyNumericUpDown _windowWidth;
	}
}
