namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class WorklistSummaryComponentControl
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
            this._worklistTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _worklistTable
            // 
            this._worklistTable.AutoSize = true;
            this._worklistTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._worklistTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._worklistTable.Location = new System.Drawing.Point(0, 0);
            this._worklistTable.MenuModel = null;
            this._worklistTable.Name = "_worklistTable";
            this._worklistTable.ReadOnly = false;
            this._worklistTable.Selection = selection1;
            this._worklistTable.Size = new System.Drawing.Size(235, 288);
            this._worklistTable.TabIndex = 2;
            this._worklistTable.Table = null;
            this._worklistTable.ToolbarModel = null;
            this._worklistTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._worklistTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._worklistTable.ItemDoubleClicked += new System.EventHandler(this._worklistTable_ItemDoubleClicked);
            // 
            // WorklistSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._worklistTable);
            this.Name = "WorklistSummaryComponentControl";
            this.Size = new System.Drawing.Size(235, 288);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _worklistTable;

    }
}
