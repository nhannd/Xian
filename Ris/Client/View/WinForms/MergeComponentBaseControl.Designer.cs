namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class MergeComponentBaseControl
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
			this._duplicateGroupBox = new System.Windows.Forms.GroupBox();
			this._report = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this._switchButton = new System.Windows.Forms.Button();
			this._originalLookupField = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._duplicateLookupField = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._duplicateGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _duplicateGroupBox
			// 
			this._duplicateGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._duplicateGroupBox.Controls.Add(this._report);
			this._duplicateGroupBox.Controls.Add(this._switchButton);
			this._duplicateGroupBox.Controls.Add(this._originalLookupField);
			this._duplicateGroupBox.Controls.Add(this._duplicateLookupField);
			this._duplicateGroupBox.Location = new System.Drawing.Point(3, 3);
			this._duplicateGroupBox.Name = "_duplicateGroupBox";
			this._duplicateGroupBox.Size = new System.Drawing.Size(272, 360);
			this._duplicateGroupBox.TabIndex = 0;
			this._duplicateGroupBox.TabStop = false;
			this._duplicateGroupBox.Text = "Select items to merge:";
			// 
			// _report
			// 
			this._report.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._report.LabelText = "Proposed Changes:";
			this._report.Location = new System.Drawing.Point(7, 146);
			this._report.Margin = new System.Windows.Forms.Padding(2);
			this._report.Name = "_report";
			this._report.ReadOnly = true;
			this._report.Size = new System.Drawing.Size(248, 209);
			this._report.TabIndex = 3;
			this._report.Value = null;
			// 
			// _switchButton
			// 
			this._switchButton.Location = new System.Drawing.Point(89, 70);
			this._switchButton.Name = "_switchButton";
			this._switchButton.Size = new System.Drawing.Size(63, 23);
			this._switchButton.TabIndex = 1;
			this._switchButton.Text = "Switch";
			this._switchButton.UseVisualStyleBackColor = true;
			this._switchButton.Click += new System.EventHandler(this._switchButton_Click);
			// 
			// _originalLookupField
			// 
			this._originalLookupField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._originalLookupField.LabelText = "With this:";
			this._originalLookupField.Location = new System.Drawing.Point(7, 90);
			this._originalLookupField.Margin = new System.Windows.Forms.Padding(0);
			this._originalLookupField.Name = "_originalLookupField";
			this._originalLookupField.Size = new System.Drawing.Size(248, 41);
			this._originalLookupField.TabIndex = 2;
			this._originalLookupField.Value = null;
			// 
			// _duplicateLookupField
			// 
			this._duplicateLookupField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._duplicateLookupField.LabelText = "Replace this:";
			this._duplicateLookupField.Location = new System.Drawing.Point(7, 20);
			this._duplicateLookupField.Margin = new System.Windows.Forms.Padding(0);
			this._duplicateLookupField.Name = "_duplicateLookupField";
			this._duplicateLookupField.Size = new System.Drawing.Size(248, 41);
			this._duplicateLookupField.TabIndex = 0;
			this._duplicateLookupField.Value = null;
			// 
			// _acceptButton
			// 
			this._acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._acceptButton.Location = new System.Drawing.Point(119, 369);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 1;
			this._acceptButton.Text = "Merge";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(200, 369);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 2;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// MergeComponentBaseControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._duplicateGroupBox);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Name = "MergeComponentBaseControl";
			this.Size = new System.Drawing.Size(281, 400);
			this._duplicateGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.GroupBox _duplicateGroupBox;
		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.Button _cancelButton;
		private ClearCanvas.Ris.Client.View.WinForms.LookupField _originalLookupField;
		private ClearCanvas.Ris.Client.View.WinForms.LookupField _duplicateLookupField;
		private System.Windows.Forms.Button _switchButton;
		private ClearCanvas.Desktop.View.WinForms.TextAreaField _report;
    }
}
