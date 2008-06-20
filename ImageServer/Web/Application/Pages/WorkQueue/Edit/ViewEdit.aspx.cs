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
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.WorkQueue.Edit
{
    public partial class ViewEdit : BasePage
    {
        #region Constants
        private const string SELECTED_WORKQUEUES_UIDS_KEY = "uid";

        #endregion Constants


        #region Private Members

        private ServerEntityKey _workQueueItemKey;
        private Model.WorkQueue _workQueue;

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

            WorkQueueItemDetailsPanel.RescheduleButtonClick +=  WorkQueueItemDetailsPanel_RescheduleButtonClick;
            WorkQueueItemDetailsPanel.ResetButtonClick += WorkQueueItemDetailsPanel_ResetButtonClick;
            WorkQueueItemDetailsPanel.DeleteButtonClick += WorkQueueItemDetailsPanel_DeleteButtonClick;

            DeleteWorkQueueDialog.WorkQueueItemDeleted += DeleteWorkQueueDialog_WorkQueueItemDeleted;
            ScheduleWorkQueueDialog.WorkQueueUpdated += ScheduleWorkQueueDialog_OnWorkQueueUpdated;
            InformationDialog.Confirmed += InformationDialog_Confirmed;

            ResetWorkQueueDialog.WorkQueueItemReseted += new ResetWorkQueueDialog.WorkQueueItemResetListener(ResetWorkQueueDialog_WorkQueueItemReseted);
            
            LoadWorkQueueItemKey();
        }


        

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DataBind();
        }


        protected override void OnPreRender(EventArgs e)
        {
            if (WorkQueueItemKey == null || _workQueue == null)
            {
                // make sure all dialogs are closed 
                ScheduleWorkQueueDialog.Hide();
                WorkQueueItemDetailsPanel.Visible = false;
                ResetWorkQueueDialog.Hide();

                Message.Text = App_GlobalResources.SR.WorkQueueNotAvailable;
                Message.Visible = true;

                UpdatePanel.Update();
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

        void ScheduleWorkQueueDialog_OnWorkQueueUpdated(List<ClearCanvas.ImageServer.Model.WorkQueue> workqueueItems)
        {
            DataBind();
            WorkQueueItemDetailsPanel.Refresh();
        }


        void DeleteWorkQueueDialog_WorkQueueItemDeleted(Model.WorkQueue item)
        {
            DataBind();
            WorkQueueItemDetailsPanel.Refresh();
        }

        void ResetWorkQueueDialog_WorkQueueItemReseted(Model.WorkQueue item)
        {
            DataBind();
            WorkQueueItemDetailsPanel.Refresh();
        }


        void WorkQueueItemDetailsPanel_RescheduleButtonClick()
        {
            RescheduleWorkQueueItem();
        }


        void WorkQueueItemDetailsPanel_ResetButtonClick()
        {
            ResetWorkQueueItem();
        }

        void WorkQueueItemDetailsPanel_DeleteButtonClick()
        {
            DeleteWorkQueueItem();
        }

        /// <summary>
        ///  Pops up a dialog box to let user to reschedule a work queue item
        /// </summary>
        private void RescheduleWorkQueueItem()
        {
            List<ServerEntityKey> keys = new List<ServerEntityKey>();
            keys.Add(WorkQueueItemKey);
            ScheduleWorkQueueDialog.WorkQueueKeys = keys;

            if (WorkQueueItemDetailsPanel.WorkQueue != null)
            {
                if (WorkQueueItemDetailsPanel.WorkQueue.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed)
                {
                    InformationDialog.Message = App_GlobalResources.SR.WorkQueueRescheduleFailed_ItemHasFailed;
                    InformationDialog.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    InformationDialog.Show();
                    return;
                }
                else if (WorkQueueItemDetailsPanel.WorkQueue.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                {
                    InformationDialog.Message = App_GlobalResources.SR.WorkQueueBeingProcessed;
                    InformationDialog.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    InformationDialog.Show();
                    return;
                }

                ScheduleWorkQueueDialog.Show();
            }
        }

        /// <summary>
        ///  Pops up a dialog box to let user to reschedule a work queue item
        /// </summary>
        private void DeleteWorkQueueItem()
        {
            DeleteWorkQueueDialog.WorkQueueItemKey = WorkQueueItemKey;
            if (WorkQueueItemKey != null)
            {
                DeleteWorkQueueDialog.Show();
            }
        }

        private void ResetWorkQueueItem()
        {

            ResetWorkQueueDialog.WorkQueueItemKey = WorkQueueItemKey;

            if (WorkQueueItemKey != null)
            {
                ResetWorkQueueDialog.Show();
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


        #endregion Private Methods


        #region Public Methods

        

        public override void DataBind()
        {
            if (WorkQueueItemKey != null)
            {
                WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
                _workQueue = adaptor.Get(WorkQueueItemKey);
                
                WorkQueueItemDetailsPanel.WorkQueue = _workQueue;

                if (_workQueue == null)
                {
                    if (!ItemNotAvailableAlertShown)
                    {
                        InformationDialog.Message = App_GlobalResources.SR.WorkQueueNotAvailable;
                        InformationDialog.MessageType =
                                MessageBox.MessageTypeEnum.ERROR; 
                        InformationDialog.Show();
                        ItemNotAvailableAlertShown = true;
                    }

                }
            }

            base.DataBind();

        }

        #endregion Public Methods
    }
}
