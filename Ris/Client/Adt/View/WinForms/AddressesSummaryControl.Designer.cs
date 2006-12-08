namespace ClearCanvas.Ris.Client.Adt.View.WinForms
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            this._addressList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._errorProvider = new ClearCanvas.Desktop.View.WinForms.ValidationErrorProvider();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // _addressList
            // 
            this._addressList.AutoSize = true;
            this._addressList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._errorProvider.SetIconAlignment(this._addressList, System.Windows.Forms.ErrorIconAlignment.TopRight);
            this._addressList.Location = new System.Drawing.Point(5, 5);
            this._addressList.Margin = new System.Windows.Forms.Padding(5, 5, 25, 5);
            this._addressList.MenuModel = null;
            this._addressList.MultiLine = true;
            this._addressList.MultiSelect = false;
            this._addressList.Name = "_addressList";
            this._addressList.ReadOnly = false;
            this._addressList.Selection = selection1;
            this._addressList.Size = new System.Drawing.Size(142, 135);
            this._addressList.TabIndex = 1;
            this._addressList.Table = null;
            this._addressList.ToolbarModel = null;
            this._addressList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
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
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(172, 145);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // _errorProvider
            // 
            this._errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this._errorProvider.ContainerControl = this;
            // 
            // AddressesSummaryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "AddressesSummaryControl";
            this.Size = new System.Drawing.Size(172, 145);
            this.Load += new System.EventHandler(this.AddressesSummaryControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _addressList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.ValidationErrorProvider _errorProvider;
    }
}
