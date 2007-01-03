namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class LocationSummaryComponentControl
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
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._locations = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this._locations, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(672, 254);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // _locations
            // 
            this._locations.Dock = System.Windows.Forms.DockStyle.Fill;
            this._locations.Location = new System.Drawing.Point(3, 3);
            this._locations.MenuModel = null;
            this._locations.MultiSelect = true;
            this._locations.Name = "_locations";
            this._locations.ReadOnly = false;
            this._locations.Selection = selection2;
            this._locations.Size = new System.Drawing.Size(666, 248);
            this._locations.TabIndex = 0;
            this._locations.Table = null;
            this._locations.ToolbarModel = null;
            this._locations.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._locations.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._locations.Load += new System.EventHandler(this._locations_Load);
            this._locations.ItemDoubleClicked += new System.EventHandler(this._locations_ItemDoubleClicked);
            // 
            // LocationSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LocationSummaryComponentControl";
            this.Size = new System.Drawing.Size(672, 254);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.TableView _locations;
    }
}
