namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class ProtocolEditorComponentControl
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
			this.components = new System.ComponentModel.Container();
			this._tableLayoutInner = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._protocolCodesSelector = new ClearCanvas.Desktop.View.WinForms.ListItemSelector();
			this._protocolGroup = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._btnSetDefault = new System.Windows.Forms.Button();
			this._urgency = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._author = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._rememberSupervisorCheckbox = new System.Windows.Forms.CheckBox();
			this._supervisor = new ClearCanvas.Ris.Client.View.WinForms.LookupField();
			this._tableLayoutInner.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableLayoutInner
			// 
			this._tableLayoutInner.ColumnCount = 1;
			this._tableLayoutInner.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutInner.Controls.Add(this.tableLayoutPanel1, 0, 3);
			this._tableLayoutInner.Controls.Add(this.groupBox1, 0, 1);
			this._tableLayoutInner.Controls.Add(this._urgency, 0, 0);
			this._tableLayoutInner.Controls.Add(this._author, 0, 2);
			this._tableLayoutInner.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutInner.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutInner.Margin = new System.Windows.Forms.Padding(0);
			this._tableLayoutInner.Name = "_tableLayoutInner";
			this._tableLayoutInner.RowCount = 4;
			this._tableLayoutInner.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutInner.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._tableLayoutInner.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutInner.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutInner.Size = new System.Drawing.Size(527, 695);
			this._tableLayoutInner.TabIndex = 3;
			// 
			// groupBox1
			// 
			this.groupBox1.AutoSize = true;
			this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox1.Controls.Add(this.tableLayoutPanel3);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(0, 45);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(527, 561);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Codes";
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel3.Controls.Add(this._protocolCodesSelector, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this._protocolGroup, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this._btnSetDefault, 1, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(521, 542);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// _protocolCodesSelector
			// 
			this._protocolCodesSelector.AutoSize = true;
			this._protocolCodesSelector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._protocolCodesSelector.AvailableItemsTable = null;
			this.tableLayoutPanel3.SetColumnSpan(this._protocolCodesSelector, 2);
			this._protocolCodesSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this._protocolCodesSelector.Location = new System.Drawing.Point(3, 48);
			this._protocolCodesSelector.Margin = new System.Windows.Forms.Padding(3, 3, 20, 3);
			this._protocolCodesSelector.Name = "_protocolCodesSelector";
			this._protocolCodesSelector.SelectedItemsTable = null;
			this._protocolCodesSelector.ShowColumnHeading = false;
			this._protocolCodesSelector.ShowToolbars = false;
			this._protocolCodesSelector.Size = new System.Drawing.Size(498, 491);
			this._protocolCodesSelector.TabIndex = 2;
			// 
			// _protocolGroup
			// 
			this._protocolGroup.AutoSize = true;
			this._protocolGroup.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._protocolGroup.DataSource = null;
			this._protocolGroup.DisplayMember = "";
			this._protocolGroup.Dock = System.Windows.Forms.DockStyle.Fill;
			this._protocolGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._protocolGroup.LabelText = "Protocol Group";
			this._protocolGroup.Location = new System.Drawing.Point(2, 2);
			this._protocolGroup.Margin = new System.Windows.Forms.Padding(2);
			this._protocolGroup.Name = "_protocolGroup";
			this._protocolGroup.Size = new System.Drawing.Size(256, 41);
			this._protocolGroup.TabIndex = 0;
			this._protocolGroup.Value = null;
			// 
			// _btnSetDefault
			// 
			this._btnSetDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._btnSetDefault.AutoSize = true;
			this._btnSetDefault.Location = new System.Drawing.Point(263, 19);
			this._btnSetDefault.Name = "_btnSetDefault";
			this._btnSetDefault.Size = new System.Drawing.Size(85, 23);
			this._btnSetDefault.TabIndex = 1;
			this._btnSetDefault.Text = "Set As Default";
			this._btnSetDefault.UseVisualStyleBackColor = true;
			this._btnSetDefault.Click += new System.EventHandler(this._btnSetDefault_Click);
			// 
			// _urgency
			// 
			this._urgency.AutoSize = true;
			this._urgency.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._urgency.DataSource = null;
			this._urgency.DisplayMember = "";
			this._urgency.Dock = System.Windows.Forms.DockStyle.Fill;
			this._urgency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._urgency.LabelText = "Urgency";
			this._urgency.Location = new System.Drawing.Point(2, 2);
			this._urgency.Margin = new System.Windows.Forms.Padding(2, 2, 20, 2);
			this._urgency.Name = "_urgency";
			this._urgency.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this._urgency.Size = new System.Drawing.Size(505, 41);
			this._urgency.TabIndex = 1;
			this._urgency.Value = null;
			// 
			// _author
			// 
			this._author.AutoSize = true;
			this._author.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._author.Dock = System.Windows.Forms.DockStyle.Fill;
			this._author.LabelText = "Author";
			this._author.Location = new System.Drawing.Point(2, 608);
			this._author.Margin = new System.Windows.Forms.Padding(2);
			this._author.Mask = "";
			this._author.Name = "_author";
			this._author.PasswordChar = '\0';
			this._author.ReadOnly = true;
			this._author.Size = new System.Drawing.Size(523, 40);
			this._author.TabIndex = 3;
			this._author.ToolTip = null;
			this._author.Value = null;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this._rememberSupervisorCheckbox, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this._supervisor, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 650);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(527, 45);
			this.tableLayoutPanel1.TabIndex = 7;
			// 
			// _rememberSupervisorCheckbox
			// 
			this._rememberSupervisorCheckbox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._rememberSupervisorCheckbox.Location = new System.Drawing.Point(420, 3);
			this._rememberSupervisorCheckbox.Name = "_rememberSupervisorCheckbox";
			this._rememberSupervisorCheckbox.Size = new System.Drawing.Size(104, 39);
			this._rememberSupervisorCheckbox.TabIndex = 2;
			this._rememberSupervisorCheckbox.Text = "Remember Supervisor?";
			this._rememberSupervisorCheckbox.UseVisualStyleBackColor = true;
			// 
			// _supervisor
			// 
			this._supervisor.AutoSize = true;
			this._supervisor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._supervisor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._supervisor.LabelText = "Supervising Radiologist (if applicable):";
			this._supervisor.Location = new System.Drawing.Point(2, 2);
			this._supervisor.Margin = new System.Windows.Forms.Padding(2);
			this._supervisor.Name = "_supervisor";
			this._supervisor.Size = new System.Drawing.Size(413, 41);
			this._supervisor.TabIndex = 1;
			this._supervisor.Value = null;
			// 
			// ProtocolEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._tableLayoutInner);
			this.Name = "ProtocolEditorComponentControl";
			this.Size = new System.Drawing.Size(527, 695);
			this._tableLayoutInner.ResumeLayout(false);
			this._tableLayoutInner.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.ListItemSelector _protocolCodesSelector;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _protocolGroup;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutInner;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button _btnSetDefault;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _urgency;
		private ClearCanvas.Desktop.View.WinForms.TextField _author;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.CheckBox _rememberSupervisorCheckbox;
		private ClearCanvas.Ris.Client.View.WinForms.LookupField _supervisor;
    }
}
