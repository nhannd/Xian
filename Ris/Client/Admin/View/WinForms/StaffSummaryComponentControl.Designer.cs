namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class StaffSummaryComponentControl
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
            this._staffs = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _staffs
            // 
            this._staffs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._staffs.Location = new System.Drawing.Point(0, 0);
            this._staffs.MenuModel = null;
            this._staffs.Name = "_staffs";
            this._staffs.ReadOnly = false;
            this._staffs.Selection = selection1;
            this._staffs.Size = new System.Drawing.Size(447, 284);
            this._staffs.TabIndex = 0;
            this._staffs.Table = null;
            this._staffs.ToolbarModel = null;
            this._staffs.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._staffs.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._staffs.Load += new System.EventHandler(this._staffs_Load);
            this._staffs.ItemDoubleClicked += new System.EventHandler(this._staffs_ItemDoubleClicked);
            // 
            // StaffSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._staffs);
            this.Name = "StaffSummaryComponentControl";
            this.Size = new System.Drawing.Size(447, 284);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _staffs;
    }
}
