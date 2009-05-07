#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Application.Controls;
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
        private Unit _height;
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

        /// <summary>
        /// Gets/Sets the height of the study list panel
        /// </summary>
        public Unit Height
        {
            get
            {
                if (ContainerTable != null)
                    return ContainerTable.Height;
                else
                    return _height;
            }
            set
            {
                _height = value;
                if (ContainerTable != null)
                    ContainerTable.Height = value;
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

            if (IsPostBack || Page.IsAsync)
            {
                ApplicationLogListControl.DataSource = ApplicationLogDataSourceObject;
            } 

            if (_height!=Unit.Empty)
                ContainerTable.Height = _height;

		    
		}

        protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.EmptyDataRow)
            {
                EmptySearchResultsMessage message =
                                        (EmptySearchResultsMessage)e.Row.FindControl("EmptySearchResultsMessage");
                if (message != null)
                {
                    if (ApplicationLogListControl.DataSource == null)
                    {
                        message.Message = "Please enter search criteria to find log entries.";
                    }
                    else
                    {
                        message.Message = "No log entries found matching the provided criteria.";
                    }
                }

            }
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
			ApplicationLogListControl.DataBind();
		}

	}
}