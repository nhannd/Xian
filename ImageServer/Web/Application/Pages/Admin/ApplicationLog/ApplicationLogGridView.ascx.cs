using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using GridView = ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.ApplicationLog
{
	public partial class ApplicationLogGridView : System.Web.UI.UserControl
	{
		#region Delegates
		public delegate void ApplicationLogDataSourceCreated(ApplicationLogDataSource theSource);
		public event ApplicationLogDataSourceCreated DataSourceCreated;
		#endregion

		#region Private Members
		private ApplicationLogDataSource _dataSource = null;
		private IList<Model.ApplicationLog> _logs;
		#endregion

		#region Properties
		/// <summary>
		/// Gets/Sets the list of devices rendered on the screen.
		/// </summary>
		public IList<Model.ApplicationLog> ApplicationLogs
		{
			get
			{
				return _logs;
			}
			set
			{
				_logs = value;
			}
		}
		public Web.Common.WebControls.UI.GridView ApplicationLogListGrid
		{
			get { return ApplicationLogListControl; }
		}

		/// <summary>
		/// Gets/Sets the current selected device.
		/// </summary>
		public IList<Model.ApplicationLog> SelectedStudies
		{
			get
			{
				if (ApplicationLogs == null || ApplicationLogs.Count == 0)
					return null;

				int[] rows = ApplicationLogListControl.SelectedIndices;
				if (rows == null || rows.Length == 0)
					return null;

				IList<Model.ApplicationLog> studies = new List<Model.ApplicationLog>();
				for (int i = 0; i < rows.Length; i++)
				{
					if (rows[i] < ApplicationLogs.Count)
						studies.Add(ApplicationLogs[rows[i]]);
				}

				return studies;
			}
		}
		public int ResultCount
		{
			get
			{
				if (_dataSource == null)
				{
					_dataSource = new ApplicationLogDataSource();

					_dataSource.ApplicationLogFoundSet += delegate(IList<Model.ApplicationLog> newlist)
											{
												ApplicationLogs = newlist;
											};
					if (DataSourceCreated != null)
						DataSourceCreated(_dataSource);
					_dataSource.SelectCount();
				}
				if (_dataSource.ResultCount == 0)
				{
					if (DataSourceCreated != null)
						DataSourceCreated(_dataSource);

					_dataSource.SelectCount();
				}
				return _dataSource.ResultCount;
			}
		}
		#endregion

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

		    ApplicationLogListControl.DataSource = ApplicationLogDataSourceObject;
		}

		protected void GetApplicationLogDataSource(object sender, ObjectDataSourceEventArgs e)
		{
			if (_dataSource == null)
			{
				_dataSource = new ApplicationLogDataSource();

				_dataSource.ApplicationLogFoundSet += delegate(IList<Model.ApplicationLog> newlist)
										{
											ApplicationLogs = newlist;
										};
			}

			e.ObjectInstance = _dataSource;

			if (DataSourceCreated != null)
				DataSourceCreated(_dataSource);
		}

		protected void DisposeApplicationLogDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
		{
			e.Cancel = true;
		}

		protected void ApplicationLogListControl_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			ApplicationLogListControl.PageIndex = e.NewPageIndex;
			DataBind();
		}

	}
}