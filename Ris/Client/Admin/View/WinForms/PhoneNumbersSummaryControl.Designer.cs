namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class PhoneNumbersSummaryControl
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._phoneNumbers = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._phoneNumbers, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(149, 114);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // _phoneNumbers
            // 
            this._phoneNumbers.Table = null;
            this._phoneNumbers.Dock = System.Windows.Forms.DockStyle.Fill;
            this._phoneNumbers.Location = new System.Drawing.Point(3, 3);
            this._phoneNumbers.Name = "_phoneNumbers";
            this._phoneNumbers.Size = new System.Drawing.Size(143, 108);
            this._phoneNumbers.TabIndex = 1;
            this._phoneNumbers.ToolbarModel = null;
            this._phoneNumbers.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._phoneNumbers.ItemDoubleClicked += new System.EventHandler(this._phoneNumbers_ItemDoubleClicked);
            this._phoneNumbers.SelectionChanged += new System.EventHandler(this._phoneNumbers_SelectionChanged);
            // 
            // PhoneNumbersSummaryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PhoneNumbersSummaryControl";
            this.Size = new System.Drawing.Size(149, 114);
            this.Load += new System.EventHandler(this.PhoneNumbersSummaryControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _phoneNumbers;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
