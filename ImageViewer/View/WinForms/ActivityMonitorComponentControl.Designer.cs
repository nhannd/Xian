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
            this._workItemsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this._statusLight = new ClearCanvas.Desktop.View.WinForms.IndicatorLight();
            this.label1 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this._totalStudies = new System.Windows.Forms.Label();
            this._failures = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this._aeTitle = new System.Windows.Forms.Label();
            this._hostName = new System.Windows.Forms.Label();
            this._port = new System.Windows.Forms.Label();
            this._fileStore = new System.Windows.Forms.Label();
            this._diskSpacePanel = new System.Windows.Forms.Panel();
            this._diskSpaceMeter = new ClearCanvas.Desktop.View.WinForms.Meter();
            this._diskSpace = new System.Windows.Forms.Label();
            this._configurationHeader = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this._reindexButton = new System.Windows.Forms.Button();
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
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            this._diskSpacePanel.SuspendLayout();
            this._configurationHeader.SuspendLayout();
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
            this._workItemsTableView.Location = new System.Drawing.Point(0, 29);
            this._workItemsTableView.Margin = new System.Windows.Forms.Padding(4);
            this._workItemsTableView.MultiSelect = false;
            this._workItemsTableView.Name = "_workItemsTableView";
            this._workItemsTableView.ReadOnly = false;
            this._workItemsTableView.ShowToolbar = false;
            this._workItemsTableView.Size = new System.Drawing.Size(1010, 426);
            this._workItemsTableView.SortButtonTooltip = null;
            this._workItemsTableView.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.linkLabel2);
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1010, 224);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Overview";
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(853, 57);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(64, 13);
            this.linkLabel2.TabIndex = 24;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "Study Rules";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 91F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 533F));
            this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.label10, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label11, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this._totalStudies, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this._failures, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.label2, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 3, 2);
            this.tableLayoutPanel2.Controls.Add(this.label4, 3, 3);
            this.tableLayoutPanel2.Controls.Add(this.label8, 3, 4);
            this.tableLayoutPanel2.Controls.Add(this._aeTitle, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this._hostName, 4, 2);
            this.tableLayoutPanel2.Controls.Add(this._port, 4, 3);
            this.tableLayoutPanel2.Controls.Add(this._fileStore, 4, 4);
            this.tableLayoutPanel2.Controls.Add(this._diskSpacePanel, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this._configurationHeader, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this._reindexButton, 4, 5);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(10, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 6;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(837, 148);
            this.tableLayoutPanel2.TabIndex = 22;
            // 
            // panel2
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.panel2, 2);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this._statusLight);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(161, 40);
            this.panel2.TabIndex = 31;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 4);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 16);
            this.label6.TabIndex = 17;
            this.label6.Text = "Status";
            // 
            // _statusLight
            // 
            this._statusLight.AutoSize = true;
            this._statusLight.LightHoverTextGreen = "Running";
            this._statusLight.LightHoverTextRed = "Not Running";
            this._statusLight.LightHoverTextYellow = null;
            this._statusLight.LinkHoverTextGreen = null;
            this._statusLight.LinkHoverTextRed = null;
            this._statusLight.LinkHoverTextYellow = null;
            this._statusLight.LinkVisible = false;
            this._statusLight.Location = new System.Drawing.Point(130, 0);
            this._statusLight.Name = "_statusLight";
            this._statusLight.Size = new System.Drawing.Size(31, 23);
            this._statusLight.TabIndex = 32;
            this._statusLight.Text = "Link";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Disk Usage";
            // 
            // label10
            // 
            this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(3, 48);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(82, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "Total Studies";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(3, 69);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(51, 13);
            this.label11.TabIndex = 25;
            this.label11.Text = "Failures";
            // 
            // _totalStudies
            // 
            this._totalStudies.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._totalStudies.AutoSize = true;
            this._totalStudies.Location = new System.Drawing.Point(95, 48);
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
            this._failures.Location = new System.Drawing.Point(118, 69);
            this._failures.Name = "_failures";
            this._failures.Size = new System.Drawing.Size(40, 13);
            this._failures.TabIndex = 27;
            this._failures.Text = "failures";
            this._failures.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(224, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "AE Title";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(224, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Hostname";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(224, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Port";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(224, 111);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(59, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "File store";
            // 
            // _aeTitle
            // 
            this._aeTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._aeTitle.AutoSize = true;
            this._aeTitle.Location = new System.Drawing.Point(307, 48);
            this._aeTitle.Name = "_aeTitle";
            this._aeTitle.Size = new System.Drawing.Size(38, 13);
            this._aeTitle.TabIndex = 20;
            this._aeTitle.Text = "ae title";
            this._aeTitle.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _hostName
            // 
            this._hostName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._hostName.AutoSize = true;
            this._hostName.Location = new System.Drawing.Point(307, 69);
            this._hostName.Name = "_hostName";
            this._hostName.Size = new System.Drawing.Size(56, 13);
            this._hostName.TabIndex = 19;
            this._hostName.Text = "host name";
            this._hostName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _port
            // 
            this._port.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._port.AutoSize = true;
            this._port.Location = new System.Drawing.Point(307, 90);
            this._port.Name = "_port";
            this._port.Size = new System.Drawing.Size(25, 13);
            this._port.TabIndex = 21;
            this._port.Text = "port";
            this._port.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _fileStore
            // 
            this._fileStore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._fileStore.AutoSize = true;
            this._fileStore.Location = new System.Drawing.Point(307, 111);
            this._fileStore.Name = "_fileStore";
            this._fileStore.Size = new System.Drawing.Size(46, 13);
            this._fileStore.TabIndex = 23;
            this._fileStore.Text = "file store";
            this._fileStore.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // _diskSpacePanel
            // 
            this.tableLayoutPanel2.SetColumnSpan(this._diskSpacePanel, 2);
            this._diskSpacePanel.Controls.Add(this._diskSpaceMeter);
            this._diskSpacePanel.Controls.Add(this._diskSpace);
            this._diskSpacePanel.Location = new System.Drawing.Point(0, 103);
            this._diskSpacePanel.Margin = new System.Windows.Forms.Padding(0);
            this._diskSpacePanel.Name = "_diskSpacePanel";
            this.tableLayoutPanel2.SetRowSpan(this._diskSpacePanel, 2);
            this._diskSpacePanel.Size = new System.Drawing.Size(161, 45);
            this._diskSpacePanel.TabIndex = 29;
            // 
            // _diskSpaceMeter
            // 
            this._diskSpaceMeter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._diskSpaceMeter.Location = new System.Drawing.Point(3, 17);
            this._diskSpaceMeter.Name = "_diskSpaceMeter";
            this._diskSpaceMeter.Size = new System.Drawing.Size(155, 15);
            this._diskSpaceMeter.TabIndex = 11;
            // 
            // _diskSpace
            // 
            this._diskSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._diskSpace.Location = new System.Drawing.Point(3, 3);
            this._diskSpace.Name = "_diskSpace";
            this._diskSpace.Size = new System.Drawing.Size(155, 13);
            this._diskSpace.TabIndex = 28;
            this._diskSpace.Text = "diskspace";
            this._diskSpace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _configurationHeader
            // 
            this.tableLayoutPanel2.SetColumnSpan(this._configurationHeader, 2);
            this._configurationHeader.Controls.Add(this.label5);
            this._configurationHeader.Controls.Add(this.linkLabel1);
            this._configurationHeader.Location = new System.Drawing.Point(221, 0);
            this._configurationHeader.Margin = new System.Windows.Forms.Padding(0);
            this._configurationHeader.Name = "_configurationHeader";
            this._configurationHeader.Size = new System.Drawing.Size(200, 40);
            this._configurationHeader.TabIndex = 30;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 16);
            this.label5.TabIndex = 17;
            this.label5.Text = "Configuration";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(108, 6);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(24, 13);
            this.linkLabel1.TabIndex = 23;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "edit";
            // 
            // _reindexButton
            // 
            this._reindexButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._reindexButton.Location = new System.Drawing.Point(304, 128);
            this._reindexButton.Margin = new System.Windows.Forms.Padding(0);
            this._reindexButton.Name = "_reindexButton";
            this._reindexButton.Size = new System.Drawing.Size(59, 20);
            this._reindexButton.TabIndex = 32;
            this._reindexButton.Text = "Re-index";
            this._reindexButton.UseVisualStyleBackColor = true;
            this._reindexButton.Click += new System.EventHandler(this.Reindex_Click);
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
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(1010, 683);
            this.splitContainer1.SplitterDistance = 224;
            this.splitContainer1.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._workItemToolStrip);
            this.panel1.Controls.Add(this._workItemsTableView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1010, 455);
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
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this._diskSpacePanel.ResumeLayout(false);
            this._configurationHeader.ResumeLayout(false);
            this._configurationHeader.PerformLayout();
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
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private Meter _diskSpaceMeter;
		private System.Windows.Forms.Label _hostName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.LinkLabel linkLabel2;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label _port;
		private System.Windows.Forms.Label _aeTitle;
		private System.Windows.Forms.Label _fileStore;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label _totalStudies;
		private System.Windows.Forms.Label _failures;
		private System.Windows.Forms.Label _diskSpace;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ToolStrip _workItemToolStrip;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripComboBox _activityFilter;
		private System.Windows.Forms.ToolStripLabel toolStripLabel2;
		private System.Windows.Forms.ToolStripComboBox _statusFilter;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripLabel toolStripLabel3;
		private System.Windows.Forms.ToolStripTextBox _textFilter;
		private System.Windows.Forms.Panel _diskSpacePanel;
		private System.Windows.Forms.Panel _configurationHeader;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private IndicatorLight _statusLight;
        private System.Windows.Forms.Button _reindexButton;

	}
}
