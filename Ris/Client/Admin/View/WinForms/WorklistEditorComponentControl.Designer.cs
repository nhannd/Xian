namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class WorklistEditorComponentControl
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
            ClearCanvas.Desktop.Selection selection3 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection4 = new ClearCanvas.Desktop.Selection();
            this._name = new ClearCanvas.Controls.WinForms.TextField();
            this._type = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._description = new ClearCanvas.Controls.WinForms.TextAreaField();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._cancelButton = new System.Windows.Forms.Button();
            this._acceptButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this._addRequestedProceduerTypeButton = new System.Windows.Forms.Button();
            this._removeRequestedProcedureTypeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._availableRequestedProcedureTypes = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._selectedRequestedProcedureTypes = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this._addUserButton = new System.Windows.Forms.Button();
            this._removeUserButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._availableUsers = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._selectedUsers = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.SuspendLayout();
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
            this._name.Size = new System.Drawing.Size(325, 41);
            this._name.TabIndex = 0;
            this._name.Value = null;
            // 
            // _type
            // 
            this._type.AutoSize = true;
            this._type.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._type.DataSource = null;
            this._type.DisplayMember = "";
            this._type.Dock = System.Windows.Forms.DockStyle.Fill;
            this._type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._type.LabelText = "Type";
            this._type.Location = new System.Drawing.Point(331, 2);
            this._type.Margin = new System.Windows.Forms.Padding(2);
            this._type.Name = "_type";
            this._type.Size = new System.Drawing.Size(325, 41);
            this._type.TabIndex = 1;
            this._type.Value = null;
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
            this._description.Size = new System.Drawing.Size(654, 69);
            this._description.TabIndex = 2;
            this._description.Value = null;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this._name, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._type, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this._description, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(658, 118);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._acceptButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 537);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(658, 29);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(580, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(499, 3);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 0;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(664, 569);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 127);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(658, 404);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel3);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(650, 378);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Requested Procedure Type Groups";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this._addRequestedProceduerTypeButton, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this._removeRequestedProcedureTypeButton, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this._availableRequestedProcedureTypes, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this._selectedRequestedProcedureTypes, 2, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(644, 372);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // _addRequestedProceduerTypeButton
            // 
            this._addRequestedProceduerTypeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._addRequestedProceduerTypeButton.Location = new System.Drawing.Point(284, 166);
            this._addRequestedProceduerTypeButton.Name = "_addRequestedProceduerTypeButton";
            this._addRequestedProceduerTypeButton.Size = new System.Drawing.Size(75, 23);
            this._addRequestedProceduerTypeButton.TabIndex = 4;
            this._addRequestedProceduerTypeButton.Text = ">>";
            this._addRequestedProceduerTypeButton.UseVisualStyleBackColor = true;
            this._addRequestedProceduerTypeButton.Click += new System.EventHandler(this.AddRequestedProcedureTypeSelection);
            // 
            // _removeRequestedProcedureTypeButton
            // 
            this._removeRequestedProcedureTypeButton.Location = new System.Drawing.Point(284, 195);
            this._removeRequestedProcedureTypeButton.Name = "_removeRequestedProcedureTypeButton";
            this._removeRequestedProcedureTypeButton.Size = new System.Drawing.Size(75, 23);
            this._removeRequestedProcedureTypeButton.TabIndex = 5;
            this._removeRequestedProcedureTypeButton.Text = "<<";
            this._removeRequestedProcedureTypeButton.UseVisualStyleBackColor = true;
            this._removeRequestedProcedureTypeButton.Click += new System.EventHandler(this.RemoveRequestedProcedureTypeSelection);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Available";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(365, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 1;
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
            this._availableRequestedProcedureTypes.Size = new System.Drawing.Size(275, 353);
            this._availableRequestedProcedureTypes.TabIndex = 2;
            this._availableRequestedProcedureTypes.Table = null;
            this._availableRequestedProcedureTypes.ToolbarModel = null;
            this._availableRequestedProcedureTypes.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._availableRequestedProcedureTypes.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._availableRequestedProcedureTypes.ItemDoubleClicked += new System.EventHandler(this.AddRequestedProcedureTypeSelection);
            // 
            // _selectedRequestedProcedureTypes
            // 
            this._selectedRequestedProcedureTypes.AutoSize = true;
            this._selectedRequestedProcedureTypes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._selectedRequestedProcedureTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this._selectedRequestedProcedureTypes.Location = new System.Drawing.Point(365, 16);
            this._selectedRequestedProcedureTypes.MenuModel = null;
            this._selectedRequestedProcedureTypes.Name = "_selectedRequestedProcedureTypes";
            this._selectedRequestedProcedureTypes.ReadOnly = false;
            this.tableLayoutPanel3.SetRowSpan(this._selectedRequestedProcedureTypes, 2);
            this._selectedRequestedProcedureTypes.Selection = selection2;
            this._selectedRequestedProcedureTypes.ShowToolbar = false;
            this._selectedRequestedProcedureTypes.Size = new System.Drawing.Size(276, 353);
            this._selectedRequestedProcedureTypes.TabIndex = 3;
            this._selectedRequestedProcedureTypes.Table = null;
            this._selectedRequestedProcedureTypes.ToolbarModel = null;
            this._selectedRequestedProcedureTypes.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._selectedRequestedProcedureTypes.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._selectedRequestedProcedureTypes.ItemDoubleClicked += new System.EventHandler(this.RemoveRequestedProcedureTypeSelection);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel4);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(650, 378);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Users";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this._addUserButton, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this._removeUserButton, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label4, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this._availableUsers, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this._selectedUsers, 2, 1);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(644, 372);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // _addUserButton
            // 
            this._addUserButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._addUserButton.Location = new System.Drawing.Point(284, 166);
            this._addUserButton.Name = "_addUserButton";
            this._addUserButton.Size = new System.Drawing.Size(75, 23);
            this._addUserButton.TabIndex = 4;
            this._addUserButton.Text = ">>";
            this._addUserButton.UseVisualStyleBackColor = true;
            this._addUserButton.Click += new System.EventHandler(this.AddUserSelection);
            // 
            // _removeUserButton
            // 
            this._removeUserButton.Location = new System.Drawing.Point(284, 195);
            this._removeUserButton.Name = "_removeUserButton";
            this._removeUserButton.Size = new System.Drawing.Size(75, 23);
            this._removeUserButton.TabIndex = 5;
            this._removeUserButton.Text = "<<";
            this._removeUserButton.UseVisualStyleBackColor = true;
            this._removeUserButton.Click += new System.EventHandler(this.RemoveUserSelection);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Available";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(365, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Selected";
            // 
            // _availableUsers
            // 
            this._availableUsers.AutoSize = true;
            this._availableUsers.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._availableUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this._availableUsers.Location = new System.Drawing.Point(3, 16);
            this._availableUsers.MenuModel = null;
            this._availableUsers.Name = "_availableUsers";
            this._availableUsers.ReadOnly = false;
            this.tableLayoutPanel4.SetRowSpan(this._availableUsers, 2);
            this._availableUsers.Selection = selection3;
            this._availableUsers.ShowToolbar = false;
            this._availableUsers.Size = new System.Drawing.Size(275, 353);
            this._availableUsers.TabIndex = 2;
            this._availableUsers.Table = null;
            this._availableUsers.ToolbarModel = null;
            this._availableUsers.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._availableUsers.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._availableUsers.ItemDoubleClicked += new System.EventHandler(this.AddUserSelection);
            // 
            // _selectedUsers
            // 
            this._selectedUsers.AutoSize = true;
            this._selectedUsers.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._selectedUsers.Dock = System.Windows.Forms.DockStyle.Fill;
            this._selectedUsers.Location = new System.Drawing.Point(365, 16);
            this._selectedUsers.MenuModel = null;
            this._selectedUsers.Name = "_selectedUsers";
            this._selectedUsers.ReadOnly = false;
            this.tableLayoutPanel4.SetRowSpan(this._selectedUsers, 2);
            this._selectedUsers.Selection = selection4;
            this._selectedUsers.ShowToolbar = false;
            this._selectedUsers.Size = new System.Drawing.Size(276, 353);
            this._selectedUsers.TabIndex = 3;
            this._selectedUsers.Table = null;
            this._selectedUsers.ToolbarModel = null;
            this._selectedUsers.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._selectedUsers.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._selectedUsers.ItemDoubleClicked += new System.EventHandler(this.RemoveUserSelection);
            // 
            // WorklistEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WorklistEditorComponentControl";
            this.Size = new System.Drawing.Size(664, 569);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Controls.WinForms.TextField _name;
        private ClearCanvas.Controls.WinForms.ComboBoxField _type;
        private ClearCanvas.Controls.WinForms.TextAreaField _description;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Button _addRequestedProceduerTypeButton;
        private System.Windows.Forms.Button _removeRequestedProcedureTypeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private ClearCanvas.Desktop.View.WinForms.TableView _availableRequestedProcedureTypes;
        private ClearCanvas.Desktop.View.WinForms.TableView _selectedRequestedProcedureTypes;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Button _addUserButton;
        private System.Windows.Forms.Button _removeUserButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private ClearCanvas.Desktop.View.WinForms.TableView _availableUsers;
        private ClearCanvas.Desktop.View.WinForms.TableView _selectedUsers;
    }
}
