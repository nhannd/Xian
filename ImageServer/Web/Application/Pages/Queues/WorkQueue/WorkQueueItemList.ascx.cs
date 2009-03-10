#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using GridView=System.Web.UI.WebControls.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue
{
    /// <summary>
    /// A specialized panel that displays a list of <see cref="WorkQueue"/> entries.
    /// </summary>
    /// <remarks>
    /// <see cref="WorkQueueItemList"/> wraps around <see cref="System.Web.UI.WebControls.GridView"/> control to specifically display
    /// <see cref="WorkQueue"/> entries on a web page. The <see cref="WorkQueue"/> entries are set through <see cref="WorkQueueItems"/>. 
    /// User of this control can set the <see cref="Height"/> of the panel. The panel always expands to fit the width of the
    /// parent control. 
    /// 
    /// By default, <see cref="AutoRefresh"/> is on. The panel periodically refreshes the list automatically. Any item that no longer
    /// exists in the system will not be rendered on the screen.
    /// 
    /// </remarks>
    public partial class WorkQueueItemList : System.Web.UI.UserControl
	{
		#region Delegates
		public delegate void WorkQueueDataSourceCreated(WorkQueueDataSource theSource);
		public event WorkQueueDataSourceCreated DataSourceCreated;
		#endregion

		#region Private Members
		private WorkQueueItemCollection _workQueueItems;
        private Unit _height;
    	private WorkQueueDataSource _dataSource;
        #endregion Private Members

        #region Public Properties
		public int ResultCount
		{
			get
			{
				if (_dataSource == null) return 0;
				return _dataSource.ResultCount;
			}
		}

        /// <summary>
        /// Gets/Sets the height of the panel
        /// </summary>
        public Unit Height
        {
            set
            {
                _height = value;
                if (ListContainerTable != null)
                    ListContainerTable.Height = value;
            }
            get
            {
                if (ListContainerTable != null)
                    return ListContainerTable.Height;
                else
                    return _height;
            }
        }

        /// <summary>
        /// Gets a reference to the work queue item list <see cref="System.Web.UI.WebControls.GridView"/>
        /// </summary>
        public GridView WorkQueueItemGridView
        {
            get { return WorkQueueGridView; }
        }

        /// <summary>
        /// Gets/Sets a value indicating paging is enabled.
        /// </summary>
        public WorkQueueItemCollection WorkQueueItems
        {
            get { return _workQueueItems; }
            set { _workQueueItems = value; }
        }

        /// <summary>
        /// Gets/Sets a key of the selected work queue item.
        /// </summary>
        public WorkQueueSummary SelectedWorkQueueItem
        {
            get
            {
                if (SelectedWorkQueueItemKey != null && WorkQueueItems.ContainsKey(SelectedWorkQueueItemKey))
                {
                    return WorkQueueItems[SelectedWorkQueueItemKey];
                }
                else
                    return null;
            }
            set
            {

                SelectedWorkQueueItemKey = value.Key;
                WorkQueueGridView.SelectedIndex = WorkQueueItems.RowIndexOf(SelectedWorkQueueItemKey, WorkQueueGridView);
            }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public IList<WorkQueueSummary> SelectedItems
        {
            get
            {
                if (WorkQueueItems == null || WorkQueueItems.Count == 0)
                    return null;

                int[] rows = WorkQueueGridView.SelectedIndices;
                if (rows == null || rows.Length == 0)
                    return null;

                IList<WorkQueueSummary> queueItems = new List<WorkQueueSummary>();
                for (int i = 0; i < rows.Length; i++)
                {
                    if (rows[i] < WorkQueueItems.Count)
                    {
                        queueItems.Add(WorkQueueItems[rows[i]]);
                    }
                }

                return queueItems;
            }
        }

        /// <summary>
        /// Sets/Gets a value which indicates whether auto refresh is on
        /// </summary>
        public bool AutoRefresh
        {
            get
            {
                if (ViewState[ "AutoRefresh"] == null)
                    return true;
                else
                    return (bool)ViewState[ "AutoRefresh"];
            }
            set { ViewState[ "AutoRefresh"] = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            WorkQueueGridView.DataKeyNames = new string[] { "Key" };

            RefreshTimer.Interval = Math.Max(WorkQueueSettings.Default.NormalRefreshIntervalSeconds*1000, 5000);// min refresh rate: every 5 sec 

            if (_height!=Unit.Empty)
                ListContainerTable.Height = _height;

            WorkQueueGridView.DataSource = WorkQueueDataSourceObject;

            if(!IsPostBack)
            {
                WorkQueueGridView.DataBind();
            }
        }
      
        protected ServerEntityKey SelectedWorkQueueItemKey
        {
            set
            {
                ViewState[ "_SelectedWorkQueueItemKey"] = value;
            }
            get
            {
                return ViewState[ "_SelectedWorkQueueItemKey"] as ServerEntityKey;
            }
        }

        protected int GetRefreshInterval()
        {
            int interval = WorkQueueSettings.Default.NormalRefreshIntervalSeconds * 1000;

            if (WorkQueueItems!=null)
            {
                // the refresh rate should be high if the item was scheduled to start soon..
                foreach(WorkQueueSummary item in WorkQueueItems.Values)
                {
                    TimeSpan span = item.TheWorkQueueItem.ScheduledTime.Subtract(Platform.Time);
                    if (span < TimeSpan.FromMinutes(1))
                    {
                        interval = WorkQueueSettings.Default.FastRefreshIntervalSeconds * 1000; 
                        break;
                    }
                }
            }

            return interval;
        }

        protected override void OnPreRender(EventArgs e)
        {
			RefreshTimer.Enabled = AutoRefresh && Visible;
            if (RefreshTimer.Enabled)
            {
                if (WorkQueueItems!=null)
                {
                    RefreshTimer.Interval = GetRefreshInterval();
                }
            }

            base.OnPreRender(e);
        }

        protected ServerEntityKey GetRowItemKey(int rowIndex)
        {
			if (WorkQueueItems == null) return null;
	
			return WorkQueueItems[rowIndex].Key;
        }

        protected void WorkQueueListView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;

            if (WorkQueueGridView.EditIndex != e.Row.RowIndex)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    WorkQueueSummary item = WorkQueueItems[GetRowItemKey(row.RowIndex)];
					row.Attributes["uid"] = item.Key.ToString();

                    CustomizeColumns(e.Row);
                    CustomizeRowAttribute(e.Row);
                }
            }
            
        }

        private void CustomizeColumns(GridViewRow row)
        {
			WorkQueueSummary summary = row.DataItem as WorkQueueSummary;

			if (summary != null)
            {
            	PersonNameLabel nameLabel = row.FindControl("PatientName") as PersonNameLabel;
				if (nameLabel != null)
            		nameLabel.PersonName = summary.PatientsName;
            }
        }

        private void CustomizeRowAttribute(GridViewRow row)
        {
            WorkQueueSummary item = row.DataItem as WorkQueueSummary;
            row.Attributes["canreschedule"] = WorkQueueController.CanReschedule(item.TheWorkQueueItem).ToString().ToLower();
            row.Attributes["canreset"] = WorkQueueController.CanReset(item.TheWorkQueueItem).ToString().ToLower();
            row.Attributes["candelete"] = WorkQueueController.CanDelete(item.TheWorkQueueItem).ToString().ToLower();
            row.Attributes["canreprocess"] = WorkQueueController.CanReprocess(item.TheWorkQueueItem).ToString().ToLower();
        }

        protected void WorkQueueListView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void WorkQueueListView_DataBound(object sender, EventArgs e)
        {
            // reselect the row based on the new order
            if (SelectedWorkQueueItemKey != null)
            {
                WorkQueueGridView.SelectedIndex = WorkQueueItems.RowIndexOf(SelectedWorkQueueItemKey, WorkQueueGridView);
            }
        }

        protected void WorkQueueListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WorkQueueGridView.SelectedDataKey!=null)
                SelectedWorkQueueItemKey = WorkQueueGridView.SelectedDataKey.Value as ServerEntityKey;

            DataBind();
        }

        protected void RefreshTimer_Tick(object sender, EventArgs e)
        {
            DataBind();
        }

    	protected void GetWorkQueueDataSource(object sender, ObjectDataSourceEventArgs e)
    	{
			if (_dataSource == null)
			{
				_dataSource = new WorkQueueDataSource();
				_dataSource.WorkQueueFoundSet += delegate(IList<WorkQueueSummary> newlist)
				                                 	{
				                                 		WorkQueueItems = new WorkQueueItemCollection(newlist);
				                                 	};
			}

    		e.ObjectInstance = _dataSource;

			if (DataSourceCreated != null)
				DataSourceCreated(_dataSource);
    	}

    	protected void DisposeWorkQueueDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
    	{
			// Don't dispose the object.
    		e.Cancel = true;
		}
		#endregion Protected Methods

        public void Refresh()
        {
            WorkQueueGridView.PageIndex = 0;
            DataBind();
        }

        public void RefreshCurrentPage()
        {
            DataBind();
        }
    }
}