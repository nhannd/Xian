namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class VisitLocationsSummaryComponentControl
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
            this._locations = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _locations
            // 
            this._locations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._locations.AutoSize = true;
            this._locations.Location = new System.Drawing.Point(0, 3);
            this._locations.MenuModel = null;
            this._locations.MultiLine = true;
            this._locations.MultiSelect = true;
            this._locations.Name = "_locations";
            this._locations.ReadOnly = false;
            this._locations.Size = new System.Drawing.Size(179, 146);
            this._locations.TabIndex = 0;
            this._locations.Table = null;
            this._locations.ToolbarModel = null;
            this._locations.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._locations.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._locations.SelectionChanged += new System.EventHandler(this._locations_SelectionChanged);
            // 
            // VisitLocationsSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._locations);
            this.Name = "VisitLocationsSummaryComponentControl";
            this.Size = new System.Drawing.Size(180, 150);
            this.Load += new System.EventHandler(this.VisitLocationsSummaryComponentControl_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _locations;
    }
}
