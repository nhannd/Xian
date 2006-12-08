namespace ClearCanvas.Ris.Client.Adt.View.WinForms
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._phoneNumbers = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._errorProvider = new ClearCanvas.Desktop.View.WinForms.ValidationErrorProvider();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._phoneNumbers, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(199, 140);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // _phoneNumbers
            // 
            this._phoneNumbers.Dock = System.Windows.Forms.DockStyle.Fill;
            this._errorProvider.SetIconAlignment(this._phoneNumbers, System.Windows.Forms.ErrorIconAlignment.TopRight);
            this._phoneNumbers.Location = new System.Drawing.Point(5, 5);
            this._phoneNumbers.Margin = new System.Windows.Forms.Padding(5, 5, 25, 5);
            this._phoneNumbers.MenuModel = null;
            this._phoneNumbers.MultiSelect = false;
            this._phoneNumbers.Name = "_phoneNumbers";
            this._phoneNumbers.ReadOnly = false;
            this._phoneNumbers.Selection = selection1;
            this._phoneNumbers.Size = new System.Drawing.Size(169, 130);
            this._phoneNumbers.TabIndex = 1;
            this._phoneNumbers.Table = null;
            this._phoneNumbers.ToolbarModel = null;
            this._phoneNumbers.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._phoneNumbers.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._phoneNumbers.ItemDoubleClicked += new System.EventHandler(this._phoneNumbers_ItemDoubleClicked);
            this._phoneNumbers.SelectionChanged += new System.EventHandler(this._phoneNumbers_SelectionChanged);
            // 
            // _errorProvider
            // 
            this._errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this._errorProvider.ContainerControl = this;
            // 
            // PhoneNumbersSummaryControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PhoneNumbersSummaryControl";
            this.Size = new System.Drawing.Size(199, 140);
            this.Load += new System.EventHandler(this.PhoneNumbersSummaryControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _phoneNumbers;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.ValidationErrorProvider _errorProvider;
    }
}
