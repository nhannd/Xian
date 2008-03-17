using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit
{
    public partial class ViewEdit : BasePage
    {
        private const string SELECTED_WORKQUEUES_UIDS_KEY = "uid";

        private ServerEntityKey _workQueueItemKey;

        private List<Model.WorkQueue> _workqueues = new List<Model.WorkQueue>();

        public ServerEntityKey WorkQueueItemKey
        {
            get { return _workQueueItemKey; }
            set { _workQueueItemKey = value; }
        }

        public List<ServerEntityKey> GetKeys(List<Model.WorkQueue> list)
        {
            List<ServerEntityKey> keys = new List<ServerEntityKey>();
            foreach (Model.WorkQueue entity in list)
            {
                keys.Add(entity.GetKey());
            }

            return keys;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            WorkQueueItemDetailsPanel1.RescheduleButtonClick += new WorkQueueItemDetailsPanel.OnRescheduleButtonClickHandler(WorkQueueItemDetailsPanel1_RescheduleButtonClick);

            ScheduleWorkQueueDialog1.OnWorkQueueUpdated += new ScheduleWorkQueueDialog.OnWorkQueueUpdatedHandler(ScheduleWorkQueueDialog1_OnWorkQueueUpdated);
            
            LoadWorkQueueItemKey();
        }

        void ScheduleWorkQueueDialog1_OnWorkQueueUpdated(List<ClearCanvas.ImageServer.Model.WorkQueue> workqueueItems)
        {
            DataBind();
            WorkQueueItemDetailsPanel1.Refresh();
        }

        void WorkQueueItemDetailsPanel1_RescheduleButtonClick()
        {
            RescheduleWorkQueueItem();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DataBind();
        }

        public override void DataBind()
        {
            if (WorkQueueItemKey!=null)
            {
                WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
                WorkQueueItemDetailsPanel1.WorkQueue = adaptor.Get(WorkQueueItemKey);

                if (WorkQueueItemDetailsPanel1.WorkQueue==null)
                {
                    if (ScriptManager.GetCurrent(Page).IsInAsyncPostBack)
                    {
                        InformationDialog.Message = "This work queue item is no longer available";
                        InformationDialog.Show();
                    }
                    
                }
            }
            
            base.DataBind();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
            }
            else
            {
                
            }
        }

        private void LoadWorkQueueItemKey()
        {
            string requestedGuid = Page.Request.QueryString[SELECTED_WORKQUEUES_UIDS_KEY];
            if (!String.IsNullOrEmpty(requestedGuid))
            {
                WorkQueueItemKey = new ServerEntityKey("WorkQueue", requestedGuid);
            }
            
        }

        public void RescheduleWorkQueueItem()
        {
            List<ServerEntityKey> keys = new List<ServerEntityKey>();
            keys.Add(WorkQueueItemKey);
            ScheduleWorkQueueDialog1.WorkQueueKeys = keys;

            if (WorkQueueItemDetailsPanel1.WorkQueue.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("Failed"))
            {
                InformationDialog.Message = "This work queue item has failed.";
                InformationDialog.Show();
                return;
            }
            else if (WorkQueueItemDetailsPanel1.WorkQueue.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("In Progress"))
            {
                InformationDialog.Message = "This work queue item is being processed. Please try again later.";
                InformationDialog.Show();
                return;
            }

            ScheduleWorkQueueDialog1.Show();
        }
    }
}
