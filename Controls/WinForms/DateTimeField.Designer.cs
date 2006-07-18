namespace ClearCanvas.Controls.WinForms
{
    partial class DateTimeField
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
            this._dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this._label = new System.Windows.Forms.Label();
            this._checkBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _dateTimePicker
            // 
            this._dateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._dateTimePicker.Location = new System.Drawing.Point(3, 25);
            this._dateTimePicker.Name = "_dateTimePicker";
            this._dateTimePicker.Size = new System.Drawing.Size(194, 22);
            this._dateTimePicker.TabIndex = 0;
            // 
            // _label
            // 
            this._label.AutoSize = true;
            this._label.Location = new System.Drawing.Point(3, 4);
            this._label.Name = "_label";
            this._label.Size = new System.Drawing.Size(38, 17);
            this._label.TabIndex = 1;
            this._label.Text = "label";
            // 
            // _checkBox
            // 
            this._checkBox.AutoSize = true;
            this._checkBox.Location = new System.Drawing.Point(6, 3);
            this._checkBox.Name = "_checkBox";
            this._checkBox.Size = new System.Drawing.Size(90, 21);
            this._checkBox.TabIndex = 2;
            this._checkBox.Text = "checkBox";
            this._checkBox.UseVisualStyleBackColor = true;
            // 
            // DateTimeField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._label);
            this.Controls.Add(this._checkBox);
            this.Controls.Add(this._dateTimePicker);
            this.Name = "DateTimeField";
            this.Size = new System.Drawing.Size(200, 50);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker _dateTimePicker;
        private System.Windows.Forms.Label _label;
        private System.Windows.Forms.CheckBox _checkBox;
    }
}
