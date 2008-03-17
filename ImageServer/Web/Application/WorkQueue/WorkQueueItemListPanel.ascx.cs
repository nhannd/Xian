using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue
{
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

                SelectedWorkQueueItemKey = value.WorkQueueGuid;
                WorkQueueListView.SelectedIndex = WorkQueueItems.RowIndexOf(SelectedWorkQueueItemKey, WorkQueueListView);
            }
        }


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

            RefreshTimer.Interval = Math.Max(WorkQueueSettings.Default.RefreshIntervalSeconds*1000, 5000);// max refresh rate: every 5 sec 

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

        public override void DataBind()
        {
            if (WorkQueueItems!=null)
            {
                WorkQueueListView.DataSource = WorkQueueItems.Values;
            }


            WorkQueueListView.PageIndex = PageIndex;
            base.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            RefreshTimer.Enabled = AutoRefresh && Visible;

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

            return WorkQueueItems[index].WorkQueueGuid;
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


                    row.Attributes["uid"] = GetRowItemKey(row.RowIndex).Key.ToString();
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

    }
}