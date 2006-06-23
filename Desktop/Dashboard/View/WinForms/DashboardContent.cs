using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Dashboard
{
	public abstract class DashboardContent : IDashboardContent
	{
		private Control _masterViewHost;
		private Control _detailViewHost;

		protected DashboardContent()
		{
		}

		public abstract string Name
		{
			get;
		}

		public Control MasterViewHost
		{
			get { return _masterViewHost; }
			set { _masterViewHost = value; }
		}

        public Control DetailViewHost
		{
			get { return _detailViewHost; }
			set { _detailViewHost = value; }
		}

		public UserControl MasterView
		{
			get { return this.MasterViewHost.Controls[0] as UserControl; }
			set 
			{
				this.MasterViewHost.SuspendLayout();
				this.MasterViewHost.Controls.Clear();
				value.Dock = DockStyle.Fill;
				this.MasterViewHost.Controls.Add(value);
				this.MasterViewHost.ResumeLayout();
			}
		}

		public UserControl DetailView
		{
			get { return this.DetailViewHost.Controls[0] as UserControl; }
			set 
			{
				this.DetailViewHost.SuspendLayout();
				this.DetailViewHost.Controls.Clear();
				value.Dock = DockStyle.Fill;
				//value.Anchor = AnchorStyles.Left | AnchorStyles.Top;
				this.DetailViewHost.Controls.Add(value);
				this.DetailViewHost.ResumeLayout();
			}
		}

		public abstract void OnSelected();
	}
}
