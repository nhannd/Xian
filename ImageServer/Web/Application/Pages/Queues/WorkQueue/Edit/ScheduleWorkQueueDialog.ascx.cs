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
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using MessageBox=ClearCanvas.ImageServer.Web.Application.Controls.MessageBox;
using ModalDialog=ClearCanvas.ImageServer.Web.Application.Controls.ModalDialog;


namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// A dialog box that prompts users to reschedule a work queue entry and reschedule it if users confirm to do so.
    /// </summary>
    /// <remarks>
    /// To use this dialog, caller must indicate the <see cref="WorkQueue"/> entries to be rescheduled through the <see cref="WorkQueueKeys"/> property then
    /// call <see cref="Show"/> to display the dialog. Optionally, caller can register an event listener for <see cref="ResetWorkQueueDialog.WorkQueueItemResetListener"/>
    /// which is fired when users confirmed to reset the entry and it was sucessfully reset.
    /// </remarks>
    
    public partial class ScheduleWorkQueueDialog : UserControl
    {

        #region Private Members
        private List<Model.WorkQueue> _workQueues;
        #endregion

        #region Protected Properties
        protected List<Model.WorkQueue> WorkQueues
        {
            get
            {
                if (_workQueues != null)
                    return _workQueues;

                List<ServerEntityKey> keys = WorkQueueKeys;
                if (keys == null)
                    return null;

                WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
                _workQueues = new List<Model.WorkQueue>();
                foreach (ServerEntityKey key in keys)
                {
                    Model.WorkQueue wq = adaptor.Get(key);
                    if (wq != null)
                        _workQueues.Add(wq);
                }
                return _workQueues;
            }

        }

        #endregion Protected Properties

        #region Public Properties
        /// <summary>
        /// Sets or gets the list of <see cref="ServerEntityKey"/> for the <see cref="Model.WorkQueue"/> to be edit
        /// </summary>
        public List<ServerEntityKey> WorkQueueKeys
        {
            get {
                return ViewState[ClientID + "_WorkQueueKeys"] as List<ServerEntityKey>;
            }
            set
            {
                ViewState[ClientID + "_WorkQueueKeys"] = value;
                _workQueues = null; // invalidate this list
            }
        }

        public ModalDialog ScheduleWorkQueueItemModalDialog
        {
            get { return ModalDialog; }
        }

        #endregion Public Properties

        #region Events
        /// <summary>
        /// Defines the event handler for <see cref="WorkQueueUpdated"/> event.
        /// </summary>
        /// <param name="workqueueItems"></param>
        public delegate void WorkQueueUpdatedListener(List<Model.WorkQueue> workqueueItems);

        /// <summary>
        /// Fires after changes to the work queue items have been committed
        /// </summary>
        public event WorkQueueUpdatedListener WorkQueueUpdated;

        #endregion Events

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PreOpenConfirmDialog.Confirmed += PreOpenConfirmDialog_Confirmed;
            PreApplyChangeConfirmDialog.Confirmed += PreApplyChangeConfirmDialog_Confirmed;

            WorkQueueItemListPanel.WorkQueueItemListControl.SelectedIndexChanged += WorkQueueListControl_SelectedIndexChanged;

			WorkQueueItemListPanel.DataSourceCreated += delegate(WorkQueueDataSource source)
														{
															source.SearchKeys = WorkQueueKeys;
														};       
        }

        protected override void OnPreRender(EventArgs e)
        {
            WorkQueueItemListPanel.AutoRefresh = Visible 
                            && ModalDialog.State == ModalDialog.ShowState.Show
                            && WorkQueueKeys!=null && WorkQueueItemListPanel.WorkQueueItems != null;

            base.OnPreRender(e);
        }

        protected void WorkQueueListControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (WorkQueueKeys!=null)
            {
                if (WorkQueueItemListPanel.WorkQueueItems!=null && 
                    WorkQueueItemListPanel.WorkQueueItems.Count != WorkQueueKeys.Count)
                {
                    MessageDialog.Message = App_GlobalResources.SR.WorkQueueNoLongerAvailable;
                    MessageDialog.MessageType =
                        MessageBox.MessageTypeEnum.ERROR;
                    MessageDialog.Show();                    
                }
            }
        }

        protected void OnApplyButtonClicked(object sender, EventArgs arg)
        {
            ModalDialog.Hide();
            
            bool prompt = false;
            
            foreach (Model.WorkQueue wq in WorkQueues)
            {
                if (wq == null)
                {
                    // the workqueue no longer exist in the db
                    MessageDialog.Message = App_GlobalResources.SR.WorkQueueRescheduleFailed_ItemNotAvailable;
                    MessageDialog.MessageType =
                        MessageBox.MessageTypeEnum.ERROR;
                    MessageDialog.Show();
                    return; // don't apply the changes
                }
                else
                {
                    if (wq.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                    {
                        // prompt the user first
                        if (_workQueues.Count > 1)
                        {
                            PreApplyChangeConfirmDialog.Message = App_GlobalResources.SR.WorkQueueRescheduleConfirm_OneOrMoreAreBeingProcessed;
                        }
                        else
                        {
                            PreApplyChangeConfirmDialog.Message = App_GlobalResources.SR.WorkQueueRescheduleConfirm_ItemBeingProcessed;
                        }
                        PreApplyChangeConfirmDialog.Title = "Warning";
                        PreApplyChangeConfirmDialog.MessageType =
                            MessageBox.MessageTypeEnum.YESNO;
                        PreApplyChangeConfirmDialog.Show();
                        prompt = true;
                        break;
                    }
                    else if (wq.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed)
                    {
                        MessageDialog.Message = App_GlobalResources.SR.WorkQueueRescheduleFailed_ItemHasFailed;
                        MessageDialog.MessageType =
                            MessageBox.MessageTypeEnum.ERROR;
                        MessageDialog.Show();
                        return; // don't apply the changes
                    }
                }
            }

            if (!prompt)
            {
                ApplyChanges();
            }           
        }

        protected void ApplyChanges()
        {
            if (WorkQueues != null)
            {
                
                List<Model.WorkQueue> toBeUpdatedList = new List<Model.WorkQueue>();
                foreach (Model.WorkQueue item in WorkQueues)
                {
                    if (item != null)
                    {
                        toBeUpdatedList.Add(item);
                    }
                }

                if (toBeUpdatedList.Count>0)
                {
                    DateTime newScheduleTime = Platform.Time;

                        if (WorkQueueSettingsPanel.NewScheduledDateTime != null && WorkQueueSettingsPanel.ScheduleNow == false)
                        {
                            newScheduleTime = WorkQueueSettingsPanel.NewScheduledDateTime.Value;
                        }

                        if (newScheduleTime < Platform.Time && WorkQueueSettingsPanel.ScheduleNow == false)
                        {
                            MessageDialog.MessageType =
                                MessageBox.MessageTypeEnum.ERROR;
                            MessageDialog.Message = App_GlobalResources.SR.WorkQueueRescheduleFailed_MustBeInFuture;
                            MessageDialog.Show();
                            ModalDialog.Show();
                        }
                        else
                        {
                            DateTime expirationTime =
                                newScheduleTime.AddSeconds(WorkQueueSettings.Default.WorkQueueExpireDelaySeconds);

                            WorkQueuePriorityEnum priority = WorkQueueSettingsPanel.SelectedPriority;

                            try
                            {
                                WorkQueueController controller = new WorkQueueController();
                                bool result =
                                    controller.RescheduleWorkQueueItems(toBeUpdatedList, newScheduleTime, expirationTime,
                                                                        priority);
                                if (result)
                                {
                                    if (WorkQueueUpdated != null)
                                        WorkQueueUpdated(toBeUpdatedList);
                                }
                                else
                                {
                                    Platform.Log(LogLevel.Error, "Unable to reschedule work queue items for user");
                                    MessageDialog.MessageType =
                                        MessageBox.MessageTypeEnum.ERROR;
                                    MessageDialog.Message = "Unable to reschedule this/these work queue items";
                                    MessageDialog.Show();
                                }
                            }
                            catch (Exception e)
                            {
                                Platform.Log(LogLevel.Error, "Unable to reschedule work queue items for user : {0}",
                                             e.StackTrace);

                                MessageDialog.MessageType =
                                    MessageBox.MessageTypeEnum.ERROR;
                                MessageDialog.Message =
                                    String.Format(App_GlobalResources.SR.WorkQueueRescheduleFailed_Exception, e.Message);
                                MessageDialog.Show();
                            }
                        }
                    }               
            }
        }

        protected void OnCancelButtonClicked(object sender, EventArgs arg)
        {
            ModalDialog.Hide();
        }

        #endregion Protected Methods

        #region Private Methods

        void PreOpenConfirmDialog_Confirmed(object data)
        {
            Display();
        }

        void PreApplyChangeConfirmDialog_Confirmed(object data)
        {
            ApplyChanges();
        }

        private void Display()
        {
            if (WorkQueues != null && WorkQueues.Count > 0)
            {
                Model.WorkQueue workqueue = WorkQueues[0];
                WorkQueueSettingsPanel.SelectedPriority = workqueue.WorkQueuePriorityEnum;
                WorkQueueSettingsPanel.NewScheduledDateTime = workqueue.ScheduledTime;
            }

            
            ModalDialog.Show();
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Hides this dialog box
        /// </summary>
        public void Hide()
        {
            ModalDialog.Hide();
        }

        /// <summary>
        /// Displays this dialog for rescheduling the work queue(s)
        /// </summary>
        public void Show()
        {
            DataBind();

            if (WorkQueues == null)
                return;

            if (WorkQueueItemListPanel.WorkQueueItems.Count != WorkQueueKeys.Count)
            {
                MessageDialog.Message = App_GlobalResources.SR.WorkQueueNoLongerAvailable;
                MessageDialog.MessageType =
                    MessageBox.MessageTypeEnum.INFORMATION;
                MessageDialog.Show();    
            }

            WorkQueueSettingsPanel.ScheduleNow = false;
            
            Display();

        }

        #endregion Public Methods
    }
}