namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class BiographyNoteComponentControl
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
            this._noteTable = new ClearCanvas.Desktop.View.WinForms.DecoratedTableView();
            this.SuspendLayout();
            // 
            // _noteTable
            // 
            this._noteTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._noteTable.Location = new System.Drawing.Point(0, 0);
            this._noteTable.MenuModel = null;
            this._noteTable.Name = "_noteTable";
            this._noteTable.ReadOnly = false;
            this._noteTable.Selection = selection1;
            this._noteTable.Size = new System.Drawing.Size(434, 269);
            this._noteTable.TabIndex = 0;
            this._noteTable.Table = null;
            this._noteTable.ToolbarModel = null;
            this._noteTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._noteTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // BiographyNoteComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._noteTable);
            this.Name = "BiographyNoteComponentControl";
            this.Size = new System.Drawing.Size(434, 269);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.DecoratedTableView _noteTable;

    }
}
