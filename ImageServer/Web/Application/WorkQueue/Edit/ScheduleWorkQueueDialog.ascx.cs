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
using AjaxControlToolkit;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Web.Common.Data;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit.ScheduleWorkQueueDialog.js", "application/x-javascript")]
namespace ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit
{
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit.ScheduleWorkQueueDialog", 
                            ResourcePath = "ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit.ScheduleWorkQueueDialog.js")]
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

        #endregion Public Properties

        #region Events
        public delegate void OnWorkQueueUpdatedHandler(List<Model.WorkQueue> workqueueItems);

        /// <summary>
        /// Fires after changes to the work queue items have been committed
        /// </summary>
        public event OnWorkQueueUpdatedHandler OnWorkQueueUpdated;

        #endregion Events

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PreOpenConfirmDialog.Confirmed += PreOpenConfirmDialog_Confirmed;
            PreApplyChangeConfirmDialog.Confirmed += PreApplyChangeConfirmDialog_Confirmed;

            WorkQueueItemListPanel.WorkQueueItemListControl.SelectedIndexChanged += new EventHandler(WorkQueueListControl_SelectedIndexChanged);
        }

        protected override void OnPreRender(EventArgs e)
        {
            WorkQueueItemListPanel.AutoRefresh = Visible 
                            && ModalDialog1.State == ClearCanvas.ImageServer.Web.Application.Common.ModalDialog.ShowState.Show
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
                    InformationDialog.Message = "One or more items is no longer available.";
                    InformationDialog.Show();

                    
                }
            }

        }

        protected void OnApplyButtonClicked(object sender, EventArgs arg)
        {
            //IList<Model.WorkQueue> workQueues = WorkQueues;

            bool prompt = false;
            foreach (Model.WorkQueue wq in WorkQueues)
            {
                if (wq == null)
                {
                    // the workqueue no longer exist in the db
                    InformationDialog.Message =
                        "One or more items you selected is no longer available and therefore will not be updated";
                    InformationDialog.Show();
                    break;
                }
                else
                {
                    if (wq.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("In Progress"))
                    {
                        // prompt the user first
                        if (_workQueues.Count > 1)
                        {
                            PreApplyChangeConfirmDialog.Message = @"At least one of the workqueue items is being processed. <br>
                                                                    Although you can save the changes you have made. They may be overwritten by the server when after the item has been processed.<P>

                                                                    Do you want to continue ?";
                        }
                        else
                        {
                            PreApplyChangeConfirmDialog.Message = @"This workqueue item is being processed.<br>
                                                                    Although you can save the changes you have made. They may be overwritten by the server when after the item has been processed.<P>
                                                                    Do you want to continue ?";
                        }
                        PreApplyChangeConfirmDialog.Title = "Warning";
                        PreApplyChangeConfirmDialog.MessageType =
                            ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog.MessageTypeEnum.YESNO;
                        PreApplyChangeConfirmDialog.Show();
                        prompt = true;
                        break;
                    }
                    else if (wq.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("Failed"))
                    {
                        // TODO: allow users to reset status
                        if (_workQueues.Count > 1)
                        {
                            PreApplyChangeConfirmDialog.Message = @"At least one of the workqueue items has failed. Although you can save the changes you have made, <br>
                                                                    failed items will not be processed by the server again.<P>

                                                                    Do you want to continue ?";

                        }
                        else
                        {
                            PreApplyChangeConfirmDialog.Message = @"This workqueue item is already failed. Although you can save the changes you have made, <br>failed items will not be processed by the server again.<P>

                                                                Do you want to continue ?";

                            PreApplyChangeConfirmDialog.Title = "Warning";
                            PreApplyChangeConfirmDialog.MessageType =
                                ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog.MessageTypeEnum.YESNO;
                            PreApplyChangeConfirmDialog.Show();
                            prompt = true;
                        }
                    }
                }
            }

            if (!prompt)
            {
                ApplyChanges();
            }

            ModalDialog1.Hide();
        }

        protected void ApplyChanges()
        {
            WorkQueueAdaptor adaptor = new WorkQueueAdaptor();

            if (WorkQueues != null)
            {
                List<Model.WorkQueue> updatedList = new List<ClearCanvas.ImageServer.Model.WorkQueue>();

                bool someAreNoLongerAvailable = false; // some items are no longer on the systems.

                foreach (Model.WorkQueue item in WorkQueues)
                {
                    if (item == null)
                    {
                        someAreNoLongerAvailable = true;
                    }
                    else
                    {
                        WorkQueueUpdateColumns updatedColumns = new WorkQueueUpdateColumns();
                        updatedColumns.WorkQueuePriorityEnum = WorkQueueSettingsPanel.SelectedPriority;

                        DateTime? newScheduleTime = WorkQueueSettingsPanel.NewScheduledDateTime;
                        if (newScheduleTime!= null)
                        {
                            // It doesn't make sense to reschedule something to start in the past. 
                            // 
                            if (newScheduleTime < Platform.Time)
                            {
                                newScheduleTime = Platform.Time.AddSeconds(WorkQueueSettings.Default.WorkQueueProcessDelaySeconds);
                            }

                            updatedColumns.ScheduledTime = newScheduleTime.Value;

                            updatedColumns.ExpirationTime = newScheduleTime.Value.AddSeconds(WorkQueueSettings.Default.WorkQueueExpireDelaySeconds );//expire 90 seconds after that

                        }

                        // the following fields should be reset too
                        updatedColumns.FailureCount = 0;
                        

                        if (adaptor.Update(item.GetKey(), updatedColumns))
                        {
                            updatedList.Add(item);
                        }
                    }
                }
                

                if (OnWorkQueueUpdated != null)
                    OnWorkQueueUpdated(updatedList);
            }
        }

        protected void OnCancelButtonClicked(object sender, EventArgs arg)
        {
            ModalDialog1.Hide();
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

            
            ModalDialog1.Show();
        }



        #endregion Private Methods

        #region Public Methods

        public override void DataBind()
        {
            if (WorkQueues!=null)   
            {
                WorkQueueItemCollection collection = new WorkQueueItemCollection();
                foreach (Model.WorkQueue item in WorkQueues)
                {
                    if (item != null)
                    {
                        collection.Add(WorkQueueSummaryAssembler.CreateWorkQueueSummary(item));
                        
                    }
                    else
                    {
                        // the work queue item is no longer available... don't show it on the list   
                    }
                }

                WorkQueueItemListPanel.WorkQueueItems = collection;
            }

            base.DataBind();

        }

        public void Hide()
        {
            ModalDialog1.Hide();
        }

        public void Show()
        {
            DataBind();

            if (WorkQueues == null)
                return;

            if (WorkQueueItemListPanel.WorkQueueItems.Count != WorkQueueKeys.Count)
            {
                InformationDialog.Message = "One or more items is no longer available and has been removed from the list.";
                InformationDialog.Show();    
            }
            
            Display();

        }


        #endregion Public Methods



    }
}