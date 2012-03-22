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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivityMonitorComponentControl));
			this._workItemsTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._searchButton = new System.Windows.Forms.Button();
			this._clearButton = new System.Windows.Forms.Button();
			this.comboBoxField1 = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.textField3 = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.textField2 = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.textField1 = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._ruleName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.linkLabel2 = new System.Windows.Forms.LinkLabel();
			this.linkLabel1 = new System.Windows.Forms.LinkLabel();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textField6 = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.button1 = new System.Windows.Forms.Button();
			this.textField5 = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.label1 = new System.Windows.Forms.Label();
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.textField4 = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.tableLayoutPanel1.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// _workItemsTableView
			// 
			this._workItemsTableView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._workItemsTableView.ColumnHeaderTooltip = null;
			this._workItemsTableView.Location = new System.Drawing.Point(4, 244);
			this._workItemsTableView.Margin = new System.Windows.Forms.Padding(4);
			this._workItemsTableView.MultiSelect = false;
			this._workItemsTableView.Name = "_workItemsTableView";
			this._workItemsTableView.ReadOnly = false;
			this._workItemsTableView.Size = new System.Drawing.Size(1002, 435);
			this._workItemsTableView.SortButtonTooltip = null;
			this._workItemsTableView.TabIndex = 7;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._workItemsTableView, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.groupBox2, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 150F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 90F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(1010, 683);
			this.tableLayoutPanel1.TabIndex = 8;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._searchButton);
			this.groupBox1.Controls.Add(this._clearButton);
			this.groupBox1.Controls.Add(this.comboBoxField1);
			this.groupBox1.Controls.Add(this.textField3);
			this.groupBox1.Controls.Add(this.textField2);
			this.groupBox1.Controls.Add(this.textField1);
			this.groupBox1.Controls.Add(this._ruleName);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 153);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1004, 84);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Search";
			// 
			// _searchButton
			// 
			this._searchButton.Image = ((System.Drawing.Image)(resources.GetObject("_searchButton.Image")));
			this._searchButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this._searchButton.Location = new System.Drawing.Point(730, 41);
			this._searchButton.Name = "_searchButton";
			this._searchButton.Size = new System.Drawing.Size(93, 25);
			this._searchButton.TabIndex = 14;
			this._searchButton.Text = "Search";
			this._searchButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._searchButton.UseVisualStyleBackColor = true;
			this._searchButton.Click += new System.EventHandler(this._searchButton_Click);
			// 
			// _clearButton
			// 
			this._clearButton.Image = ((System.Drawing.Image)(resources.GetObject("_clearButton.Image")));
			this._clearButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this._clearButton.Location = new System.Drawing.Point(829, 40);
			this._clearButton.Name = "_clearButton";
			this._clearButton.Size = new System.Drawing.Size(93, 25);
			this._clearButton.TabIndex = 15;
			this._clearButton.Text = "Clear";
			this._clearButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
			this._clearButton.UseVisualStyleBackColor = true;
			// 
			// comboBoxField1
			// 
			this.comboBoxField1.DataSource = null;
			this.comboBoxField1.DisplayMember = "";
			this.comboBoxField1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxField1.LabelText = "Activity Type";
			this.comboBoxField1.Location = new System.Drawing.Point(589, 28);
			this.comboBoxField1.Margin = new System.Windows.Forms.Padding(2);
			this.comboBoxField1.Name = "comboBoxField1";
			this.comboBoxField1.Size = new System.Drawing.Size(107, 44);
			this.comboBoxField1.TabIndex = 13;
			this.comboBoxField1.Value = null;
			// 
			// textField3
			// 
			this.textField3.DataSource = null;
			this.textField3.DisplayMember = "";
			this.textField3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.textField3.LabelText = "Status";
			this.textField3.Location = new System.Drawing.Point(456, 28);
			this.textField3.Margin = new System.Windows.Forms.Padding(2);
			this.textField3.Name = "textField3";
			this.textField3.Size = new System.Drawing.Size(107, 44);
			this.textField3.TabIndex = 12;
			this.textField3.Value = null;
			// 
			// textField2
			// 
			this.textField2.LabelText = "Accession #";
			this.textField2.Location = new System.Drawing.Point(307, 28);
			this.textField2.Margin = new System.Windows.Forms.Padding(2);
			this.textField2.Mask = "";
			this.textField2.Name = "textField2";
			this.textField2.PasswordChar = '\0';
			this.textField2.Size = new System.Drawing.Size(126, 44);
			this.textField2.TabIndex = 11;
			this.textField2.ToolTip = null;
			this.textField2.Value = null;
			// 
			// textField1
			// 
			this.textField1.LabelText = "Patient Name";
			this.textField1.Location = new System.Drawing.Point(158, 28);
			this.textField1.Margin = new System.Windows.Forms.Padding(2);
			this.textField1.Mask = "";
			this.textField1.Name = "textField1";
			this.textField1.PasswordChar = '\0';
			this.textField1.Size = new System.Drawing.Size(126, 44);
			this.textField1.TabIndex = 10;
			this.textField1.ToolTip = null;
			this.textField1.Value = null;
			// 
			// _ruleName
			// 
			this._ruleName.LabelText = "Patient ID";
			this._ruleName.Location = new System.Drawing.Point(10, 28);
			this._ruleName.Margin = new System.Windows.Forms.Padding(2);
			this._ruleName.Mask = "";
			this._ruleName.Name = "_ruleName";
			this._ruleName.PasswordChar = '\0';
			this._ruleName.Size = new System.Drawing.Size(126, 44);
			this._ruleName.TabIndex = 9;
			this._ruleName.ToolTip = null;
			this._ruleName.Value = null;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.linkLabel2);
			this.groupBox2.Controls.Add(this.linkLabel1);
			this.groupBox2.Controls.Add(this.tableLayoutPanel2);
			this.groupBox2.Controls.Add(this.textField6);
			this.groupBox2.Controls.Add(this.button1);
			this.groupBox2.Controls.Add(this.textField5);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.progressBar1);
			this.groupBox2.Controls.Add(this.textField4);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox2.Location = new System.Drawing.Point(3, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(1004, 144);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Overview";
			// 
			// linkLabel2
			// 
			this.linkLabel2.AutoSize = true;
			this.linkLabel2.Location = new System.Drawing.Point(784, 79);
			this.linkLabel2.Name = "linkLabel2";
			this.linkLabel2.Size = new System.Drawing.Size(64, 13);
			this.linkLabel2.TabIndex = 24;
			this.linkLabel2.TabStop = true;
			this.linkLabel2.Text = "Study Rules";
			// 
			// linkLabel1
			// 
			this.linkLabel1.AutoSize = true;
			this.linkLabel1.Location = new System.Drawing.Point(784, 45);
			this.linkLabel1.Name = "linkLabel1";
			this.linkLabel1.Size = new System.Drawing.Size(132, 13);
			this.linkLabel1.TabIndex = 23;
			this.linkLabel1.TabStop = true;
			this.linkLabel1.Text = "Local Server Configuration";
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.label2, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.label7, 1, 2);
			this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.label5, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.label6, 1, 0);
			this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(521, 33);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(200, 90);
			this.tableLayoutPanel2.TabIndex = 22;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(3, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(52, 13);
			this.label2.TabIndex = 16;
			this.label2.Text = "AE Title";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(103, 62);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(55, 13);
			this.label7.TabIndex = 21;
			this.label7.Text = "ip address";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(3, 31);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(63, 13);
			this.label3.TabIndex = 17;
			this.label3.Text = "Hostname";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(103, 31);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(56, 13);
			this.label5.TabIndex = 19;
			this.label5.Text = "host name";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(103, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(38, 13);
			this.label6.TabIndex = 20;
			this.label6.Text = "ae title";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(3, 62);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(48, 13);
			this.label4.TabIndex = 18;
			this.label4.Text = "IP/Port";
			// 
			// textField6
			// 
			this.textField6.LabelText = "Failures";
			this.textField6.Location = new System.Drawing.Point(5, 79);
			this.textField6.Margin = new System.Windows.Forms.Padding(2);
			this.textField6.Mask = "";
			this.textField6.Name = "textField6";
			this.textField6.PasswordChar = '\0';
			this.textField6.ReadOnly = true;
			this.textField6.Size = new System.Drawing.Size(169, 44);
			this.textField6.TabIndex = 15;
			this.textField6.ToolTip = null;
			this.textField6.Value = null;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(376, 100);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 14;
			this.button1.Text = "Re-index";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// textField5
			// 
			this.textField5.LabelText = "File store path";
			this.textField5.Location = new System.Drawing.Point(203, 85);
			this.textField5.Margin = new System.Windows.Forms.Padding(2);
			this.textField5.Mask = "";
			this.textField5.Name = "textField5";
			this.textField5.PasswordChar = '\0';
			this.textField5.ReadOnly = true;
			this.textField5.Size = new System.Drawing.Size(169, 44);
			this.textField5.TabIndex = 13;
			this.textField5.ToolTip = null;
			this.textField5.Value = null;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(200, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 13);
			this.label1.TabIndex = 12;
			this.label1.Text = "Disk Usage";
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(203, 41);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(192, 23);
			this.progressBar1.TabIndex = 11;
			// 
			// textField4
			// 
			this.textField4.LabelText = "Total Studies";
			this.textField4.Location = new System.Drawing.Point(5, 31);
			this.textField4.Margin = new System.Windows.Forms.Padding(2);
			this.textField4.Mask = "";
			this.textField4.Name = "textField4";
			this.textField4.PasswordChar = '\0';
			this.textField4.ReadOnly = true;
			this.textField4.Size = new System.Drawing.Size(169, 44);
			this.textField4.TabIndex = 10;
			this.textField4.ToolTip = null;
			this.textField4.Value = null;
			// 
			// ActivityMonitorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "ActivityMonitorComponentControl";
			this.Size = new System.Drawing.Size(1010, 683);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _workItemsTableView;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private ClearCanvas.Desktop.View.WinForms.TextField textField2;
		private ClearCanvas.Desktop.View.WinForms.TextField textField1;
		private ClearCanvas.Desktop.View.WinForms.TextField _ruleName;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField textField3;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField comboBoxField1;
		private System.Windows.Forms.Button _searchButton;
		private System.Windows.Forms.Button _clearButton;
		private System.Windows.Forms.GroupBox groupBox2;
		private ClearCanvas.Desktop.View.WinForms.TextField textField4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private ClearCanvas.Desktop.View.WinForms.TextField textField6;
		private System.Windows.Forms.Button button1;
		private ClearCanvas.Desktop.View.WinForms.TextField textField5;
		private System.Windows.Forms.LinkLabel linkLabel2;
		private System.Windows.Forms.LinkLabel linkLabel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;

	}
}
