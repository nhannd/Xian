namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class VisitPractitionersSummaryComponentControl
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
            this._visitPractitioners = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _visitPractitioners
            // 
            this._visitPractitioners.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._visitPractitioners.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._visitPractitioners.Location = new System.Drawing.Point(3, 3);
            this._visitPractitioners.MenuModel = null;
            this._visitPractitioners.MultiLine = true;
            this._visitPractitioners.Name = "_visitPractitioners";
            this._visitPractitioners.ReadOnly = false;
            this._visitPractitioners.Selection = selection1;
            this._visitPractitioners.Size = new System.Drawing.Size(144, 144);
            this._visitPractitioners.TabIndex = 0;
            this._visitPractitioners.Table = null;
            this._visitPractitioners.ToolbarModel = null;
            this._visitPractitioners.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._visitPractitioners.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._visitPractitioners.SelectionChanged += new System.EventHandler(this._visitPractitioners_SelectionChanged);
            // 
            // VisitPractitionersSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._visitPractitioners);
            this.Name = "VisitPractitionersSummaryComponentControl";
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _visitPractitioners;
    }
}
