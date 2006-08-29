namespace ClearCanvas.Ris.Client.Calendar.View.WinForms
{
    partial class CalendarSearchControl
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
            this._fromDate = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._untilDate = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._searchButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _fromDate
            // 
            this._fromDate.LabelText = "From";
            this._fromDate.Location = new System.Drawing.Point(8, 18);
            this._fromDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._fromDate.Name = "_fromDate";
            this._fromDate.Nullable = true;
            this._fromDate.Size = new System.Drawing.Size(200, 50);
            this._fromDate.TabIndex = 0;
            this._fromDate.Value = null;
            // 
            // _untilDate
            // 
            this._untilDate.LabelText = "Until";
            this._untilDate.Location = new System.Drawing.Point(8, 81);
            this._untilDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._untilDate.Name = "_untilDate";
            this._untilDate.Nullable = true;
            this._untilDate.Size = new System.Drawing.Size(200, 50);
            this._untilDate.TabIndex = 1;
            this._untilDate.Value = null;
            // 
            // _searchButton
            // 
            this._searchButton.Location = new System.Drawing.Point(123, 146);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(75, 23);
            this._searchButton.TabIndex = 2;
            this._searchButton.Text = "Search";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
            // 
            // CalendarSearchControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._searchButton);
            this.Controls.Add(this._untilDate);
            this.Controls.Add(this._fromDate);
            this.Name = "CalendarSearchControl";
            this.Size = new System.Drawing.Size(211, 193);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Controls.WinForms.DateTimeField _fromDate;
        private ClearCanvas.Controls.WinForms.DateTimeField _untilDate;
        private System.Windows.Forms.Button _searchButton;
    }
}
