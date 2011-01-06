#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	/// <summary>
	/// Summary description for Example.
	/// </summary>
	public class StackTab : UserControl
	{        
        // Private field
        private Control _applicationComponentControl;
        private EventHandler _buttonClicked;
		private EventHandler _titleClicked;
		private EventHandler _titleDoubleClicked;

		// Designer generated
        private TitleBar _titleBar;
        private Panel _panel;
        private TableLayoutPanel tableLayoutPanel1;

		private readonly StackTabPage _page;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StackTab()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		// New constructor
		public StackTab(StackTabPage page, DockStyle docStyle)
		{
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

        	_page = page;
			_titleBar.Dock = docStyle;

            // Set initial values
			_titleBar.PreText = String.Empty;
			_titleBar.Text = _page.Title;
			_titleBar.PostText = String.Empty;
			if (_page.IconSet != null)
				_titleBar.Image = _page.IconSet.CreateIcon(IconSize.Small, _page.ResourceResolver);

			_page.TitleChanged += OnPageTitleChanged;
			_page.IconSetChanged += OnPageIconChanged;
		}

		#region Event Handlers

		private void OnPageTitleChanged(object sender, EventArgs e)
		{
			_titleBar.PreText = String.Empty;
			_titleBar.Text = _page.Title;
			_titleBar.PostText = String.Empty;
		}

		private void OnPageIconChanged(object sender, EventArgs e)
		{
			if (_page.IconSet != null)
				_titleBar.Image = _page.IconSet.CreateIcon(IconSize.Small, _page.ResourceResolver);
			else
				_titleBar.Image = null;
		}

		private void OnButtonClick(object sender, EventArgs e)
		{
			if (_buttonClicked != null)
				_buttonClicked(sender, e);
		}

		private void OnTitleClicked(object sender, EventArgs e)
		{
			if (_titleClicked != null)
				_titleClicked(sender, e);
		}

		private void OnTitleDoubleClick(object sender, EventArgs e)
		{
			if (_titleDoubleClicked != null)
				_titleDoubleClicked(sender, e);
		}

		#endregion

		#region UserControl overrides

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this._panel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _titleBar
			// 
			this._titleBar.ArrowButton = Crownwood.DotNetMagic.Controls.ArrowButton.DownArrow;
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this._titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
			this._titleBar.ImageAlignment = Crownwood.DotNetMagic.Controls.ImageAlignment.Far;
			this._titleBar.Location = new System.Drawing.Point(0, 0);
			this._titleBar.Margin = new System.Windows.Forms.Padding(0);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(368, 24);
			this._titleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.Office2007Black;
			this._titleBar.TabIndex = 0;
			this._titleBar.Text = "titleBar1";
			this._titleBar.Click += new System.EventHandler(this.OnTitleClicked);
			this._titleBar.ButtonClick += new System.EventHandler(this.OnButtonClick);
			this._titleBar.DoubleClick += new System.EventHandler(this.OnTitleDoubleClick);
			// 
			// _panel
			// 
			this._panel.AutoScroll = true;
			this._panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._panel.Location = new System.Drawing.Point(0, 24);
			this._panel.Margin = new System.Windows.Forms.Padding(0);
			this._panel.Name = "_panel";
			this._panel.Size = new System.Drawing.Size(368, 192);
			this._panel.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._titleBar, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this._panel, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(368, 216);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// StackTab
			// 
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "StackTab";
			this.Size = new System.Drawing.Size(368, 216);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		// Allow direct access to the titlebar
		public TitleBar TitleBar
		{
			get { return _titleBar; }
		}

		public event EventHandler ButtonClicked
		{
			add { _buttonClicked += value; }
			remove { _buttonClicked -= value; }
		}

		public event EventHandler TitleClicked
		{
			add { _titleClicked += value; }
			remove { _titleClicked -= value; }
		}

		public event EventHandler TitleDoubleClicked
		{
			add { _titleDoubleClicked += value; }
			remove { _titleDoubleClicked -= value; }
		}

		public Control ApplicationComponentControl
        {
            get { return _applicationComponentControl; }
            set
            {
                _applicationComponentControl = value;
                this._panel.Controls.Clear();
                this._panel.Controls.Add(_applicationComponentControl);
            }
        }

		// Caller can discover minimum requested size
		public Size MinimumRequestedSize
		{
			get { return new Size(23, 23); }
		}
	}
}
