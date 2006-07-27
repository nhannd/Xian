namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class PhoneNumbersSummaryControl
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
            this.label1 = new System.Windows.Forms.Label();
            this._phoneNumbers = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._addButton = new System.Windows.Forms.Button();
            this._updateButton = new System.Windows.Forms.Button();
            this._deleteButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(94, 126);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Phone Numbers";
            // 
            // _phoneNumbers
            // 
            this._phoneNumbers.DataSource = null;
            this._phoneNumbers.Location = new System.Drawing.Point(27, 36);
            this._phoneNumbers.Name = "_phoneNumbers";
            this._phoneNumbers.Size = new System.Drawing.Size(540, 228);
            this._phoneNumbers.TabIndex = 1;
            this._phoneNumbers.ItemDoubleClicked += new System.EventHandler(this._phoneNumbers_ItemDoubleClicked);
            // 
            // _addButton
            // 
            this._addButton.Location = new System.Drawing.Point(285, 302);
            this._addButton.Name = "_addButton";
            this._addButton.Size = new System.Drawing.Size(75, 23);
            this._addButton.TabIndex = 2;
            this._addButton.Text = "Add...";
            this._addButton.UseVisualStyleBackColor = true;
            this._addButton.Click += new System.EventHandler(this._addButton_Click);
            // 
            // _updateButton
            // 
            this._updateButton.Location = new System.Drawing.Point(388, 302);
            this._updateButton.Name = "_updateButton";
            this._updateButton.Size = new System.Drawing.Size(75, 23);
            this._updateButton.TabIndex = 3;
            this._updateButton.Text = "Update...";
            this._updateButton.UseVisualStyleBackColor = true;
            this._updateButton.Click += new System.EventHandler(this._updateButton_Click);
            // 
            // _deleteButton
            // 
            this._deleteButton.Location = new System.Drawing.Point(492, 302);
            this._deleteButton.Name = "_deleteButton";
            this._deleteButton.Size = new System.Drawing.Size(75, 23);
            this._deleteButton.TabIndex = 4;
            this._deleteButton.Text = "Delete";
            this._deleteButton.UseVisualStyleBackColor = true;
            this._deleteButton.Click += new System.EventHandler(this._deleteButton_Click);
            // 
            // PhoneNumbersSummaryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._deleteButton);
            this.Controls.Add(this._updateButton);
            this.Controls.Add(this._addButton);
            this.Controls.Add(this._phoneNumbers);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "PhoneNumbersSummaryControl";
            this.Size = new System.Drawing.Size(610, 371);
            this.Load += new System.EventHandler(this.PhoneNumbersSummaryControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private ClearCanvas.Desktop.View.WinForms.TableView _phoneNumbers;
        private System.Windows.Forms.Button _addButton;
        private System.Windows.Forms.Button _updateButton;
        private System.Windows.Forms.Button _deleteButton;
    }
}
