namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class PractitionerSummaryComponentControl
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
            this._practitioners = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _practitioners
            // 
            this._practitioners.Dock = System.Windows.Forms.DockStyle.Fill;
            this._practitioners.Location = new System.Drawing.Point(0, 0);
            this._practitioners.MenuModel = null;
            this._practitioners.Name = "_practitioners";
            this._practitioners.ReadOnly = false;
            this._practitioners.Selection = selection1;
            this._practitioners.Size = new System.Drawing.Size(447, 284);
            this._practitioners.TabIndex = 0;
            this._practitioners.Table = null;
            this._practitioners.ToolbarModel = null;
            this._practitioners.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._practitioners.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._practitioners.Load += new System.EventHandler(this._practitioners_Load);
            this._practitioners.ItemDoubleClicked += new System.EventHandler(this._practitioners_ItemDoubleClicked);
            // 
            // PractitionerSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._practitioners);
            this.Name = "PractitionerSummaryComponentControl";
            this.Size = new System.Drawing.Size(447, 284);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _practitioners;
    }
}
