namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class AuthorityGroupSummaryComponentControl
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
            this._authorityGroups = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _authorityGroups
            // 
            this._authorityGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._authorityGroups.AutoSize = true;
            this._authorityGroups.Location = new System.Drawing.Point(0, 0);
            this._authorityGroups.MenuModel = null;
            this._authorityGroups.Name = "_authorityGroups";
            this._authorityGroups.ReadOnly = false;
            this._authorityGroups.Selection = selection1;
            this._authorityGroups.Size = new System.Drawing.Size(150, 150);
            this._authorityGroups.TabIndex = 0;
            this._authorityGroups.Table = null;
            this._authorityGroups.ToolbarModel = null;
            this._authorityGroups.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._authorityGroups.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._authorityGroups.Load += new System.EventHandler(this._authorityGroups_Load);
            this._authorityGroups.ItemDoubleClicked += new System.EventHandler(this._authorityGroups_ItemDoubleClicked);
            // 
            // AuthorityGroupSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._authorityGroups);
            this.Name = "AuthorityGroupSummaryComponentControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _authorityGroups;
    }
}
