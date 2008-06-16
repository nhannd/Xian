namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class ProtocolGroupEditorComponentControl
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
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this._protocolCodesTabPage = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this._codesSelector = new ClearCanvas.Desktop.View.WinForms.ListItemSelector();
			this._readingGroupsTabPage = new System.Windows.Forms.TabPage();
			this._readingGroupsSelector = new ClearCanvas.Desktop.View.WinForms.ListItemSelector();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this._protocolCodesTabPage.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this._readingGroupsTabPage.SuspendLayout();
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
			this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(664, 569);
			this.tableLayoutPanel1.TabIndex = 0;
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
			this.flowLayoutPanel1.TabIndex = 0;
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
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this._protocolCodesTabPage);
			this.tabControl1.Controls.Add(this._readingGroupsTabPage);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(3, 126);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(658, 405);
			this.tabControl1.TabIndex = 1;
			// 
			// _protocolCodesTabPage
			// 
			this._protocolCodesTabPage.Controls.Add(this.tableLayoutPanel3);
			this._protocolCodesTabPage.Location = new System.Drawing.Point(4, 22);
			this._protocolCodesTabPage.Name = "_protocolCodesTabPage";
			this._protocolCodesTabPage.Padding = new System.Windows.Forms.Padding(3);
			this._protocolCodesTabPage.Size = new System.Drawing.Size(650, 379);
			this._protocolCodesTabPage.TabIndex = 0;
			this._protocolCodesTabPage.Text = "Codes";
			this._protocolCodesTabPage.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 1;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.Controls.Add(this._codesSelector, 0, 0);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(644, 373);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// _codesSelector
			// 
			this._codesSelector.AutoSize = true;
			this._codesSelector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._codesSelector.AvailableItemsTable = null;
			this._codesSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this._codesSelector.Location = new System.Drawing.Point(3, 3);
			this._codesSelector.Name = "_codesSelector";
			this._codesSelector.SelectedItemsTable = null;
			this._codesSelector.Size = new System.Drawing.Size(638, 367);
			this._codesSelector.TabIndex = 0;
			// 
			// _readingGroupsTabPage
			// 
			this._readingGroupsTabPage.Controls.Add(this._readingGroupsSelector);
			this._readingGroupsTabPage.Location = new System.Drawing.Point(4, 22);
			this._readingGroupsTabPage.Name = "_readingGroupsTabPage";
			this._readingGroupsTabPage.Padding = new System.Windows.Forms.Padding(3);
			this._readingGroupsTabPage.Size = new System.Drawing.Size(650, 379);
			this._readingGroupsTabPage.TabIndex = 1;
			this._readingGroupsTabPage.Text = "Reading Groups";
			this._readingGroupsTabPage.UseVisualStyleBackColor = true;
			// 
			// _readingGroupsSelector
			// 
			this._readingGroupsSelector.AutoSize = true;
			this._readingGroupsSelector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._readingGroupsSelector.AvailableItemsTable = null;
			this._readingGroupsSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this._readingGroupsSelector.Location = new System.Drawing.Point(3, 3);
			this._readingGroupsSelector.Name = "_readingGroupsSelector";
			this._readingGroupsSelector.SelectedItemsTable = null;
			this._readingGroupsSelector.ShowToolbars = false;
			this._readingGroupsSelector.Size = new System.Drawing.Size(644, 373);
			this._readingGroupsSelector.TabIndex = 0;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.AutoSize = true;
			this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this._name, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this._description, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(658, 117);
			this.tableLayoutPanel2.TabIndex = 0;
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
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(654, 40);
			this._name.TabIndex = 0;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _description
			// 
			this._description.AutoSize = true;
			this._description.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._description.Dock = System.Windows.Forms.DockStyle.Fill;
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(2, 46);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(654, 69);
			this._description.TabIndex = 1;
			this._description.Value = null;
			// 
			// ProtocolGroupEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ProtocolGroupEditorComponentControl";
			this.Size = new System.Drawing.Size(664, 569);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this._protocolCodesTabPage.ResumeLayout(false);
			this._protocolCodesTabPage.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this._readingGroupsTabPage.ResumeLayout(false);
			this._readingGroupsTabPage.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage _protocolCodesTabPage;
        private System.Windows.Forms.TabPage _readingGroupsTabPage;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private ClearCanvas.Desktop.View.WinForms.TextField _name;
        private ClearCanvas.Desktop.View.WinForms.TextAreaField _description;
        private ClearCanvas.Desktop.View.WinForms.ListItemSelector _codesSelector;
        private ClearCanvas.Desktop.View.WinForms.ListItemSelector _readingGroupsSelector;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}
