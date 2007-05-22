namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class AddressesSummaryControl
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
            this._addressList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _addressList
            // 
            this._addressList.AutoSize = true;
            this._addressList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._addressList.Location = new System.Drawing.Point(0, 0);
            this._addressList.Margin = new System.Windows.Forms.Padding(4, 4, 19, 4);
            this._addressList.MenuModel = null;
            this._addressList.MultiLine = true;
            this._addressList.MultiSelect = false;
            this._addressList.Name = "_addressList";
            this._addressList.ReadOnly = false;
            this._addressList.Selection = selection1;
            this._addressList.Size = new System.Drawing.Size(129, 118);
            this._addressList.TabIndex = 2;
            this._addressList.Table = null;
            this._addressList.ToolbarModel = null;
            this._addressList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._addressList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // AddressesSummaryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._addressList);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "AddressesSummaryControl";
            this.Size = new System.Drawing.Size(129, 118);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _addressList;

    }
}
