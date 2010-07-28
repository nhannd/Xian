namespace ClearCanvas.Desktop.View.WinForms
{
    partial class TimestampField
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
            this._datePicker = new System.Windows.Forms.DateTimePicker();
            this._timePicker = new System.Windows.Forms.DateTimePicker();
            this.label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _datePicker
            // 
            this._datePicker.Location = new System.Drawing.Point(3, 18);
            this._datePicker.Name = "_datePicker";
            this._datePicker.ShowCheckBox = true;
            this._datePicker.Size = new System.Drawing.Size(144, 20);
            this._datePicker.TabIndex = 0;
            this._datePicker.ValueChanged += new System.EventHandler(this.OnValueChanged);
            this._datePicker.MouseUp += new System.Windows.Forms.MouseEventHandler(this.OnDatePickerMouseUp);
            // 
            // _timePicker
            // 
            this._timePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this._timePicker.Location = new System.Drawing.Point(153, 18);
            this._timePicker.Name = "_timePicker";
            this._timePicker.ShowUpDown = true;
            this._timePicker.Size = new System.Drawing.Size(88, 20);
            this._timePicker.TabIndex = 1;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(3, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(29, 13);
            this.label.TabIndex = 2;
            this.label.Text = "label";
            // 
            // TimestampField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._timePicker);
            this.Controls.Add(this.label);
            this.Controls.Add(this._datePicker);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TimestampField";
            this.Size = new System.Drawing.Size(243, 41);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker _datePicker;
        private System.Windows.Forms.DateTimePicker _timePicker;
        private System.Windows.Forms.Label label;
    }
}
