namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class ContactPersonsSummaryComponentControl
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
            this._contactPersonList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _contactPersonList
            // 
            this._contactPersonList.AutoSize = true;
            this._contactPersonList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._contactPersonList.Location = new System.Drawing.Point(0, 0);
            this._contactPersonList.MenuModel = null;
            this._contactPersonList.Name = "_contactPersonList";
            this._contactPersonList.ReadOnly = false;
            this._contactPersonList.Selection = selection1;
            this._contactPersonList.Size = new System.Drawing.Size(206, 156);
            this._contactPersonList.TabIndex = 0;
            this._contactPersonList.Table = null;
            this._contactPersonList.ToolbarModel = null;
            this._contactPersonList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._contactPersonList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._contactPersonList.ItemDoubleClicked += new System.EventHandler(this._contactPersonList_ItemDoubleClicked);
            // 
            // ContactPersonsSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._contactPersonList);
            this.Name = "ContactPersonsSummaryComponentControl";
            this.Size = new System.Drawing.Size(206, 156);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _contactPersonList;
    }
}
