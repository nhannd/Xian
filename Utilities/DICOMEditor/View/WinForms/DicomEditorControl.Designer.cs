namespace ClearCanvas.Utilities.DicomEditor.View.WinForms
{
    partial class DicomEditorControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._dicomTagTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._dicomEditorTitleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
            this.SuspendLayout();
            // 
            // _dicomTagTable
            // 
            this._dicomTagTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dicomTagTable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._dicomTagTable.Location = new System.Drawing.Point(0, 23);
            this._dicomTagTable.MenuModel = null;
            this._dicomTagTable.Name = "_dicomTagTable";
            this._dicomTagTable.ReadOnly = false;
            this._dicomTagTable.Size = new System.Drawing.Size(701, 444);
            this._dicomTagTable.TabIndex = 0;
            this._dicomTagTable.Table = null;
            this._dicomTagTable.ToolbarModel = null;
            this._dicomTagTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this._dicomTagTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _dicomEditorTitleBar
            // 
            this._dicomEditorTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
            this._dicomEditorTitleBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._dicomEditorTitleBar.ForeColor = System.Drawing.SystemColors.WindowText;
            this._dicomEditorTitleBar.Location = new System.Drawing.Point(0, 0);
            this._dicomEditorTitleBar.MouseOverColor = System.Drawing.Color.Empty;
            this._dicomEditorTitleBar.Name = "_dicomEditorTitleBar";
            this._dicomEditorTitleBar.Size = new System.Drawing.Size(701, 23);
            this._dicomEditorTitleBar.TabIndex = 4;
            // 
            // DicomEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._dicomTagTable);
            this.Controls.Add(this._dicomEditorTitleBar);
            this.Name = "DicomEditorControl";
            this.Size = new System.Drawing.Size(701, 467);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _dicomTagTable;
        private Crownwood.DotNetMagic.Controls.TitleBar _dicomEditorTitleBar;
    }
}
