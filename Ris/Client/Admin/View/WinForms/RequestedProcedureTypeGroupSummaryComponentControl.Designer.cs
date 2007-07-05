namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class RequestedProcedureTypeGroupSummaryComponentControl
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
            this._requestedProcedureTypeGroupTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _requestedProcedureTypeGroupTable
            // 
            this._requestedProcedureTypeGroupTable.AutoSize = true;
            this._requestedProcedureTypeGroupTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._requestedProcedureTypeGroupTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._requestedProcedureTypeGroupTable.Location = new System.Drawing.Point(0, 0);
            this._requestedProcedureTypeGroupTable.MenuModel = null;
            this._requestedProcedureTypeGroupTable.Name = "_requestedProcedureTypeGroupTable";
            this._requestedProcedureTypeGroupTable.ReadOnly = false;
            this._requestedProcedureTypeGroupTable.Selection = selection1;
            this._requestedProcedureTypeGroupTable.Size = new System.Drawing.Size(235, 288);
            this._requestedProcedureTypeGroupTable.TabIndex = 0;
            this._requestedProcedureTypeGroupTable.Table = null;
            this._requestedProcedureTypeGroupTable.ToolbarModel = null;
            this._requestedProcedureTypeGroupTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._requestedProcedureTypeGroupTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._requestedProcedureTypeGroupTable.ItemDoubleClicked += new System.EventHandler(this._requestedProcedureTypeGroupTable_ItemDoubleClicked);
            // 
            // RequestedProcedureTypeGroupSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._requestedProcedureTypeGroupTable);
            this.Name = "RequestedProcedureTypeGroupSummaryComponentControl";
            this.Size = new System.Drawing.Size(235, 288);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _requestedProcedureTypeGroupTable;
    }
}
