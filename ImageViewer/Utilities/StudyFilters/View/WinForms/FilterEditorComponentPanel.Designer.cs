namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms {
	partial class FilterEditorComponentPanel {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.GroupBox _grpAvailableColumns;
			System.Windows.Forms.GroupBox _grpDicomColumns;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FilterEditorComponentPanel));
			System.Windows.Forms.GroupBox _grpDicomTag;
			System.Windows.Forms.Label _lblDicomTagElement;
			System.Windows.Forms.Label _lblDicomTagGroup;
			System.Windows.Forms.GroupBox _grpSpecialColumns;
			System.Windows.Forms.GroupBox _grpSelectedColumns;
			this._btnAddDicomColumn = new System.Windows.Forms.Button();
			this._icons = new System.Windows.Forms.ImageList(this.components);
			this._lstDicomColumns = new System.Windows.Forms.ListBox();
			this._txtFilterDicomColumns = new System.Windows.Forms.TextBox();
			this._splitSpecialColumnsResizer = new System.Windows.Forms.Splitter();
			this._btnAddDicomTag = new System.Windows.Forms.Button();
			this._txtDicomTagElement = new System.Windows.Forms.TextBox();
			this._txtDicomTagGroup = new System.Windows.Forms.TextBox();
			this._btnAddSpecialColumn = new System.Windows.Forms.Button();
			this._lstSpecialColumns = new System.Windows.Forms.ListBox();
			this._btnMoveColumnDown = new System.Windows.Forms.Button();
			this._btnMoveColumnUp = new System.Windows.Forms.Button();
			this._btnDelColumn = new System.Windows.Forms.Button();
			this._lstFilters = new System.Windows.Forms.ListBox();
			this.pnlSplitter = new System.Windows.Forms.SplitContainer();
			this._tooltips = new System.Windows.Forms.ToolTip(this.components);
			this._cboSpecialColumnOperators = new System.Windows.Forms.ComboBox();
			this._txtSpecialColumnsValue = new System.Windows.Forms.TextBox();
			this._txtDicomColumnsValue = new System.Windows.Forms.TextBox();
			this._cboDicomColumnsOperator = new System.Windows.Forms.ComboBox();
			this._txtDicomTagValue = new System.Windows.Forms.TextBox();
			this._cboDicomTagOperators = new System.Windows.Forms.ComboBox();
			_grpAvailableColumns = new System.Windows.Forms.GroupBox();
			_grpDicomColumns = new System.Windows.Forms.GroupBox();
			_grpDicomTag = new System.Windows.Forms.GroupBox();
			_lblDicomTagElement = new System.Windows.Forms.Label();
			_lblDicomTagGroup = new System.Windows.Forms.Label();
			_grpSpecialColumns = new System.Windows.Forms.GroupBox();
			_grpSelectedColumns = new System.Windows.Forms.GroupBox();
			_grpAvailableColumns.SuspendLayout();
			_grpDicomColumns.SuspendLayout();
			_grpDicomTag.SuspendLayout();
			_grpSpecialColumns.SuspendLayout();
			_grpSelectedColumns.SuspendLayout();
			this.pnlSplitter.Panel1.SuspendLayout();
			this.pnlSplitter.Panel2.SuspendLayout();
			this.pnlSplitter.SuspendLayout();
			this.SuspendLayout();
			// 
			// _grpAvailableColumns
			// 
			_grpAvailableColumns.Controls.Add(_grpDicomColumns);
			_grpAvailableColumns.Controls.Add(this._splitSpecialColumnsResizer);
			_grpAvailableColumns.Controls.Add(_grpDicomTag);
			_grpAvailableColumns.Controls.Add(_grpSpecialColumns);
			_grpAvailableColumns.Dock = System.Windows.Forms.DockStyle.Fill;
			_grpAvailableColumns.Location = new System.Drawing.Point(0, 0);
			_grpAvailableColumns.Name = "_grpAvailableColumns";
			_grpAvailableColumns.Size = new System.Drawing.Size(566, 443);
			_grpAvailableColumns.TabIndex = 0;
			_grpAvailableColumns.TabStop = false;
			_grpAvailableColumns.Text = "Available Columns";
			// 
			// _grpDicomColumns
			// 
			_grpDicomColumns.Controls.Add(this._txtDicomColumnsValue);
			_grpDicomColumns.Controls.Add(this._cboDicomColumnsOperator);
			_grpDicomColumns.Controls.Add(this._btnAddDicomColumn);
			_grpDicomColumns.Controls.Add(this._lstDicomColumns);
			_grpDicomColumns.Controls.Add(this._txtFilterDicomColumns);
			_grpDicomColumns.Dock = System.Windows.Forms.DockStyle.Fill;
			_grpDicomColumns.Location = new System.Drawing.Point(3, 137);
			_grpDicomColumns.Name = "_grpDicomColumns";
			_grpDicomColumns.Size = new System.Drawing.Size(560, 244);
			_grpDicomColumns.TabIndex = 8;
			_grpDicomColumns.TabStop = false;
			_grpDicomColumns.Text = "DICOM Columns";
			// 
			// _btnAddDicomColumn
			// 
			this._btnAddDicomColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAddDicomColumn.ImageKey = "BuilderAdd.bmp";
			this._btnAddDicomColumn.ImageList = this._icons;
			this._btnAddDicomColumn.Location = new System.Drawing.Point(522, 19);
			this._btnAddDicomColumn.Name = "_btnAddDicomColumn";
			this._btnAddDicomColumn.Size = new System.Drawing.Size(32, 31);
			this._btnAddDicomColumn.TabIndex = 3;
			this._tooltips.SetToolTip(this._btnAddDicomColumn, "Add");
			this._btnAddDicomColumn.UseVisualStyleBackColor = true;
			this._btnAddDicomColumn.Click += new System.EventHandler(this.OnAddDicomColumnClick);
			// 
			// _icons
			// 
			this._icons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_icons.ImageStream")));
			this._icons.TransparentColor = System.Drawing.Color.Magenta;
			this._icons.Images.SetKeyName(0, "BuilderAdd.bmp");
			this._icons.Images.SetKeyName(1, "BuilderDelete.bmp");
			this._icons.Images.SetKeyName(2, "BuilderMoveDown.bmp");
			this._icons.Images.SetKeyName(3, "BuilderMoveUp.bmp");
			// 
			// _lstDicomColumns
			// 
			this._lstDicomColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lstDicomColumns.FormattingEnabled = true;
			this._lstDicomColumns.Location = new System.Drawing.Point(6, 45);
			this._lstDicomColumns.Name = "_lstDicomColumns";
			this._lstDicomColumns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this._lstDicomColumns.Size = new System.Drawing.Size(338, 186);
			this._lstDicomColumns.Sorted = true;
			this._lstDicomColumns.TabIndex = 2;
			this._lstDicomColumns.DoubleClick += new System.EventHandler(this._lstDicomColumns_DoubleClick);
			this._lstDicomColumns.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this._lstDicomColumns_PreviewKeyDown);
			this._lstDicomColumns.KeyDown += new System.Windows.Forms.KeyEventHandler(this._lstDicomColumns_KeyDown);
			// 
			// _txtFilterDicomColumns
			// 
			this._txtFilterDicomColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._txtFilterDicomColumns.Location = new System.Drawing.Point(6, 19);
			this._txtFilterDicomColumns.Name = "_txtFilterDicomColumns";
			this._txtFilterDicomColumns.Size = new System.Drawing.Size(338, 20);
			this._txtFilterDicomColumns.TabIndex = 1;
			this._tooltips.SetToolTip(this._txtFilterDicomColumns, "Search DICOM tags");
			this._txtFilterDicomColumns.TextChanged += new System.EventHandler(this._txtFilterDicomColumns_TextChanged);
			this._txtFilterDicomColumns.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this._txtFilterDicomColumns_PreviewKeyDown);
			this._txtFilterDicomColumns.KeyDown += new System.Windows.Forms.KeyEventHandler(this._txtFilterDicomColumns_KeyDown);
			// 
			// _splitSpecialColumnsResizer
			// 
			this._splitSpecialColumnsResizer.Dock = System.Windows.Forms.DockStyle.Top;
			this._splitSpecialColumnsResizer.Location = new System.Drawing.Point(3, 134);
			this._splitSpecialColumnsResizer.Name = "_splitSpecialColumnsResizer";
			this._splitSpecialColumnsResizer.Size = new System.Drawing.Size(560, 3);
			this._splitSpecialColumnsResizer.TabIndex = 10;
			this._splitSpecialColumnsResizer.TabStop = false;
			// 
			// _grpDicomTag
			// 
			_grpDicomTag.Controls.Add(this._txtDicomTagValue);
			_grpDicomTag.Controls.Add(this._cboDicomTagOperators);
			_grpDicomTag.Controls.Add(this._btnAddDicomTag);
			_grpDicomTag.Controls.Add(this._txtDicomTagElement);
			_grpDicomTag.Controls.Add(_lblDicomTagElement);
			_grpDicomTag.Controls.Add(this._txtDicomTagGroup);
			_grpDicomTag.Controls.Add(_lblDicomTagGroup);
			_grpDicomTag.Dock = System.Windows.Forms.DockStyle.Bottom;
			_grpDicomTag.Location = new System.Drawing.Point(3, 381);
			_grpDicomTag.Name = "_grpDicomTag";
			_grpDicomTag.Size = new System.Drawing.Size(560, 59);
			_grpDicomTag.TabIndex = 9;
			_grpDicomTag.TabStop = false;
			_grpDicomTag.Text = "Custom DICOM Columns";
			// 
			// _btnAddDicomTag
			// 
			this._btnAddDicomTag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAddDicomTag.ImageKey = "BuilderAdd.bmp";
			this._btnAddDicomTag.ImageList = this._icons;
			this._btnAddDicomTag.Location = new System.Drawing.Point(522, 19);
			this._btnAddDicomTag.Name = "_btnAddDicomTag";
			this._btnAddDicomTag.Size = new System.Drawing.Size(32, 31);
			this._btnAddDicomTag.TabIndex = 4;
			this._tooltips.SetToolTip(this._btnAddDicomTag, "Add");
			this._btnAddDicomTag.UseVisualStyleBackColor = true;
			this._btnAddDicomTag.Click += new System.EventHandler(this.OnAddDicomTagClick);
			// 
			// _txtDicomTagElement
			// 
			this._txtDicomTagElement.Location = new System.Drawing.Point(155, 25);
			this._txtDicomTagElement.MaxLength = 4;
			this._txtDicomTagElement.Name = "_txtDicomTagElement";
			this._txtDicomTagElement.Size = new System.Drawing.Size(51, 20);
			this._txtDicomTagElement.TabIndex = 3;
			this._txtDicomTagElement.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this._txtDicomTagElement_PreviewKeyDown);
			this._txtDicomTagElement.KeyDown += new System.Windows.Forms.KeyEventHandler(this._txtDicomTagElement_KeyDown);
			// 
			// _lblDicomTagElement
			// 
			_lblDicomTagElement.AutoSize = true;
			_lblDicomTagElement.Location = new System.Drawing.Point(104, 28);
			_lblDicomTagElement.Name = "_lblDicomTagElement";
			_lblDicomTagElement.Size = new System.Drawing.Size(45, 13);
			_lblDicomTagElement.TabIndex = 2;
			_lblDicomTagElement.Text = "Element";
			// 
			// _txtDicomTagGroup
			// 
			this._txtDicomTagGroup.Location = new System.Drawing.Point(48, 25);
			this._txtDicomTagGroup.MaxLength = 4;
			this._txtDicomTagGroup.Name = "_txtDicomTagGroup";
			this._txtDicomTagGroup.Size = new System.Drawing.Size(51, 20);
			this._txtDicomTagGroup.TabIndex = 1;
			this._txtDicomTagGroup.TextChanged += new System.EventHandler(this._txtDicomTagGroup_TextChanged);
			this._txtDicomTagGroup.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this._txtDicomTagGroup_PreviewKeyDown);
			this._txtDicomTagGroup.KeyDown += new System.Windows.Forms.KeyEventHandler(this._txtDicomTagGroup_KeyDown);
			// 
			// _lblDicomTagGroup
			// 
			_lblDicomTagGroup.AutoSize = true;
			_lblDicomTagGroup.Location = new System.Drawing.Point(6, 28);
			_lblDicomTagGroup.Name = "_lblDicomTagGroup";
			_lblDicomTagGroup.Size = new System.Drawing.Size(36, 13);
			_lblDicomTagGroup.TabIndex = 0;
			_lblDicomTagGroup.Text = "Group";
			// 
			// _grpSpecialColumns
			// 
			_grpSpecialColumns.Controls.Add(this._txtSpecialColumnsValue);
			_grpSpecialColumns.Controls.Add(this._cboSpecialColumnOperators);
			_grpSpecialColumns.Controls.Add(this._btnAddSpecialColumn);
			_grpSpecialColumns.Controls.Add(this._lstSpecialColumns);
			_grpSpecialColumns.Dock = System.Windows.Forms.DockStyle.Top;
			_grpSpecialColumns.Location = new System.Drawing.Point(3, 16);
			_grpSpecialColumns.Name = "_grpSpecialColumns";
			_grpSpecialColumns.Size = new System.Drawing.Size(560, 118);
			_grpSpecialColumns.TabIndex = 7;
			_grpSpecialColumns.TabStop = false;
			_grpSpecialColumns.Text = "Special Columns";
			// 
			// _btnAddSpecialColumn
			// 
			this._btnAddSpecialColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnAddSpecialColumn.ImageIndex = 0;
			this._btnAddSpecialColumn.ImageList = this._icons;
			this._btnAddSpecialColumn.Location = new System.Drawing.Point(522, 19);
			this._btnAddSpecialColumn.Name = "_btnAddSpecialColumn";
			this._btnAddSpecialColumn.Size = new System.Drawing.Size(32, 31);
			this._btnAddSpecialColumn.TabIndex = 1;
			this._tooltips.SetToolTip(this._btnAddSpecialColumn, "Add");
			this._btnAddSpecialColumn.UseVisualStyleBackColor = true;
			this._btnAddSpecialColumn.Click += new System.EventHandler(this.OnAddSpecialColumnClick);
			// 
			// _lstSpecialColumns
			// 
			this._lstSpecialColumns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lstSpecialColumns.FormattingEnabled = true;
			this._lstSpecialColumns.Location = new System.Drawing.Point(6, 19);
			this._lstSpecialColumns.Name = "_lstSpecialColumns";
			this._lstSpecialColumns.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this._lstSpecialColumns.Size = new System.Drawing.Size(338, 82);
			this._lstSpecialColumns.TabIndex = 0;
			this._lstSpecialColumns.DoubleClick += new System.EventHandler(this._lstSpecialColumns_DoubleClick);
			this._lstSpecialColumns.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this._lstSpecialColumns_PreviewKeyDown);
			this._lstSpecialColumns.KeyDown += new System.Windows.Forms.KeyEventHandler(this._lstSpecialColumns_KeyDown);
			// 
			// _grpSelectedColumns
			// 
			_grpSelectedColumns.Controls.Add(this._btnMoveColumnDown);
			_grpSelectedColumns.Controls.Add(this._btnMoveColumnUp);
			_grpSelectedColumns.Controls.Add(this._btnDelColumn);
			_grpSelectedColumns.Controls.Add(this._lstFilters);
			_grpSelectedColumns.Dock = System.Windows.Forms.DockStyle.Fill;
			_grpSelectedColumns.Location = new System.Drawing.Point(0, 0);
			_grpSelectedColumns.Name = "_grpSelectedColumns";
			_grpSelectedColumns.Size = new System.Drawing.Size(330, 443);
			_grpSelectedColumns.TabIndex = 1;
			_grpSelectedColumns.TabStop = false;
			_grpSelectedColumns.Text = "Selected Columns";
			// 
			// _btnMoveColumnDown
			// 
			this._btnMoveColumnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnMoveColumnDown.ImageKey = "BuilderMoveDown.bmp";
			this._btnMoveColumnDown.ImageList = this._icons;
			this._btnMoveColumnDown.Location = new System.Drawing.Point(292, 167);
			this._btnMoveColumnDown.Name = "_btnMoveColumnDown";
			this._btnMoveColumnDown.Size = new System.Drawing.Size(32, 31);
			this._btnMoveColumnDown.TabIndex = 5;
			this._tooltips.SetToolTip(this._btnMoveColumnDown, "Move Down");
			this._btnMoveColumnDown.UseVisualStyleBackColor = true;
			this._btnMoveColumnDown.Click += new System.EventHandler(this._btnMoveColumnDown_Click);
			// 
			// _btnMoveColumnUp
			// 
			this._btnMoveColumnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnMoveColumnUp.ImageKey = "BuilderMoveUp.bmp";
			this._btnMoveColumnUp.ImageList = this._icons;
			this._btnMoveColumnUp.Location = new System.Drawing.Point(292, 130);
			this._btnMoveColumnUp.Name = "_btnMoveColumnUp";
			this._btnMoveColumnUp.Size = new System.Drawing.Size(32, 31);
			this._btnMoveColumnUp.TabIndex = 4;
			this._tooltips.SetToolTip(this._btnMoveColumnUp, "Move Up");
			this._btnMoveColumnUp.UseVisualStyleBackColor = true;
			this._btnMoveColumnUp.Click += new System.EventHandler(this._btnMoveColumnUp_Click);
			// 
			// _btnDelColumn
			// 
			this._btnDelColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._btnDelColumn.ImageKey = "BuilderDelete.bmp";
			this._btnDelColumn.ImageList = this._icons;
			this._btnDelColumn.Location = new System.Drawing.Point(292, 93);
			this._btnDelColumn.Name = "_btnDelColumn";
			this._btnDelColumn.Size = new System.Drawing.Size(32, 31);
			this._btnDelColumn.TabIndex = 3;
			this._tooltips.SetToolTip(this._btnDelColumn, "Delete");
			this._btnDelColumn.UseVisualStyleBackColor = true;
			this._btnDelColumn.Click += new System.EventHandler(this._btnDelColumn_Click);
			// 
			// _lstFilters
			// 
			this._lstFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._lstFilters.FormattingEnabled = true;
			this._lstFilters.IntegralHeight = false;
			this._lstFilters.Location = new System.Drawing.Point(6, 19);
			this._lstFilters.Name = "_lstFilters";
			this._lstFilters.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this._lstFilters.Size = new System.Drawing.Size(280, 418);
			this._lstFilters.TabIndex = 0;
			// 
			// pnlSplitter
			// 
			this.pnlSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlSplitter.Location = new System.Drawing.Point(0, 0);
			this.pnlSplitter.Name = "pnlSplitter";
			// 
			// pnlSplitter.Panel1
			// 
			this.pnlSplitter.Panel1.Controls.Add(_grpAvailableColumns);
			// 
			// pnlSplitter.Panel2
			// 
			this.pnlSplitter.Panel2.Controls.Add(_grpSelectedColumns);
			this.pnlSplitter.Size = new System.Drawing.Size(900, 443);
			this.pnlSplitter.SplitterDistance = 566;
			this.pnlSplitter.TabIndex = 2;
			// 
			// _cboSpecialColumnOperators
			// 
			this._cboSpecialColumnOperators.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cboSpecialColumnOperators.FormattingEnabled = true;
			this._cboSpecialColumnOperators.Location = new System.Drawing.Point(350, 19);
			this._cboSpecialColumnOperators.Name = "_cboSpecialColumnOperators";
			this._cboSpecialColumnOperators.Size = new System.Drawing.Size(64, 21);
			this._cboSpecialColumnOperators.TabIndex = 2;
			// 
			// _txtSpecialColumnsValue
			// 
			this._txtSpecialColumnsValue.Location = new System.Drawing.Point(420, 19);
			this._txtSpecialColumnsValue.Name = "_txtSpecialColumnsValue";
			this._txtSpecialColumnsValue.Size = new System.Drawing.Size(96, 20);
			this._txtSpecialColumnsValue.TabIndex = 3;
			// 
			// _txtDicomColumnsValue
			// 
			this._txtDicomColumnsValue.Location = new System.Drawing.Point(420, 19);
			this._txtDicomColumnsValue.Name = "_txtDicomColumnsValue";
			this._txtDicomColumnsValue.Size = new System.Drawing.Size(96, 20);
			this._txtDicomColumnsValue.TabIndex = 5;
			// 
			// _cboDicomColumnsOperator
			// 
			this._cboDicomColumnsOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cboDicomColumnsOperator.FormattingEnabled = true;
			this._cboDicomColumnsOperator.Location = new System.Drawing.Point(350, 19);
			this._cboDicomColumnsOperator.Name = "_cboDicomColumnsOperator";
			this._cboDicomColumnsOperator.Size = new System.Drawing.Size(64, 21);
			this._cboDicomColumnsOperator.TabIndex = 4;
			// 
			// _txtDicomTagValue
			// 
			this._txtDicomTagValue.Location = new System.Drawing.Point(420, 25);
			this._txtDicomTagValue.Name = "_txtDicomTagValue";
			this._txtDicomTagValue.Size = new System.Drawing.Size(96, 20);
			this._txtDicomTagValue.TabIndex = 6;
			// 
			// _cboDicomTagOperators
			// 
			this._cboDicomTagOperators.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cboDicomTagOperators.FormattingEnabled = true;
			this._cboDicomTagOperators.Location = new System.Drawing.Point(350, 25);
			this._cboDicomTagOperators.Name = "_cboDicomTagOperators";
			this._cboDicomTagOperators.Size = new System.Drawing.Size(64, 21);
			this._cboDicomTagOperators.TabIndex = 5;
			// 
			// FilterEditorComponentPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlSplitter);
			this.Name = "FilterEditorComponentPanel";
			this.Size = new System.Drawing.Size(900, 443);
			_grpAvailableColumns.ResumeLayout(false);
			_grpDicomColumns.ResumeLayout(false);
			_grpDicomColumns.PerformLayout();
			_grpDicomTag.ResumeLayout(false);
			_grpDicomTag.PerformLayout();
			_grpSpecialColumns.ResumeLayout(false);
			_grpSpecialColumns.PerformLayout();
			_grpSelectedColumns.ResumeLayout(false);
			this.pnlSplitter.Panel1.ResumeLayout(false);
			this.pnlSplitter.Panel2.ResumeLayout(false);
			this.pnlSplitter.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListBox _lstFilters;
		private System.Windows.Forms.SplitContainer pnlSplitter;
		private System.Windows.Forms.ListBox _lstSpecialColumns;
		private System.Windows.Forms.Button _btnAddSpecialColumn;
		private System.Windows.Forms.Splitter _splitSpecialColumnsResizer;
		private System.Windows.Forms.Button _btnAddDicomColumn;
		private System.Windows.Forms.ListBox _lstDicomColumns;
		private System.Windows.Forms.TextBox _txtFilterDicomColumns;
		private System.Windows.Forms.Button _btnAddDicomTag;
		private System.Windows.Forms.TextBox _txtDicomTagElement;
		private System.Windows.Forms.TextBox _txtDicomTagGroup;
		private System.Windows.Forms.Button _btnMoveColumnDown;
		private System.Windows.Forms.Button _btnMoveColumnUp;
		private System.Windows.Forms.Button _btnDelColumn;
		private System.Windows.Forms.ImageList _icons;
		private System.Windows.Forms.ToolTip _tooltips;
		private System.Windows.Forms.ComboBox _cboSpecialColumnOperators;
		private System.Windows.Forms.TextBox _txtDicomColumnsValue;
		private System.Windows.Forms.ComboBox _cboDicomColumnsOperator;
		private System.Windows.Forms.TextBox _txtDicomTagValue;
		private System.Windows.Forms.ComboBox _cboDicomTagOperators;
		private System.Windows.Forms.TextBox _txtSpecialColumnsValue;
	}
}
