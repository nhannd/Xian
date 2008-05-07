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
			this._path = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._buttonFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._acceptButton = new System.Windows.Forms.Button();
			this._editorTableLayoutPanel.SuspendLayout();
			this._buttonFlowLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _editorTableLayoutPanel
			// 
			this._editorTableLayoutPanel.ColumnCount = 1;
			this._editorTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._editorTableLayoutPanel.Controls.Add(this._text, 0, 2);
			this._editorTableLayoutPanel.Controls.Add(this._name, 0, 0);
			this._editorTableLayoutPanel.Controls.Add(this._path, 0, 1);
			this._editorTableLayoutPanel.Controls.Add(this._buttonFlowLayoutPanel, 0, 3);
			this._editorTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._editorTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._editorTableLayoutPanel.Name = "_editorTableLayoutPanel";
			this._editorTableLayoutPanel.Padding = new System.Windows.Forms.Padding(0, 0, 15, 0);
			this._editorTableLayoutPanel.RowCount = 4;
			this._editorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._editorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._editorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._editorTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this._editorTableLayoutPanel.Size = new System.Drawing.Size(294, 344);
			this._editorTableLayoutPanel.TabIndex = 0;
			// 
			// _text
			// 
			this._text.Dock = System.Windows.Forms.DockStyle.Fill;
			this._text.LabelText = "Text";
			this._text.Location = new System.Drawing.Point(2, 92);
			this._text.Margin = new System.Windows.Forms.Padding(2);
			this._text.Name = "_text";
			this._text.Size = new System.Drawing.Size(275, 215);
			this._text.TabIndex = 2;
			this._text.Value = null;
			// 
			// _name
			// 
			this._name.Dock = System.Windows.Forms.DockStyle.Fill;
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(2, 2);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(275, 41);
			this._name.TabIndex = 0;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _path
			// 
			this._path.Dock = System.Windows.Forms.DockStyle.Fill;
			this._path.LabelText = "Path";
			this._path.Location = new System.Drawing.Point(2, 47);
			this._path.Margin = new System.Windows.Forms.Padding(2);
			this._path.Mask = "";
			this._path.Name = "_path";
			this._path.PasswordChar = '\0';
			this._path.Size = new System.Drawing.Size(275, 41);
			this._path.TabIndex = 1;
			this._path.ToolTip = null;
			this._path.Value = null;
			// 
			// _buttonFlowLayoutPanel
			// 
			this._buttonFlowLayoutPanel.Controls.Add(this._cancelButton);
			this._buttonFlowLayoutPanel.Controls.Add(this._acceptButton);
			this._buttonFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._buttonFlowLayoutPanel.Location = new System.Drawing.Point(3, 312);
			this._buttonFlowLayoutPanel.Name = "_buttonFlowLayoutPanel";
			this._buttonFlowLayoutPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this._buttonFlowLayoutPanel.Size = new System.Drawing.Size(273, 29);
			this._buttonFlowLayoutPanel.TabIndex = 3;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(195, 3);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 1;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(114, 3);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 0;
			this._acceptButton.Text = "Accept";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// CannedTextEditorComponentControl
			// 
			this.AcceptButton = this._acceptButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this._editorTableLayoutPanel);
			this.Name = "CannedTextEditorComponentControl";
			this.Size = new System.Drawing.Size(294, 344);
			this._editorTableLayoutPanel.ResumeLayout(false);
			this._buttonFlowLayoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TableLayoutPanel _editorTableLayoutPanel;
		private ClearCanvas.Desktop.View.WinForms.TextAreaField _text;
		private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private ClearCanvas.Desktop.View.WinForms.TextField _path;
		private System.Windows.Forms.FlowLayoutPanel _buttonFlowLayoutPanel;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _acceptButton;
    }
}
