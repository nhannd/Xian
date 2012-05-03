using ClearCanvas.Desktop.View.WinForms;
namespace ClearCanvas.ImageViewer.View.WinForms
{
	partial class ActivityMonitorComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivityMonitorComponentControl));
			this._workItemsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._overviewPanel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.panel2 = new System.Windows.Forms.Panel();
			this._aeTitle = new System.Windows.Forms.Label();
			this._hostName = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this._statusLight = new ClearCanvas.Desktop.View.WinForms.IndicatorLight();
			this.label11 = new System.Windows.Forms.Label();
			this._totalStudies = new System.Windows.Forms.Label();
			this._failures = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this._diskSpacePanel = new System.Windows.Forms.Panel();
			this._diskSpaceMeter = new ClearCanvas.Desktop.View.WinForms.Meter();
			this._diskSpaceWarningIcon = new System.Windows.Forms.PictureBox();
			this._diskSpaceWarningMessage = new System.Windows.Forms.Label();
			this._diskSpace = new System.Windows.Forms.Label();
			this._reindexLink = new System.Windows.Forms.LinkLabel();
			this._openFileStoreLink = new System.Windows.Forms.LinkLabel();
			this._localServerConfigLink = new System.Windows.Forms.LinkLabel();
			this._studyRulesLink = new System.Windows.Forms.LinkLabel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.panel1 = new System.Windows.Forms.Panel();
			this._workItemToolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this._activityFilter = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this._statusFilter = new System.Windows.Forms.ToolStripComboBox();
			this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
			this._textFilter = new System.Windows.Forms.ToolStripTextBox();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._overviewPanel.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.panel2.SuspendLayout();
			this._diskSpacePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._diskSpaceWarningIcon)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.panel1.SuspendLayout();
			this._workItemToolStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _workItemsTableView
			// 
			this._workItemsTableView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this._workItemsTableView.ColumnHeaderTooltip = null;
			this._workItemsTableView.Location = new System.Drawing.Point(0, 30);
			this._workItemsTableView.Margin = new System.Windows.Forms.Padding(4);
			this._workItemsTableView.Name = "_workItemsTableView";
			this._workItemsTableView.ReadOnly = false;
			this._workItemsTableView.ShowToolbar = false;
			this._workItemsTableView.Size = new System.Drawing.Size(1010, 475);
			this._workItemsTableView.SortButtonTooltip = null;
			this._workItemsTableView.TabIndex = 7;
			// 
			// _overviewPanel
			// 
			this._overviewPanel.BackColor = System.Drawing.SystemColors.Control;
			this._overviewPanel.Controls.Add(this.tableLayoutPanel2);
			this._overviewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._overviewPanel.Location = new System.Drawing.Point(0, 0);
			this._overviewPanel.Name = "_overviewPanel";
			this._overviewPanel.Size = new System.Drawing.Size(1010, 175);
			this._overviewPanel.TabIndex = 9;
			this._overviewPanel.Text = "Overview";
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 8;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 106F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 106F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 49F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 515F));
			this.tableLayoutPanel2.Controls.Add(this.label2, 7, 0);
			this.tableLayoutPanel2.Controls.Add(this.panel2, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.label10, 1, 2);
			this.tableLayoutPanel2.Controls.Add(this._statusLight, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.label11, 1, 3);
			this.tableLayoutPanel2.Controls.Add(this._totalStudies, 2, 2);
			this.tableLayoutPanel2.Controls.Add(this._failures, 2, 3);
			this.tableLayoutPanel2.Controls.Add(this.label1, 4, 2);
			this.tableLayoutPanel2.Controls.Add(this._diskSpacePanel, 4, 3);
			this.tableLayoutPanel2.Controls.Add(this._reindexLink, 7, 2);
			this.tableLayoutPanel2.Controls.Add(this._openFileStoreLink, 7, 1);
			this.tableLayoutPanel2.Controls.Add(this._localServerConfigLink, 7, 3);
			this.tableLayoutPanel2.Controls.Add(this._studyRulesLink, 7, 4);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 14);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 6;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(1004, 140);
			this.tableLayoutPanel2.TabIndex = 22;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(492, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(74, 13);
			this.label2.TabIndex = 33;
			this.label2.Text = "Quick Links";
			// 
			// panel2
			// 
			this.tableLayoutPanel2.SetColumnSpan(this.panel2, 5);
			this.panel2.Controls.Add(this._aeTitle);
			this.panel2.Controls.Add(this._hostName);
			this.panel2.Location = new System.Drawing.Point(30, 0);
			this.panel2.Margin = new System.Windows.Forms.Padding(0);
			this.panel2.Name = "panel2";
			this.tableLayoutPanel2.SetRowSpan(this.panel2, 2);
			this.panel2.Size = new System.Drawing.Size(410, 51);
			this.panel2.TabIndex = 34;
			// 
			// _aeTitle
			// 
			this._aeTitle.AutoSize = true;
			this._aeTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._aeTitle.Location = new System.Drawing.Point(3, 0);
			this._aeTitle.Name = "_aeTitle";
			this._aeTitle.Size = new System.Drawing.Size(117, 25);
			this._aeTitle.TabIndex = 20;
			this._aeTitle.Text = "AE_TITLE";
			// 
			// _hostName
			// 
			this._hostName.AutoSize = true;
			this._hostName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._hostName.Location = new System.Drawing.Point(4, 25);
			this._hostName.Name = "_hostName";
			this._hostName.Size = new System.Drawing.Size(103, 20);
			this._hostName.TabIndex = 19;
			this._hostName.Text = "host and port";
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label10.AutoSize = true;
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(33, 60);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(82, 13);
			this.label10.TabIndex = 24;
			this.label10.Text = "Total Studies";
			// 
			// _statusLight
			// 
			this._statusLight.AutoSize = true;
			this._statusLight.LightHoverTextGreen = "Local server running";
			this._statusLight.LightHoverTextRed = "Local server not running";
			this._statusLight.LightHoverTextYellow = null;
			this._statusLight.LinkHoverTextGreen = null;
			this._statusLight.LinkHoverTextRed = null;
			this._statusLight.LinkHoverTextYellow = null;
			this._statusLight.LinkVisible = false;
			this._statusLight.Location = new System.Drawing.Point(3, 3);
			this._statusLight.Name = "_statusLight";
			this._statusLight.Size = new System.Drawing.Size(24, 23);
			this._statusLight.TabIndex = 32;
			this._statusLight.Text = "Link";
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label11.AutoSize = true;
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label11.Location = new System.Drawing.Point(33, 82);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(51, 13);
			this.label11.TabIndex = 25;
			this.label11.Text = "Failures";
			// 
			// _totalStudies
			// 
			this._totalStudies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._totalStudies.AutoSize = true;
			this._totalStudies.Location = new System.Drawing.Point(125, 60);
			this._totalStudies.Name = "_totalStudies";
			this._totalStudies.Size = new System.Drawing.Size(63, 13);
			this._totalStudies.TabIndex = 26;
			this._totalStudies.Text = "total studies";
			this._totalStudies.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// _failures
			// 
			this._failures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._failures.AutoSize = true;
			this._failures.Location = new System.Drawing.Point(148, 82);
			this._failures.Name = "_failures";
			this._failures.Size = new System.Drawing.Size(40, 13);
			this._failures.TabIndex = 27;
			this._failures.Text = "failures";
			this._failures.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(231, 60);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 13);
			this.label1.TabIndex = 12;
			this.label1.Text = "Disk Usage";
			// 
			// _diskSpacePanel
			// 
			this.tableLayoutPanel2.SetColumnSpan(this._diskSpacePanel, 2);
			this._diskSpacePanel.Controls.Add(this._diskSpaceMeter);
			this._diskSpacePanel.Controls.Add(this._diskSpaceWarningIcon);
			this._diskSpacePanel.Controls.Add(this._diskSpaceWarningMessage);
			this._diskSpacePanel.Controls.Add(this._diskSpace);
			this._diskSpacePanel.Location = new System.Drawing.Point(228, 73);
			this._diskSpacePanel.Margin = new System.Windows.Forms.Padding(0);
			this._diskSpacePanel.Name = "_diskSpacePanel";
			this.tableLayoutPanel2.SetRowSpan(this._diskSpacePanel, 3);
			this._diskSpacePanel.Size = new System.Drawing.Size(212, 67);
			this._diskSpacePanel.TabIndex = 29;
			// 
			// _diskSpaceMeter
			// 
			this._diskSpaceMeter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._diskSpaceMeter.Location = new System.Drawing.Point(6, 22);
			this._diskSpaceMeter.Name = "_diskSpaceMeter";
			this._diskSpaceMeter.Size = new System.Drawing.Size(203, 17);
			this._diskSpaceMeter.TabIndex = 11;
			// 
			// _diskSpaceWarningIcon
			// 
			this._diskSpaceWarningIcon.Image = ((System.Drawing.Image)(resources.GetObject("_diskSpaceWarningIcon.Image")));
			this._diskSpaceWarningIcon.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this._diskSpaceWarningIcon.Location = new System.Drawing.Point(6, 44);
			this._diskSpaceWarningIcon.Name = "_diskSpaceWarningIcon";
			this._diskSpaceWarningIcon.Size = new System.Drawing.Size(16, 16);
			this._diskSpaceWarningIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this._diskSpaceWarningIcon.TabIndex = 30;
			this._diskSpaceWarningIcon.TabStop = false;
			// 
			// _diskSpaceWarningMessage
			// 
			this._diskSpaceWarningMessage.AutoEllipsis = true;
			this._diskSpaceWarningMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._diskSpaceWarningMessage.ForeColor = System.Drawing.Color.Red;
			this._diskSpaceWarningMessage.Location = new System.Drawing.Point(28, 44);
			this._diskSpaceWarningMessage.Name = "_diskSpaceWarningMessage";
			this._diskSpaceWarningMessage.Size = new System.Drawing.Size(181, 16);
			this._diskSpaceWarningMessage.TabIndex = 29;
			this._diskSpaceWarningMessage.Text = "don\'t translate me";
			this._diskSpaceWarningMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _diskSpace
			// 
			this._diskSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._diskSpace.Location = new System.Drawing.Point(6, 9);
			this._diskSpace.Name = "_diskSpace";
			this._diskSpace.Size = new System.Drawing.Size(203, 13);
			this._diskSpace.TabIndex = 28;
			this._diskSpace.Text = "disk space";
			this._diskSpace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _reindexLink
			// 
			this._reindexLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._reindexLink.AutoSize = true;
			this._reindexLink.Location = new System.Drawing.Point(492, 60);
			this._reindexLink.Name = "_reindexLink";
			this._reindexLink.Size = new System.Drawing.Size(91, 13);
			this._reindexLink.TabIndex = 25;
			this._reindexLink.TabStop = true;
			this._reindexLink.Text = "Re-index file store";
			this._reindexLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._reindexLink_LinkClicked);
			// 
			// _openFileStoreLink
			// 
			this._openFileStoreLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._openFileStoreLink.AutoSize = true;
			this._openFileStoreLink.Location = new System.Drawing.Point(492, 38);
			this._openFileStoreLink.Name = "_openFileStoreLink";
			this._openFileStoreLink.Size = new System.Drawing.Size(75, 13);
			this._openFileStoreLink.TabIndex = 35;
			this._openFileStoreLink.TabStop = true;
			this._openFileStoreLink.Text = "Open file store";
			this._openFileStoreLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._openFileStoreLink_LinkClicked);
			// 
			// _localServerConfigLink
			// 
			this._localServerConfigLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._localServerConfigLink.AutoSize = true;
			this._localServerConfigLink.Location = new System.Drawing.Point(492, 82);
			this._localServerConfigLink.Name = "_localServerConfigLink";
			this._localServerConfigLink.Size = new System.Drawing.Size(132, 13);
			this._localServerConfigLink.TabIndex = 23;
			this._localServerConfigLink.TabStop = true;
			this._localServerConfigLink.Text = "Local Server Configuration";
			this._localServerConfigLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._localServerConfigLink_LinkClicked);
			// 
			// _studyRulesLink
			// 
			this._studyRulesLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._studyRulesLink.AutoSize = true;
			this._studyRulesLink.Location = new System.Drawing.Point(492, 104);
			this._studyRulesLink.Name = "_studyRulesLink";
			this._studyRulesLink.Size = new System.Drawing.Size(129, 13);
			this._studyRulesLink.TabIndex = 24;
			this._studyRulesLink.TabStop = true;
			this._studyRulesLink.Text = "Study Management Rules";
			this._studyRulesLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._studyRulesLink_LinkClicked);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._overviewPanel);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.panel1);
			this.splitContainer1.Size = new System.Drawing.Size(1010, 683);
			this.splitContainer1.SplitterDistance = 175;
			this.splitContainer1.TabIndex = 10;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._workItemToolStrip);
			this.panel1.Controls.Add(this._workItemsTableView);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1010, 504);
			this.panel1.TabIndex = 9;
			// 
			// _workItemToolStrip
			// 
			this._workItemToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._workItemToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._workItemToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this._activityFilter,
            this.toolStripLabel2,
            this._statusFilter,
            this.toolStripLabel3,
            this._textFilter,
            this.toolStripSeparator1});
			this._workItemToolStrip.Location = new System.Drawing.Point(0, 0);
			this._workItemToolStrip.Name = "_workItemToolStrip";
			this._workItemToolStrip.Size = new System.Drawing.Size(1010, 25);
			this._workItemToolStrip.TabIndex = 0;
			this._workItemToolStrip.Text = "toolStrip1";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(47, 22);
			this.toolStripLabel1.Text = "Activity";
			// 
			// _activityFilter
			// 
			this._activityFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._activityFilter.Name = "_activityFilter";
			this._activityFilter.Size = new System.Drawing.Size(121, 25);
			// 
			// toolStripLabel2
			// 
			this.toolStripLabel2.Name = "toolStripLabel2";
			this.toolStripLabel2.Size = new System.Drawing.Size(39, 22);
			this.toolStripLabel2.Text = "Status";
			// 
			// _statusFilter
			// 
			this._statusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._statusFilter.Name = "_statusFilter";
			this._statusFilter.Size = new System.Drawing.Size(121, 25);
			// 
			// toolStripLabel3
			// 
			this.toolStripLabel3.Name = "toolStripLabel3";
			this.toolStripLabel3.Size = new System.Drawing.Size(42, 22);
			this.toolStripLabel3.Text = "Search";
			// 
			// _textFilter
			// 
			this._textFilter.Name = "_textFilter";
			this._textFilter.Size = new System.Drawing.Size(100, 25);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// ActivityMonitorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "ActivityMonitorComponentControl";
			this.Size = new System.Drawing.Size(1010, 683);
			this._overviewPanel.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this._diskSpacePanel.ResumeLayout(false);
			this._diskSpacePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._diskSpaceWarningIcon)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this._workItemToolStrip.ResumeLayout(false);
			this._workItemToolStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _workItemsTableView;
		private System.Windows.Forms.Panel _overviewPanel;
		private System.Windows.Forms.Label label1;
		private Meter _diskSpaceMeter;
		private System.Windows.Forms.Label _hostName;
		private System.Windows.Forms.LinkLabel _studyRulesLink;
		private System.Windows.Forms.LinkLabel _localServerConfigLink;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label _aeTitle;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label _totalStudies;
		private System.Windows.Forms.Label _failures;
		private System.Windows.Forms.Label _diskSpace;
		private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel _diskSpacePanel;
		private IndicatorLight _statusLight;
		private System.Windows.Forms.LinkLabel _reindexLink;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.ToolTip _toolTip;
		private System.Windows.Forms.LinkLabel _openFileStoreLink;
        private System.Windows.Forms.Label _diskSpaceWarningMessage;
        private System.Windows.Forms.PictureBox _diskSpaceWarningIcon;
        private System.Windows.Forms.ToolStrip _workItemToolStrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox _activityFilter;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox _statusFilter;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripTextBox _textFilter;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

	}
}
