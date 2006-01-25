namespace ClearCanvas.Controls
{
	partial class OutlookSidebar
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutlookSidebar));
			this._stackStripSplitter = new System.Windows.Forms.SplitContainer();
			this._userPanel = new System.Windows.Forms.Panel();
			this._subHeaderStrip = new ClearCanvas.Controls.HeaderStrip();
			this._subHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
			this._mainHeaderStrip = new ClearCanvas.Controls.HeaderStrip();
			this._mainHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
			this._stackStrip = new ClearCanvas.Controls.StackStrip();
			this._overflowStrip = new ClearCanvas.Controls.BaseStackStrip();
			this._toolStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
			this._addorRemoveButtonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._stackStripSplitter.Panel1.SuspendLayout();
			this._stackStripSplitter.Panel2.SuspendLayout();
			this._stackStripSplitter.SuspendLayout();
			this._subHeaderStrip.SuspendLayout();
			this._mainHeaderStrip.SuspendLayout();
			this._overflowStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _stackStripSplitter
			// 
			this._stackStripSplitter.BackColor = System.Drawing.Color.Transparent;
			this._stackStripSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this._stackStripSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this._stackStripSplitter.Location = new System.Drawing.Point(0, 0);
			this._stackStripSplitter.Name = "_stackStripSplitter";
			this._stackStripSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _stackStripSplitter.Panel1
			// 
			this._stackStripSplitter.Panel1.Controls.Add(this._userPanel);
			this._stackStripSplitter.Panel1.Controls.Add(this._subHeaderStrip);
			this._stackStripSplitter.Panel1.Controls.Add(this._mainHeaderStrip);
			// 
			// _stackStripSplitter.Panel2
			// 
			this._stackStripSplitter.Panel2.Controls.Add(this._stackStrip);
			this._stackStripSplitter.Panel2.Controls.Add(this._overflowStrip);
			this._stackStripSplitter.Size = new System.Drawing.Size(312, 629);
			this._stackStripSplitter.SplitterDistance = 315;
			this._stackStripSplitter.SplitterWidth = 7;
			this._stackStripSplitter.TabIndex = 0;
			this._stackStripSplitter.TabStop = false;
			this._stackStripSplitter.Text = "splitContainer1";
			this._stackStripSplitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.stackStripSplitter_SplitterMoved);
			this._stackStripSplitter.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer1_Paint);
			// 
			// _userPanel
			// 
			this._userPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._userPanel.Location = new System.Drawing.Point(0, 44);
			this._userPanel.Name = "_userPanel";
			this._userPanel.Size = new System.Drawing.Size(312, 271);
			this._userPanel.TabIndex = 2;
			// 
			// _subHeaderStrip
			// 
			this._subHeaderStrip.AutoSize = false;
			this._subHeaderStrip.Font = new System.Drawing.Font("Tahoma", 9.75F);
			this._subHeaderStrip.ForeColor = System.Drawing.Color.Black;
			this._subHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._subHeaderStrip.HeaderStyle = ClearCanvas.Controls.AreaHeaderStyle.Small;
			this._subHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._subHeaderStripLabel});
			this._subHeaderStrip.Location = new System.Drawing.Point(0, 25);
			this._subHeaderStrip.Name = "_subHeaderStrip";
			this._subHeaderStrip.Size = new System.Drawing.Size(312, 19);
			this._subHeaderStrip.TabIndex = 0;
			this._subHeaderStrip.Text = "headerStrip1";
			// 
			// _subHeaderStripLabel
			// 
			this._subHeaderStripLabel.Name = "_subHeaderStripLabel";
			this._subHeaderStripLabel.Size = new System.Drawing.Size(0, 16);
			// 
			// _mainHeaderStrip
			// 
			this._mainHeaderStrip.AutoSize = false;
			this._mainHeaderStrip.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
			this._mainHeaderStrip.ForeColor = System.Drawing.Color.White;
			this._mainHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._mainHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mainHeaderStripLabel});
			this._mainHeaderStrip.Location = new System.Drawing.Point(0, 0);
			this._mainHeaderStrip.Name = "_mainHeaderStrip";
			this._mainHeaderStrip.Size = new System.Drawing.Size(312, 25);
			this._mainHeaderStrip.TabIndex = 1;
			this._mainHeaderStrip.Text = "headerStrip2";
			// 
			// _mainHeaderStripLabel
			// 
			this._mainHeaderStripLabel.Name = "_mainHeaderStripLabel";
			this._mainHeaderStripLabel.Size = new System.Drawing.Size(0, 22);
			// 
			// _stackStrip
			// 
			this._stackStrip.AutoSize = false;
			this._stackStrip.CanOverflow = false;
			this._stackStrip.Dock = System.Windows.Forms.DockStyle.Fill;
			this._stackStrip.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.World);
			this._stackStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._stackStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._stackStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
			this._stackStrip.Location = new System.Drawing.Point(0, 0);
			this._stackStrip.Name = "_stackStrip";
			this._stackStrip.Padding = new System.Windows.Forms.Padding(0);
			this._stackStrip.Size = new System.Drawing.Size(312, 275);
			this._stackStrip.TabIndex = 0;
			this._stackStrip.Tag = "Read";
			this._stackStrip.Text = "stackStrip1";
			// 
			// _overflowStrip
			// 
			this._overflowStrip.AutoSize = false;
			this._overflowStrip.CanOverflow = false;
			this._overflowStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._overflowStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._overflowStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripDropDownButton});
			this._overflowStrip.Location = new System.Drawing.Point(0, 275);
			this._overflowStrip.Name = "_overflowStrip";
			this._overflowStrip.Size = new System.Drawing.Size(312, 32);
			this._overflowStrip.TabIndex = 1;
			this._overflowStrip.Text = "overflowStrip";
			// 
			// _toolStripDropDownButton
			// 
			this._toolStripDropDownButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this._toolStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
			this._toolStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._addorRemoveButtonsToolStripMenuItem});
			this._toolStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("_toolStripDropDownButton.Image")));
			this._toolStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this._toolStripDropDownButton.Margin = new System.Windows.Forms.Padding(0);
			this._toolStripDropDownButton.Name = "_toolStripDropDownButton";
			this._toolStripDropDownButton.Padding = new System.Windows.Forms.Padding(3);
			this._toolStripDropDownButton.Size = new System.Drawing.Size(19, 32);
			this._toolStripDropDownButton.Text = "toolStripDropDownButton1";
			// 
			// _addorRemoveButtonsToolStripMenuItem
			// 
			this._addorRemoveButtonsToolStripMenuItem.Name = "_addorRemoveButtonsToolStripMenuItem";
			this._addorRemoveButtonsToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
			this._addorRemoveButtonsToolStripMenuItem.Text = "&Add or Remove Buttons";
			// 
			// OutlookSidebar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Transparent;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._stackStripSplitter);
			this.Name = "OutlookSidebar";
			this.Size = new System.Drawing.Size(312, 629);
			this.Load += new System.EventHandler(this.StackBar_Load);
			this._stackStripSplitter.Panel1.ResumeLayout(false);
			this._stackStripSplitter.Panel2.ResumeLayout(false);
			this._stackStripSplitter.ResumeLayout(false);
			this._subHeaderStrip.ResumeLayout(false);
			this._subHeaderStrip.PerformLayout();
			this._mainHeaderStrip.ResumeLayout(false);
			this._mainHeaderStrip.PerformLayout();
			this._overflowStrip.ResumeLayout(false);
			this._overflowStrip.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer _stackStripSplitter;
		private HeaderStrip _subHeaderStrip;
		private System.Windows.Forms.ToolStripLabel _subHeaderStripLabel;
		private HeaderStrip _mainHeaderStrip;
		private System.Windows.Forms.ToolStripLabel _mainHeaderStripLabel;
		private StackStrip _stackStrip;
		private BaseStackStrip _overflowStrip;
		private System.Windows.Forms.ToolStripDropDownButton _toolStripDropDownButton;
		private System.Windows.Forms.ToolStripMenuItem _addorRemoveButtonsToolStripMenuItem;
		private System.Windows.Forms.Panel _userPanel;
	}
}
