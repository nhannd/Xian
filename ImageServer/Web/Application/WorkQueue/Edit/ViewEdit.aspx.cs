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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit
{
    public partial class ViewEdit : BasePage
    {
        #region Constants
        private const string SELECTED_WORKQUEUES_UIDS_KEY = "uid";

        #endregion Constants


        #region Private Members

        private ServerEntityKey _workQueueItemKey;

        #endregion Private Members

        #region Protected Properties

        protected ServerEntityKey WorkQueueItemKey
        {
            get { return _workQueueItemKey; }
            set { _workQueueItemKey = value; }
        }


        protected bool ItemNotAvailableAlertShown
        {
            get
            {
                if (ViewState[ClientID + "ItemNotAvailableAlertShown"] == null)
                    return false;
                else
                    return (bool)ViewState[ClientID + "ItemNotAvailableAlertShown"];
            }
            set { ViewState[ClientID + "ItemNotAvailableAlertShown"] = value; }
        }

        #endregion Protected Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            WorkQueueItemDetailsPanel1.RescheduleButtonClick +=  WorkQueueItemDetailsPanel1_RescheduleButtonClick;
            ScheduleWorkQueueDialog1.OnWorkQueueUpdated += ScheduleWorkQueueDialog1_OnWorkQueueUpdated;
            InformationDialog.Confirmed += InformationDialog_Confirmed;
            
            LoadWorkQueueItemKey();
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (WorkQueueItemKey == null || WorkQueueItemDetailsPanel1.WorkQueue == null)
            {
                // make sure all dialogs are closed 
                ScheduleWorkQueueDialog1.Hide();
                WorkQueueItemDetailsPanel1.Visible = false;

                Message.Text = "The requested item is no longer available on the system";
                Message.Visible = true;
            }
            else
            {
                Message.Visible = false;
            }

            base.OnPreRender(e);
        }

        #endregion Protected Methods


        #region Private Methods

        void InformationDialog_Confirmed(object data)
        {
            ItemNotAvailableAlertShown = true;
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

        


        private void LoadWorkQueueItemKey()
        {
            string requestedGuid = Page.Request.QueryString[SELECTED_WORKQUEUES_UIDS_KEY];
            if (!String.IsNullOrEmpty(requestedGuid))
            {
                WorkQueueItemKey = new ServerEntityKey("WorkQueue", requestedGuid);
            }

        }

        #endregion Private Methods


        #region Public Methods

        /// <summary>
        ///  Pops up a dialog box to let user to reschedule a work queue item
        /// </summary>
        public void RescheduleWorkQueueItem()
        {
            List<ServerEntityKey> keys = new List<ServerEntityKey>();
            keys.Add(WorkQueueItemKey);
            ScheduleWorkQueueDialog1.WorkQueueKeys = keys;

            if (WorkQueueItemDetailsPanel1.WorkQueue!=null)
            {
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

        public override void DataBind()
        {
            if (WorkQueueItemKey != null)
            {
                WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
                WorkQueueItemDetailsPanel1.WorkQueue = adaptor.Get(WorkQueueItemKey);

                if (WorkQueueItemDetailsPanel1.WorkQueue == null)
                {

                    if (!ItemNotAvailableAlertShown)
                    {
                        InformationDialog.Message = "This work queue item is no longer available on the system.";
                        InformationDialog.Show();
                    }

                }
            }

            base.DataBind();
        }

        #endregion Public Methods
    }
}
