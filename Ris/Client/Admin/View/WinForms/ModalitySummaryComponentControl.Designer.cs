namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class ModalitySummaryComponentControl
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
            this._modalities = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _modalities
            // 
            this._modalities.Dock = System.Windows.Forms.DockStyle.Fill;
            this._modalities.Location = new System.Drawing.Point(0, 0);
            this._modalities.MenuModel = null;
            this._modalities.MultiSelect = true;
            this._modalities.Name = "_modalities";
            this._modalities.ReadOnly = false;
            this._modalities.Selection = selection1;
            this._modalities.Size = new System.Drawing.Size(229, 261);
            this._modalities.TabIndex = 0;
            this._modalities.Table = null;
            this._modalities.ToolbarModel = null;
            this._modalities.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._modalities.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._modalities.Load += new System.EventHandler(this._modalities_Load);
            this._modalities.ItemDoubleClicked += new System.EventHandler(this._modalities_ItemDoubleClicked);
            // 
            // ModalitySummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._modalities);
            this.Name = "ModalitySummaryComponentControl";
            this.Size = new System.Drawing.Size(229, 261);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _modalities;
    }
}
