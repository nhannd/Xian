using System;
using System.Collections.Generic;
using System.Web.UI;
using AjaxControlToolkit;
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
            WorkQueueItemListPanel.AutoRefresh = WorkQueueKeys!=null && WorkQueueItemListPanel.WorkQueueItems != null &&
                                                 WorkQueueItemListPanel.WorkQueueItems.Count == WorkQueueKeys.Count;
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
                bool someAreGone = false;

                foreach (Model.WorkQueue item in WorkQueues)
                {
                    if (item == null)
                    {
                        someAreGone = true;
                    }
                    else
                    {
                        WorkQueueUpdateColumns updatedColumns = new WorkQueueUpdateColumns();
                        updatedColumns.WorkQueuePriorityEnum = WorkQueueSettingsPanel.SelectedPriority;

                        DateTime? newScheduleTime = WorkQueueSettingsPanel.NewScheduledDateTime;
                        if (newScheduleTime!= null)
                        {
                            updatedColumns.ScheduledTime = newScheduleTime.Value;
                            if (newScheduleTime != null)
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

                if (updatedList.Count != WorkQueues.Count)
                {
                    InformationDialog.Message = "One or more items could not be updated";
                    InformationDialog.MessageType =
                        ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog.MessageTypeEnum.INFORMATION;
                    InformationDialog.Show();
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