namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class AddressesSummaryControl
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
            this._addressList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _addressList
            // 
            this._addressList.AutoSize = true;
            this._addressList.DataSource = null;
            this._addressList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._addressList.Location = new System.Drawing.Point(3, 3);
            this._addressList.Name = "_addressList";
            this._addressList.Size = new System.Drawing.Size(123, 112);
            this._addressList.TabIndex = 1;
            this._addressList.ToolbarModel = null;
            this._addressList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._addressList.ItemDoubleClicked += new System.EventHandler(this._addressList_ItemDoubleClicked);
            this._addressList.SelectionChanged += new System.EventHandler(this._addressList_SelectionChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._addressList, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(129, 118);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // AddressesSummaryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "AddressesSummaryControl";
            this.Size = new System.Drawing.Size(129, 118);
            this.Load += new System.EventHandler(this.AddressesSummaryControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _addressList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
