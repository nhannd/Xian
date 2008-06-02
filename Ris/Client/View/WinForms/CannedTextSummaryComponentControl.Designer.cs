namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class CannedTextSummaryComponentControl
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
			this._cannedTexts = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._previewGroupBox = new System.Windows.Forms.GroupBox();
			this._previewTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._text = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this._category = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._group = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this._previewGroupBox.SuspendLayout();
			this._previewTableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _cannedTexts
			// 
			this._cannedTexts.Dock = System.Windows.Forms.DockStyle.Fill;
			this._cannedTexts.Location = new System.Drawing.Point(0, 0);
			this._cannedTexts.Name = "_cannedTexts";
			this._cannedTexts.ReadOnly = false;
			this._cannedTexts.Size = new System.Drawing.Size(355, 234);
			this._cannedTexts.TabIndex = 0;
			this._cannedTexts.TabStop = false;
			this._cannedTexts.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._cannedTexts.ItemDrag += new System.EventHandler<System.Windows.Forms.ItemDragEventArgs>(this._cannedTexts_ItemDrag);
			this._cannedTexts.ItemDoubleClicked += new System.EventHandler(this._cannedTexts_ItemDoubleClicked);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._cannedTexts);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this._previewGroupBox);
			this.splitContainer1.Size = new System.Drawing.Size(355, 552);
			this.splitContainer1.SplitterDistance = 234;
			this.splitContainer1.TabIndex = 0;
			this.splitContainer1.TabStop = false;
			// 
			// _previewGroupBox
			// 
			this._previewGroupBox.Controls.Add(this._previewTableLayoutPanel);
			this._previewGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this._previewGroupBox.Location = new System.Drawing.Point(0, 0);
			this._previewGroupBox.Name = "_previewGroupBox";
			this._previewGroupBox.Size = new System.Drawing.Size(355, 314);
			this._previewGroupBox.TabIndex = 0;
			this._previewGroupBox.TabStop = false;
			this._previewGroupBox.Text = "Preview";
			// 
			// _previewTableLayoutPanel
			// 
			this._previewTableLayoutPanel.ColumnCount = 2;
			this._previewTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._previewTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this._previewTableLayoutPanel.Controls.Add(this._text, 0, 2);
			this._previewTableLayoutPanel.Controls.Add(this._category, 1, 1);
			this._previewTableLayoutPanel.Controls.Add(this._name, 0, 1);
			this._previewTableLayoutPanel.Controls.Add(this._group, 0, 0);
			this._previewTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._previewTableLayoutPanel.Location = new System.Drawing.Point(3, 16);
			this._previewTableLayoutPanel.Name = "_previewTableLayoutPanel";
			this._previewTableLayoutPanel.RowCount = 3;
			this._previewTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._previewTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._previewTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this._previewTableLayoutPanel.Size = new System.Drawing.Size(349, 295);
			this._previewTableLayoutPanel.TabIndex = 4;
			// 
			// _text
			// 
			this._previewTableLayoutPanel.SetColumnSpan(this._text, 2);
			this._text.Dock = System.Windows.Forms.DockStyle.Fill;
			this._text.LabelText = "Text";
			this._text.Location = new System.Drawing.Point(2, 92);
			this._text.Margin = new System.Windows.Forms.Padding(2);
			this._text.Name = "_text";
			this._text.ReadOnly = true;
			this._text.Size = new System.Drawing.Size(345, 201);
			this._text.TabIndex = 3;
			this._text.TabStop = false;
			this._text.Value = null;
			// 
			// _category
			// 
			this._category.Dock = System.Windows.Forms.DockStyle.Fill;
			this._category.LabelText = "Category";
			this._category.Location = new System.Drawing.Point(176, 47);
			this._category.Margin = new System.Windows.Forms.Padding(2);
			this._category.Mask = "";
			this._category.Name = "_category";
			this._category.PasswordChar = '\0';
			this._category.ReadOnly = true;
			this._category.Size = new System.Drawing.Size(171, 41);
			this._category.TabIndex = 2;
			this._category.TabStop = false;
			this._category.ToolTip = null;
			this._category.Value = null;
			// 
			// _name
			// 
			this._name.Dock = System.Windows.Forms.DockStyle.Fill;
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(2, 47);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.ReadOnly = true;
			this._name.Size = new System.Drawing.Size(170, 41);
			this._name.TabIndex = 1;
			this._name.TabStop = false;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _group
			// 
			this._previewTableLayoutPanel.SetColumnSpan(this._group, 2);
			this._group.Dock = System.Windows.Forms.DockStyle.Fill;
			this._group.LabelText = "Group";
			this._group.Location = new System.Drawing.Point(2, 2);
			this._group.Margin = new System.Windows.Forms.Padding(2);
			this._group.Mask = "";
			this._group.Name = "_group";
			this._group.PasswordChar = '\0';
			this._group.ReadOnly = true;
			this._group.Size = new System.Drawing.Size(345, 41);
			this._group.TabIndex = 0;
			this._group.TabStop = false;
			this._group.ToolTip = null;
			this._group.Value = null;
			// 
			// CannedTextSummaryComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "CannedTextSummaryComponentControl";
			this.Size = new System.Drawing.Size(355, 552);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this._previewGroupBox.ResumeLayout(false);
			this._previewTableLayoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _cannedTexts;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private ClearCanvas.Desktop.View.WinForms.TextAreaField _text;
		private System.Windows.Forms.GroupBox _previewGroupBox;
		private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private System.Windows.Forms.TableLayoutPanel _previewTableLayoutPanel;
		private ClearCanvas.Desktop.View.WinForms.TextField _category;
		private ClearCanvas.Desktop.View.WinForms.TextField _group;
    }
}
