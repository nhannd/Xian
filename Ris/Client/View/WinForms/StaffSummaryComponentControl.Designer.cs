namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class StaffSummaryComponentControl
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
            this._staffs = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this._lastName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._firstName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._searchButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _staffs
            // 
            this._staffs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._staffs.Location = new System.Drawing.Point(4, 62);
            this._staffs.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._staffs.MenuModel = null;
            this._staffs.Name = "_staffs";
            this._staffs.ReadOnly = false;
            this._staffs.Selection = selection1;
            this._staffs.Size = new System.Drawing.Size(562, 293);
            this._staffs.TabIndex = 0;
            this._staffs.Table = null;
            this._staffs.ToolbarModel = null;
            this._staffs.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._staffs.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._staffs.Load += new System.EventHandler(this._staffs_Load);
            this._staffs.ItemDoubleClicked += new System.EventHandler(this._staffs_ItemDoubleClicked);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._staffs, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(570, 411);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this._lastName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._firstName, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this._searchButton, 2, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(410, 54);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // _lastName
            // 
            this._lastName.LabelText = "Last Name";
            this._lastName.Location = new System.Drawing.Point(2, 2);
            this._lastName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._lastName.Mask = "";
            this._lastName.Name = "_lastName";
            this._lastName.Size = new System.Drawing.Size(137, 41);
            this._lastName.TabIndex = 0;
            this._lastName.Value = null;
            // 
            // _firstName
            // 
            this._firstName.LabelText = "First Name";
            this._firstName.Location = new System.Drawing.Point(143, 2);
            this._firstName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._firstName.Mask = "";
            this._firstName.Name = "_firstName";
            this._firstName.Size = new System.Drawing.Size(150, 41);
            this._firstName.TabIndex = 1;
            this._firstName.Value = null;
            // 
            // _searchButton
            // 
            this._searchButton.Location = new System.Drawing.Point(297, 2);
            this._searchButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._searchButton.Name = "_searchButton";
            this._searchButton.Size = new System.Drawing.Size(56, 19);
            this._searchButton.TabIndex = 2;
            this._searchButton.Text = "Search";
            this._searchButton.UseVisualStyleBackColor = true;
            this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._okButton);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(2, 361);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(566, 46);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(508, 2);
            this._cancelButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(56, 19);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(448, 2);
            this._okButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(56, 19);
            this._okButton.TabIndex = 0;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // StaffSummaryComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "StaffSummaryComponentControl";
            this.Size = new System.Drawing.Size(570, 411);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _staffs;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private ClearCanvas.Desktop.View.WinForms.TextField _lastName;
        private ClearCanvas.Desktop.View.WinForms.TextField _firstName;
        private System.Windows.Forms.Button _searchButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
    }
}
