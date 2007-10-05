namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    partial class RadiologistSelectionComponentControl
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this._radiologistTable = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._makeDefault = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this._radiologistTable, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._makeDefault, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(316, 291);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._acceptButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(140, 255);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(173, 30);
            this.flowLayoutPanel1.TabIndex = 5;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(95, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 2;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(14, 3);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 0;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // _radiologistTable
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._radiologistTable, 2);
            this._radiologistTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._radiologistTable.Location = new System.Drawing.Point(3, 3);
            this._radiologistTable.MenuModel = null;
            this._radiologistTable.Name = "_radiologistTable";
            this._radiologistTable.ReadOnly = false;
            this._radiologistTable.Selection = selection1;
            this._radiologistTable.ShowToolbar = false;
            this._radiologistTable.Size = new System.Drawing.Size(310, 246);
            this._radiologistTable.TabIndex = 1;
            this._radiologistTable.Table = null;
            this._radiologistTable.ToolbarModel = null;
            this._radiologistTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._radiologistTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._radiologistTable.ItemDoubleClicked += new System.EventHandler(this._radiologistTable_ItemDoubleClicked);
            // 
            // _makeDefault
            // 
            this._makeDefault.AutoSize = true;
            this._makeDefault.Location = new System.Drawing.Point(3, 255);
            this._makeDefault.Name = "_makeDefault";
            this._makeDefault.Size = new System.Drawing.Size(90, 17);
            this._makeDefault.TabIndex = 2;
            this._makeDefault.Text = "Make Default";
            this._makeDefault.UseVisualStyleBackColor = true;
            // 
            // RadiologistSelectionComponentControl
            // 
            this.AcceptButton = this._acceptButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RadiologistSelectionComponentControl";
            this.Size = new System.Drawing.Size(316, 291);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.TableView _radiologistTable;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.CheckBox _makeDefault;
    }
}
