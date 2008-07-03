namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class CannedTextEditorComponentControl
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
			this._editorTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._text = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._buttonFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this._typeGroupBox = new System.Windows.Forms.GroupBox();
			this._radioGroup = new System.Windows.Forms.RadioButton();
			this._radioPersonal = new System.Windows.Forms.RadioButton();
			this._groups = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._category = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._editorTableLayoutPanel.SuspendLayout();
			this._buttonFlowLayoutPanel.SuspendLayout();
			this._typeGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _editorTableLayoutPanel
			// 
			this._editorTableLayoutPanel.ColumnCount = 1;
			this._editorTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._editorTableLayoutPanel.Controls.Add(this._text, 0, 4);
			this._editorTableLayoutPanel.Controls.Add(this._name, 0, 2);
			this._editorTableLayoutPanel.Controls.Add(this._buttonFlowLayoutPanel, 0, 5);
			this._editorTableLayoutPanel.Controls.Add(this._typeGroupBox, 0, 0);
			this._editorTableLayoutPanel.Controls.Add(this._groups, 0, 1);
			this._editorTableLayoutPanel.Controls.Add(this._category, 0, 3);
			this._editorTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._editorTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._editorTableLayoutPanel.Name = "_editorTableLayoutPanel";
			this._editorTableLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
			this._editorTableLayoutPanel.RowCount = 6;
			this._editorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._editorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._editorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._editorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._editorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._editorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this._editorTableLayoutPanel.Size = new System.Drawing.Size(301, 351);
			this._editorTableLayoutPanel.TabIndex = 0;
			// 
			// _text
			// 
			this._text.Dock = System.Windows.Forms.DockStyle.Fill;
			this._text.LabelText = "Text";
			this._text.Location = new System.Drawing.Point(2, 191);
			this._text.Margin = new System.Windows.Forms.Padding(2);
			this._text.Name = "_text";
			this._text.Size = new System.Drawing.Size(282, 123);
			this._text.TabIndex = 4;
			this._text.Value = null;
			// 
			// _name
			// 
			this._name.Dock = System.Windows.Forms.DockStyle.Fill;
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(2, 101);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(282, 41);
			this._name.TabIndex = 2;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _buttonFlowLayoutPanel
			// 
			this._buttonFlowLayoutPanel.Controls.Add(this._cancelButton);
			this._buttonFlowLayoutPanel.Controls.Add(this._acceptButton);
			this._buttonFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._buttonFlowLayoutPanel.Location = new System.Drawing.Point(3, 319);
			this._buttonFlowLayoutPanel.Name = "_buttonFlowLayoutPanel";
			this._buttonFlowLayoutPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this._buttonFlowLayoutPanel.Size = new System.Drawing.Size(280, 29);
			this._buttonFlowLayoutPanel.TabIndex = 5;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(202, 3);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(121, 3);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 0;
			this._acceptButton.Text = "OK";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _typeGroupBox
			// 
			this._typeGroupBox.Controls.Add(this._radioGroup);
			this._typeGroupBox.Controls.Add(this._radioPersonal);
			this._typeGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._typeGroupBox.Location = new System.Drawing.Point(3, 3);
			this._typeGroupBox.Name = "_typeGroupBox";
			this._typeGroupBox.Size = new System.Drawing.Size(280, 47);
			this._typeGroupBox.TabIndex = 0;
			this._typeGroupBox.TabStop = false;
			this._typeGroupBox.Text = "Type";
			// 
			// _radioGroup
			// 
			this._radioGroup.AutoSize = true;
			this._radioGroup.Location = new System.Drawing.Point(78, 19);
			this._radioGroup.Name = "_radioGroup";
			this._radioGroup.Size = new System.Drawing.Size(54, 17);
			this._radioGroup.TabIndex = 1;
			this._radioGroup.TabStop = true;
			this._radioGroup.Text = "Group";
			this._radioGroup.UseVisualStyleBackColor = true;
			// 
			// _radioPersonal
			// 
			this._radioPersonal.AutoSize = true;
			this._radioPersonal.Location = new System.Drawing.Point(6, 19);
			this._radioPersonal.Name = "_radioPersonal";
			this._radioPersonal.Size = new System.Drawing.Size(66, 17);
			this._radioPersonal.TabIndex = 0;
			this._radioPersonal.TabStop = true;
			this._radioPersonal.Text = "Personal";
			this._radioPersonal.UseVisualStyleBackColor = true;
			// 
			// _groups
			// 
			this._groups.DataSource = null;
			this._groups.DisplayMember = "";
			this._groups.Dock = System.Windows.Forms.DockStyle.Fill;
			this._groups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._groups.LabelText = "Staff Group";
			this._groups.Location = new System.Drawing.Point(2, 55);
			this._groups.Margin = new System.Windows.Forms.Padding(2);
			this._groups.Name = "_groups";
			this._groups.Size = new System.Drawing.Size(282, 42);
			this._groups.TabIndex = 1;
			this._groups.Value = null;
			// 
			// _category
			// 
			this._category.DataSource = null;
			this._category.DisplayMember = "";
			this._category.Dock = System.Windows.Forms.DockStyle.Fill;
			this._category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
			this._category.LabelText = "Category";
			this._category.Location = new System.Drawing.Point(2, 146);
			this._category.Margin = new System.Windows.Forms.Padding(2);
			this._category.Name = "_category";
			this._category.Size = new System.Drawing.Size(282, 41);
			this._category.TabIndex = 3;
			this._category.Value = null;
			// 
			// CannedTextEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._editorTableLayoutPanel);
			this.Name = "CannedTextEditorComponentControl";
			this.Size = new System.Drawing.Size(301, 351);
			this._editorTableLayoutPanel.ResumeLayout(false);
			this._buttonFlowLayoutPanel.ResumeLayout(false);
			this._typeGroupBox.ResumeLayout(false);
			this._typeGroupBox.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel _editorTableLayoutPanel;
		private ClearCanvas.Desktop.View.WinForms.TextAreaField _text;
		private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private System.Windows.Forms.FlowLayoutPanel _buttonFlowLayoutPanel;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.GroupBox _typeGroupBox;
		private System.Windows.Forms.RadioButton _radioGroup;
		private System.Windows.Forms.RadioButton _radioPersonal;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _groups;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _category;
    }
}
