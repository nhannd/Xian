#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue
{
    /// <summary>
    /// A specialized panel that displays a list of <see cref="WorkQueue"/> entries.
    /// </summary>
    /// <remarks>
    /// <see cref="WorkQueueItemListPanel"/> wraps around <see cref="GridView"/> control to specifically display
    /// <see cref="WorkQueue"/> entries on a web page. The <see cref="WorkQueue"/> entries are set through <see cref="WorkQueueItems"/>. 
    /// User of this control can set the <see cref="Height"/> of the panel. The panel always expands to fit the width of the
    /// parent control. To enable paging, <see cref="AllowPaging"/> must be set to <b>true</b> and the page size is set through <see cref="PageSize"/>.
    /// 
    /// By default, <see cref="AutoRefresh"/> is on. The panel periodically refreshes the list automatically. Any item that no longer
    /// exists in the system will not be rendered on the screen.
    /// 
    /// </remarks>
    public partial class WorkQueueItemListPanel : System.Web.UI.UserControl
    {
        #region Private Members
        private WorkQueueItemCollection _workQueueItems;
        private int _pageIndex;
        private int _pageSize=20;
        private Unit _height;
        private bool _allowPaging;

        #endregion Private Members


        #region Public Properties

        /// <summary>
        /// Sets/Gets the page index
        /// </summary>
        public int PageIndex
        {
            set
            {
                _pageIndex = value;
                if (WorkQueueListView != null)
                    WorkQueueListView.PageIndex = value;
            }
            get
            {
                if (WorkQueueListView != null)
                    return WorkQueueListView.PageIndex;
                else
                    return _pageIndex;
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
        /// Gets/Sets the number of rows displayed on each page
        /// </summary>
        public int PageSize
        {
            set
            {
                _pageSize = value;
                if (WorkQueueListView != null)
                    WorkQueueListView.PageSize = value;
            }
            get
            {
                if (WorkQueueListView != null)
                    return WorkQueueListView.PageSize;
                else
                    return _pageSize;
            }
        }

        /// <summary>
        /// Gets/Sets a value indicating paging is enabled.
        /// </summary>
        public bool AllowPaging
        {
            get
            {
                if (WorkQueueListView != null)
                    return WorkQueueListView.AllowPaging;
                else
                    return _allowPaging;
            }
            set
            {
                _allowPaging = value;
                if (WorkQueueListView != null)
                    WorkQueueListView.AllowPaging = value; 
            }
        }

        /// <summary>
        /// Gets a reference to the work queue item list <see cref="GridView"/>
        /// </summary>
        public GridView WorkQueueItemListControl
        {
            get { return WorkQueueListView; }
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
        public Model.WorkQueue SelectedWorkQueueItem
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

                SelectedWorkQueueItemKey = value.GetKey();
                WorkQueueListView.SelectedIndex = WorkQueueItems.RowIndexOf(SelectedWorkQueueItemKey, WorkQueueListView);
            }
        }

        /// <summary>
        /// Sets/Gets a value which indicates whether auto refresh is on
        /// </summary>
        public bool AutoRefresh
        {
            get
            {
                if (ViewState[ClientID + "_AutoRefresh"] == null)
                    return true;
                else
                    return (bool)ViewState[ClientID + "_AutoRefresh"];
            }
            set { ViewState[ClientID + "_AutoRefresh"] = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            WorkQueueListView.DataKeyNames = new string[] { "WorkQueueGuid" };

            RefreshTimer.Interval = Math.Max(WorkQueueSettings.Default.NormalRefreshIntervalSeconds*1000, 5000);// max refresh rate: every 5 sec 

            if (_height!=Unit.Empty)
                ListContainerTable.Height = _height;

            WorkQueueListView.PageIndex = _pageIndex;
            WorkQueueListView.AllowPaging = _allowPaging;
            if (_allowPaging)
            {
                WorkQueueListView.PageSize = _pageSize;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                DataBind();
            }
        }

        
        protected ServerEntityKey SelectedWorkQueueItemKey
        {
            set
            {
                ViewState[ClientID + "_SelectedWorkQueueItemKey"] = value;
            }
            get
            {
                return ViewState[ClientID + "_SelectedWorkQueueItemKey"] as ServerEntityKey;
            }
        }

        protected int GetRefreshInterval()
        {
            int interval = WorkQueueSettings.Default.NormalRefreshIntervalSeconds * 1000;

            if (WorkQueueItems!=null)
            {
                // the refresh rate should be high if the item was scheduled to start soon..
                foreach(Model.WorkQueue item in WorkQueueItems.Values)
                {
                    TimeSpan span = item.ScheduledTime.Subtract(Platform.Time);
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
            int index = rowIndex;
            if (WorkQueueListView.AllowPaging)
            {
                index = WorkQueueListView.PageIndex * WorkQueueListView.PageSize + rowIndex;
            }

            if (index < 0 || index >= WorkQueueItems.Count)
                return null;

            return WorkQueueItems[index].GetKey();
        }

        protected void WorkQueueListView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow row = e.Row;

            if (WorkQueueListView.EditIndex != e.Row.RowIndex)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Add OnClick attribute to each row to make javascript call "Select$###" (where ### is the selected row)
                    // This method when posted back will be handled by the grid
                    row.Attributes["OnClick"] =
                        Page.ClientScript.GetPostBackEventReference(WorkQueueListView, "Select$" + e.Row.RowIndex);
                    row.Style["cursor"] = "hand";

                    Model.WorkQueue item = WorkQueueItems[GetRowItemKey(row.RowIndex)];
                    row.Attributes["uid"] = item.GetKey().ToString();

                    CustomizeColumns(e.Row);
                }
            }
            
        }

        private void CustomizeColumns(GridViewRow row)
        {
            WorkQueueSummary item = row.DataItem as WorkQueueSummary;

            if (item!=null)
            {
                if (item.ProcessingServer!=null)
                {
                    Label serverLabel = row.FindControl("ServerInfoLabel") as Label;
                    if (serverLabel != null)
                    {
                        serverLabel.Text = item.ProcessingServer.ServerName;
                        if (item.ProcessingServer.ExtInformation != null)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(ServerAddress));
                            try
                            {
                                ServerAddress address = (ServerAddress)serializer.Deserialize(new XmlNodeReader(item.ProcessingServer.ExtInformation.FirstChild));

                                for (int i = 0; i < address.IPAddresses.Count; i++)
                                {
                                    serverLabel.ToolTip += String.Format("IP {0} : {1}\r\n", i + 1, address.IPAddresses[i]);
                                }

                            }
                            catch (Exception)
                            {
                                // ignore it
                            }
                        }
                    }
                }
               

                Label typeLabel = row.FindControl("Type") as Label;
                if (typeLabel != null)
                {
                    typeLabel.Text = WorkQueueTypeEnumHelper.GetDescription(item.Type);
                }

                Label priorityLabel = row.FindControl("Priority") as Label;
                if (priorityLabel != null)
                {
                    priorityLabel.Text = WorkQueuePriorityEnumHelper.GetDescription(item.Priority);
                }

                Label statusLabel = row.FindControl("Status") as Label;
                if (statusLabel != null)
                {
                    statusLabel.Text = WorkQueueStatusEnumHelper.GetDescription(item.Status);
                }
            }
            
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
                WorkQueueListView.SelectedIndex = WorkQueueItems.RowIndexOf(SelectedWorkQueueItemKey, WorkQueueListView);
            }
        }

        protected void WorkQueueListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WorkQueueListView.SelectedDataKey!=null)
                SelectedWorkQueueItemKey = WorkQueueListView.SelectedDataKey.Value as ServerEntityKey;

            DataBind();
        }


        protected void RefreshTimer_Tick(object sender, EventArgs e)
        {
            DataBind();
        }

        #endregion Protected Methods


        #region Public Methods

        public override void DataBind()
        {
            if (WorkQueueItems != null)
            {
                List<WorkQueueSummary> workQueueItemSummaryList = new List<WorkQueueSummary>();
                foreach(Model.WorkQueue item in WorkQueueItems.Values)
                {
                    workQueueItemSummaryList.Add(WorkQueueSummaryAssembler.CreateWorkQueueSummary(item));
                }
                WorkQueueListView.DataSource = workQueueItemSummaryList;
            }


            WorkQueueListView.PageIndex = PageIndex;
            base.DataBind();
        }


        #endregion Public Methods

    }
}