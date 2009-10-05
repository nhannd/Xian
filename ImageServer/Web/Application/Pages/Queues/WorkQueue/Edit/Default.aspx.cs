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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.App_GlobalResources;
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

        private Model.WorkQueue _workQueue;

        #region Protected Properties

        protected ServerEntityKey WorkQueueItemKey { get; set; }


        protected bool ItemNotAvailableAlertShown
        {
            get { return ViewState["ItemNotAvailableAlertShown"] != null && (bool) ViewState["ItemNotAvailableAlertShown"]; }
            set { ViewState["ItemNotAvailableAlertShown"] = value; }
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
            DeleteWorkQueueDialog.OnShow += delegate { WorkQueueItemDetailsPanel.ResetRefresh(true); };
            DeleteWorkQueueDialog.OnHide += delegate { WorkQueueItemDetailsPanel.ResetRefresh(true); };

            ScheduleWorkQueueDialog.WorkQueueUpdated += ScheduleWorkQueueDialog_OnWorkQueueUpdated;
            ScheduleWorkQueueDialog.OnShow += delegate { WorkQueueItemDetailsPanel.ResetRefresh(true); };
            ScheduleWorkQueueDialog.OnHide += delegate { WorkQueueItemDetailsPanel.ResetRefresh(true); };

            MessageBox.Confirmed += MessageBox_Confirmed;
            MessageBox.OnShow += delegate { WorkQueueItemDetailsPanel.ResetRefresh(true); };
            MessageBox.OnHide += delegate { WorkQueueItemDetailsPanel.ResetRefresh(true); };

            ResetWorkQueueDialog.WorkQueueItemReseted += ResetWorkQueueDialog_WorkQueueItemReseted;
            ResetWorkQueueDialog.Error += ResetWorkQueueDialog_Error;
            ResetWorkQueueDialog.OnShow += delegate { WorkQueueItemDetailsPanel.ResetRefresh(true); };
            ResetWorkQueueDialog.OnHide += delegate { WorkQueueItemDetailsPanel.ResetRefresh(true); };

            LoadWorkQueueItemKey();

            SetPageTitle(Titles.WorkQueuePageTitle);
        }

        private void ResetWorkQueueDialog_Error(object sender, WorkQueueItemResetErrorEventArgs e)
        {
            MessageBox.Message = e.ErrorMessage;
            MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
            MessageBox.Show();
            UpdatePanel.Update();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DataBind();
            UpdatePanel.Update();

            //Hide the UserPanel information
            var master = Master as MasterProperties;
            if(master != null) master.DisplayUserInformationPanel = false;
        }


        protected override void OnPreRender(EventArgs e)
        {
            if (WorkQueueItemKey == null || _workQueue == null)
            {
                // make sure all dialogs are closed 
                ScheduleWorkQueueDialog.Hide();
                WorkQueueItemDetailsPanel.Visible = false;
                ResetWorkQueueDialog.Hide();

                Message.Text = SR.WorkQueueNotAvailable;
                Message.Visible = true;

                UpdatePanel.Update();
            }
            else
            {
                Message.Visible = false;
            }

            WorkQueueItemDetailsPanel.AutoRefresh = !DeleteWorkQueueDialog.IsShown;

            base.OnPreRender(e);
        }

        #endregion Protected Methods

        #region Private Methods

        private void MessageBox_Confirmed(object data)
        {
            ItemNotAvailableAlertShown = true;
        }

        private void ScheduleWorkQueueDialog_OnWorkQueueUpdated(List<Model.WorkQueue> workqueueItems)
        {
            DataBind();
            WorkQueueItemDetailsPanel.Refresh();
        }


        private void DeleteWorkQueueDialog_WorkQueueItemDeleted(Model.WorkQueue item)
        {
            DataBind();
            Response.Redirect(ImageServerConstants.PageURLs.WorkQueueItemDeletedPage);
        }


        private void ResetWorkQueueDialog_WorkQueueItemReseted(Model.WorkQueue item)
        {
            DataBind();
            WorkQueueItemDetailsPanel.Refresh();
        }


        private void WorkQueueItemDetailsPanel_ReprocessButtonClick(object sender, WorkQueueDetailsButtonEventArg e)
        {
            Model.WorkQueue item = e.WorkQueueItem;
            ReprocessWorkQueueItem(item);
        }


        private void WorkQueueItemDetailsPanel_DeleteButtonClick(object sender, WorkQueueDetailsButtonEventArg e)
        {
            DeleteWorkQueueItem();
        }

        private void WorkQueueItemDetailsPanel_ResetButtonClick(object sender, WorkQueueDetailsButtonEventArg e)
        {
            ResetWorkQueueItem();
        }

        private void WorkQueueItemDetailsPanel_RescheduleButtonClick(object sender, WorkQueueDetailsButtonEventArg e)
        {
            Model.WorkQueue item = e.WorkQueueItem;
            RescheduleWorkQueueItem(item);
        }


        /// <summary>
        ///  Pops up a dialog box to let user to reschedule a work queue item
        /// </summary>
        private void RescheduleWorkQueueItem(Model.WorkQueue item)
        {
            if (item == null)
            {
                MessageBox.BackgroundCSS = string.Empty;
                MessageBox.Message = SR.SelectedWorkQueueNoLongerOnTheList;
                MessageBox.MessageStyle = "color: red; font-weight: bold;";
                MessageBox.MessageType =
                    MessageBox.MessageTypeEnum.ERROR;
                MessageBox.Show();

                return;
            }

            var keys = new List<ServerEntityKey>();
            keys.Add(item.GetKey());
            ScheduleWorkQueueDialog.WorkQueueKeys = keys;

            if (WorkQueueItemDetailsPanel.WorkQueue != null)
            {
                if (WorkQueueItemDetailsPanel.WorkQueue.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed)
                {
                    MessageBox.Message = SR.WorkQueueRescheduleFailed_ItemHasFailed;
                    MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    MessageBox.Show();
                    return;
                }
                if (WorkQueueItemDetailsPanel.WorkQueue.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                {
                    MessageBox.Message = SR.WorkQueueBeingProcessed;
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
        private void DeleteWorkQueueItem()
        {
            if (WorkQueueItemKey != null)
            {
                DeleteWorkQueueDialog.WorkQueueItemKey = WorkQueueItemKey;
                DeleteWorkQueueDialog.Show();
            }
        }

        private void ResetWorkQueueItem()
        {
            ResetWorkQueueDialog.WorkQueueItemKey = WorkQueueItemKey;
            ResetWorkQueueDialog.Show();
        }

        private void ReprocessWorkQueueItem(Model.WorkQueue item)
        {
            var controller = new WorkQueueController();
            if (controller.ReprocessWorkQueueItem(item))
            {
                MessageBox.Message = SR.ReprocessOK;
                MessageBox.MessageType = MessageBox.MessageTypeEnum.INFORMATION;
                MessageBox.Show();
            }
            else
            {
                MessageBox.Message = SR.ReprocessFailed;
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
                var adaptor = new WorkQueueAdaptor();
                _workQueue = adaptor.Get(WorkQueueItemKey);

                WorkQueueItemDetailsPanel.WorkQueue = _workQueue;

                if (_workQueue == null)
                {
                    if (!ItemNotAvailableAlertShown)
                    {
                        MessageBox.Message = SR.WorkQueueNotAvailable;
                        MessageBox.MessageType =
                            MessageBox.MessageTypeEnum.ERROR;
                        ItemNotAvailableAlertShown = true;
                    }
                }
            }
            else
            {
                ExceptionHandler.ThrowException(new WorkQueueItemNotFoundException());
            }

            base.DataBind();
        }

        public void HideRescheduleDialog()
        {
            ScheduleWorkQueueDialog.Hide();
            var itemList = FindControl("WorkQueueItemList") as WorkQueueItemList;
            if(itemList != null) itemList.Refresh();
        }

        #endregion Public Methods
    }
}