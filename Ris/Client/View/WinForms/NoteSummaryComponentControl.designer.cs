namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class NoteSummaryComponentControl
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
            this._noteList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.SuspendLayout();
            // 
            // _noteList
            // 
            this._noteList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._noteList.AutoSize = true;
            this._noteList.Location = new System.Drawing.Point(0, 0);
            this._noteList.MenuModel = null;
            this._noteList.Name = "_noteList";
            this._noteList.ReadOnly = false;
            this._noteList.Selection = selection1;
            this._noteList.Size = new System.Drawing.Size(152, 158);
            this._noteList.TabIndex = 0;
            this._noteList.Table = null;
            this._noteList.ToolbarModel = null;
            this._noteList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._noteList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            // 
            // NoteSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._noteList);
            this.Name = "NoteSummaryComponentControl";
            this.Size = new System.Drawing.Size(150, 158);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _noteList;
    }
}
