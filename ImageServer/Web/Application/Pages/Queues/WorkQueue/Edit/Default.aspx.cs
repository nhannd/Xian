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
using ClearCanvas.ImageServer.Web.Common.Exceptions;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    public partial class Default : BasePage
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
                if (ViewState[ "ItemNotAvailableAlertShown"] == null)
                    return false;
                else
                    return (bool)ViewState[ "ItemNotAvailableAlertShown"];
            }
            set { ViewState[ "ItemNotAvailableAlertShown"] = value; }
        }

        #endregion Protected Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            WorkQueueItemDetailsPanel.RescheduleButtonClick += WorkQueueItemDetailsPanel_RescheduleButtonClick;
            WorkQueueItemDetailsPanel.ResetButtonClick += WorkQueueItemDetailsPanel_ResetButtonClick;
            WorkQueueItemDetailsPanel.DeleteButtonClick += WorkQueueItemDetailsPanel_DeleteButtonClick;
            WorkQueueItemDetailsPanel.ReprocessButtonClick += WorkQueueItemDetailsPanel_ReprocessButtonClick;

            DeleteWorkQueueDialog.WorkQueueItemDeleted += DeleteWorkQueueDialog_WorkQueueItemDeleted;
            ScheduleWorkQueueDialog.WorkQueueUpdated += ScheduleWorkQueueDialog_OnWorkQueueUpdated;
            MessageBox.Confirmed += MessageBox_Confirmed;

            ResetWorkQueueDialog.WorkQueueItemReseted += ResetWorkQueueDialog_WorkQueueItemReseted;
            
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

        void MessageBox_Confirmed(object data)
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
            Response.Redirect(ImageServerConstants.PageURLs.WorkQueueItemDeletedPage);
        }

        
        void ResetWorkQueueDialog_WorkQueueItemReseted(Model.WorkQueue item)
        {
            DataBind();
            WorkQueueItemDetailsPanel.Refresh();
        }


        void WorkQueueItemDetailsPanel_ReprocessButtonClick(object sender, WorkQueueDetailsButtonEventArg e)
        {
            Model.WorkQueue item = e.WorkQueueItem;
            ReprocessWorkQueueItem(item);
        }


        void WorkQueueItemDetailsPanel_DeleteButtonClick(object sender, WorkQueueDetailsButtonEventArg e)
        {
            Model.WorkQueue item = e.WorkQueueItem;
            DeleteWorkQueueItem(item);
        }

        void WorkQueueItemDetailsPanel_ResetButtonClick(object sender, WorkQueueDetailsButtonEventArg e)
        {
            Model.WorkQueue item = e.WorkQueueItem;
            ResetWorkQueueItem(item);
        }

        void WorkQueueItemDetailsPanel_RescheduleButtonClick(object sender, WorkQueueDetailsButtonEventArg e)
        {
            Model.WorkQueue item = e.WorkQueueItem;
            RescheduleWorkQueueItem(item);
        }


        /// <summary>
        ///  Pops up a dialog box to let user to reschedule a work queue item
        /// </summary>
        private void RescheduleWorkQueueItem(Model.WorkQueue item)
        {
            List<ServerEntityKey> keys = new List<ServerEntityKey>();
            keys.Add(item.GetKey());
            ScheduleWorkQueueDialog.WorkQueueKeys = keys;

            if (WorkQueueItemDetailsPanel.WorkQueue != null)
            {
                if (WorkQueueItemDetailsPanel.WorkQueue.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed)
                {
                    MessageBox.Message = App_GlobalResources.SR.WorkQueueRescheduleFailed_ItemHasFailed;
                    MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    MessageBox.Show();
                    return;
                }
                else if (WorkQueueItemDetailsPanel.WorkQueue.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                {
                    MessageBox.Message = App_GlobalResources.SR.WorkQueueBeingProcessed;
                    MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    MessageBox.Show();
                    return;
                }

                ScheduleWorkQueueDialog.Show();
            }
        }

        /// <summary>
        ///  Pops up a dialog box to let user to reschedule a work queue item
        /// </summary>
        private void DeleteWorkQueueItem(Model.WorkQueue item)
        {
            if (WorkQueueItemKey != null)
            {
                DeleteWorkQueueDialog.WorkQueueItemKey = WorkQueueItemKey;
                DeleteWorkQueueDialog.Show();
            }
        }

        private void ResetWorkQueueItem(Model.WorkQueue item)
        {
            ResetWorkQueueDialog.WorkQueueItemKey = WorkQueueItemKey;
            ResetWorkQueueDialog.Show();
        }

        private void ReprocessWorkQueueItem(Model.WorkQueue item)
        {
            WorkQueueController controller = new WorkQueueController();
            if (controller.ReprocessWorkQueueItem(item))
            {
                MessageBox.Message = App_GlobalResources.SR.ReprocessOK;
                MessageBox.MessageType = MessageBox.MessageTypeEnum.INFORMATION;
                MessageBox.Show();
            }
            else
            {
                MessageBox.Message = App_GlobalResources.SR.ReprocessFailed;
                MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                MessageBox.Show();
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
                        MessageBox.Message = App_GlobalResources.SR.WorkQueueNotAvailable;
                        MessageBox.MessageType =
                                MessageBox.MessageTypeEnum.ERROR; 
                        MessageBox.Show();
                        ItemNotAvailableAlertShown = true;
                    }

                }
            } else
            {
                ExceptionHandler.ThrowException(new WorkQueueItemNotFoundException());
            }

            base.DataBind();

        }

        #endregion Public Methods
    }
}
