namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class EmailAddressesSummaryComponentControl
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
            this._emailAddressList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _emailAddressList
            // 
            this._emailAddressList.AutoSize = true;
            this._emailAddressList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._emailAddressList.Location = new System.Drawing.Point(0, 0);
            this._emailAddressList.MenuModel = null;
            this._emailAddressList.Name = "_emailAddressList";
            this._emailAddressList.ReadOnly = false;
            this._emailAddressList.Selection = selection1;
            this._emailAddressList.Size = new System.Drawing.Size(206, 156);
            this._emailAddressList.TabIndex = 0;
            this._emailAddressList.Table = null;
            this._emailAddressList.ToolbarModel = null;
            this._emailAddressList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._emailAddressList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._emailAddressList.ItemDoubleClicked += new System.EventHandler(this._emailAddressList_ItemDoubleClicked);
            // 
            // EmailAddressesSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._emailAddressList);
            this.Name = "EmailAddressesSummaryComponentControl";
            this.Size = new System.Drawing.Size(206, 156);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _emailAddressList;
    }
}
