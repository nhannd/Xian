namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class RequestedProcedureTypeGroupEditorComponentControl
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
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this._addButton = new System.Windows.Forms.Button();
            this._removeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._availableRequestedProcedureTypes = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._selectedRequestedProcedureTypes = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this._name = new ClearCanvas.Controls.WinForms.TextField();
            this._category = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._description = new ClearCanvas.Controls.WinForms.TextAreaField();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(660, 566);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._acceptButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 534);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(654, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(576, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 0;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(495, 3);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 1;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel3);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 127);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(654, 401);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Requested Procedure Types";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this._addButton, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this._removeButton, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this._availableRequestedProcedureTypes, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this._selectedRequestedProcedureTypes, 2, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(648, 382);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // _addButton
            // 
            this._addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._addButton.Location = new System.Drawing.Point(286, 171);
            this._addButton.Name = "_addButton";
            this._addButton.Size = new System.Drawing.Size(75, 23);
            this._addButton.TabIndex = 0;
            this._addButton.Text = ">>";
            this._addButton.UseVisualStyleBackColor = true;
            this._addButton.Click += new System.EventHandler(this.AddSelection);
            // 
            // _removeButton
            // 
            this._removeButton.Location = new System.Drawing.Point(286, 200);
            this._removeButton.Name = "_removeButton";
            this._removeButton.Size = new System.Drawing.Size(75, 23);
            this._removeButton.TabIndex = 1;
            this._removeButton.Text = "<<";
            this._removeButton.UseVisualStyleBackColor = true;
            this._removeButton.Click += new System.EventHandler(this.RemoveSelection);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Available";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(367, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Selected";
            // 
            // _availableRequestedProcedureTypes
            // 
            this._availableRequestedProcedureTypes.AutoSize = true;
            this._availableRequestedProcedureTypes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._availableRequestedProcedureTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this._availableRequestedProcedureTypes.Location = new System.Drawing.Point(3, 16);
            this._availableRequestedProcedureTypes.MenuModel = null;
            this._availableRequestedProcedureTypes.Name = "_availableRequestedProcedureTypes";
            this._availableRequestedProcedureTypes.ReadOnly = false;
            this.tableLayoutPanel3.SetRowSpan(this._availableRequestedProcedureTypes, 2);
            this._availableRequestedProcedureTypes.Selection = selection1;
            this._availableRequestedProcedureTypes.ShowToolbar = false;
            this._availableRequestedProcedureTypes.Size = new System.Drawing.Size(277, 363);
            this._availableRequestedProcedureTypes.TabIndex = 6;
            this._availableRequestedProcedureTypes.Table = null;
            this._availableRequestedProcedureTypes.ToolbarModel = null;
            this._availableRequestedProcedureTypes.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._availableRequestedProcedureTypes.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._availableRequestedProcedureTypes.ItemDoubleClicked += new System.EventHandler(this.AddSelection);
            // 
            // _selectedRequestedProcedureTypes
            // 
            this._selectedRequestedProcedureTypes.AutoSize = true;
            this._selectedRequestedProcedureTypes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._selectedRequestedProcedureTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this._selectedRequestedProcedureTypes.Location = new System.Drawing.Point(367, 16);
            this._selectedRequestedProcedureTypes.MenuModel = null;
            this._selectedRequestedProcedureTypes.Name = "_selectedRequestedProcedureTypes";
            this._selectedRequestedProcedureTypes.ReadOnly = false;
            this.tableLayoutPanel3.SetRowSpan(this._selectedRequestedProcedureTypes, 2);
            this._selectedRequestedProcedureTypes.Selection = selection2;
            this._selectedRequestedProcedureTypes.ShowToolbar = false;
            this._selectedRequestedProcedureTypes.Size = new System.Drawing.Size(278, 363);
            this._selectedRequestedProcedureTypes.TabIndex = 7;
            this._selectedRequestedProcedureTypes.Table = null;
            this._selectedRequestedProcedureTypes.ToolbarModel = null;
            this._selectedRequestedProcedureTypes.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._selectedRequestedProcedureTypes.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._selectedRequestedProcedureTypes.ItemDoubleClicked += new System.EventHandler(this.RemoveSelection);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this._name, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._category, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this._description, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(654, 118);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // _name
            // 
            this._name.AutoSize = true;
            this._name.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._name.Dock = System.Windows.Forms.DockStyle.Fill;
            this._name.LabelText = "Name";
            this._name.Location = new System.Drawing.Point(2, 2);
            this._name.Margin = new System.Windows.Forms.Padding(2);
            this._name.Mask = "";
            this._name.Name = "_name";
            this._name.Size = new System.Drawing.Size(323, 41);
            this._name.TabIndex = 0;
            this._name.Value = null;
            // 
            // _category
            // 
            this._category.AutoSize = true;
            this._category.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._category.DataSource = null;
            this._category.DisplayMember = "";
            this._category.Dock = System.Windows.Forms.DockStyle.Fill;
            this._category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._category.LabelText = "Category";
            this._category.Location = new System.Drawing.Point(329, 2);
            this._category.Margin = new System.Windows.Forms.Padding(2);
            this._category.Name = "_category";
            this._category.Size = new System.Drawing.Size(323, 41);
            this._category.TabIndex = 1;
            this._category.Value = null;
            // 
            // _description
            // 
            this._description.AutoSize = true;
            this._description.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.SetColumnSpan(this._description, 2);
            this._description.Dock = System.Windows.Forms.DockStyle.Fill;
            this._description.LabelText = "Description";
            this._description.Location = new System.Drawing.Point(2, 47);
            this._description.Margin = new System.Windows.Forms.Padding(2);
            this._description.Name = "_description";
            this._description.Size = new System.Drawing.Size(650, 69);
            this._description.TabIndex = 2;
            this._description.Value = null;
            // 
            // RequestedProcedureTypeGroupEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "RequestedProcedureTypeGroupEditorComponentControl";
            this.Size = new System.Drawing.Size(660, 566);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private ClearCanvas.Controls.WinForms.TextField _name;
        private ClearCanvas.Controls.WinForms.ComboBoxField _category;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button _addButton;
        private System.Windows.Forms.Button _removeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ClearCanvas.Controls.WinForms.TextAreaField _description;
        private ClearCanvas.Desktop.View.WinForms.TableView _availableRequestedProcedureTypes;
        private ClearCanvas.Desktop.View.WinForms.TableView _selectedRequestedProcedureTypes;
    }
}
