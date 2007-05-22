namespace ClearCanvas.Ris.Client.View.WinForms
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            this._phoneNumbers = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _phoneNumbers
            // 
            this._phoneNumbers.AutoSize = true;
            this._phoneNumbers.Dock = System.Windows.Forms.DockStyle.Fill;
            this._phoneNumbers.Location = new System.Drawing.Point(0, 0);
            this._phoneNumbers.Margin = new System.Windows.Forms.Padding(4, 4, 19, 4);
            this._phoneNumbers.MenuModel = null;
            this._phoneNumbers.MultiSelect = false;
            this._phoneNumbers.Name = "_phoneNumbers";
            this._phoneNumbers.ReadOnly = false;
            this._phoneNumbers.Selection = selection1;
            this._phoneNumbers.Size = new System.Drawing.Size(149, 114);
            this._phoneNumbers.TabIndex = 2;
            this._phoneNumbers.Table = null;
            this._phoneNumbers.ToolbarModel = null;
            this._phoneNumbers.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._phoneNumbers.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // PhoneNumbersSummaryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._phoneNumbers);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "PhoneNumbersSummaryControl";
            this.Size = new System.Drawing.Size(149, 114);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _phoneNumbers;

    }
}
