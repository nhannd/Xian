using System;
using System.Collections.Generic;
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using MessageBox=ClearCanvas.ImageServer.Web.Application.Controls.MessageBox;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// A dialog box that prompts users for confirmation to delete a work queue entry and carries out the deletion if users do so.
    /// </summary>
    /// <remarks>
    /// To use this dialog, caller must indicate the <see cref="WorkQueue"/> entry through the <see cref="WorkQueueItemKey"/> property then
    /// call <see cref="Show"/> to display the dialog. Optionally, caller can register an event listener for <see cref="WorkQueueItemDeleted"/>
    /// which is fired when users confirmed to delete the entry and it was sucessfully deleted.
    /// </remarks>
    public partial class DeleteWorkQueueDialog : UserControl
    {
        #region Private Members
        private ServerEntityKey _workQueueItemKey;
        private Model.WorkQueue _workQueue;
        #endregion Private Members

        #region Events

        /// <summary>
        /// Fired when the <see cref="WorkQueue"/> object associated with this dialog box is deleted.
        /// </summary>
        public event WorkQueueItemDeletedListener WorkQueueItemDeleted;
        
        
        /// <summary>
        /// Defines handler for <see cref="WorkQueueItemDeleted"/> event.
        /// </summary>
        /// <param name="item"></param>
        public delegate void WorkQueueItemDeletedListener(Model.WorkQueue item);

        public delegate void OnShowEventHandler();
        public event OnShowEventHandler OnShow;

        public delegate void OnHideEventHandler();
        public event OnHideEventHandler OnHide;

        
        #endregion Events

        #region Public Properties

        /// <summary>
        /// Sets / Gets the <see cref="ServerEntityKey"/> of the <see cref="WorkQueue"/> item associated with this dialog
        /// </summary>
        public ServerEntityKey WorkQueueItemKey
        {
            get { return _workQueueItemKey; }
            set { _workQueueItemKey = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PreDeleteConfirmDialog.Confirmed += PreDeleteConfirmDialog_Confirmed;
            PreDeleteConfirmDialog.Cancel += Hide;
        }

        #endregion Protected Methods

        #region Private Methods

        void PreDeleteConfirmDialog_Confirmed(object data)
        {
            ServerEntityKey key = data as ServerEntityKey;

            if (key != null)
            {
                WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
                Model.WorkQueue item = adaptor.Get(key);
                if (item == null)
                {
                    MessageBox.Message = App_GlobalResources.SR.WorkQueueNotAvailable;
                    MessageBox.MessageType =
                        MessageBox.MessageTypeEnum.ERROR;
                    MessageBox.Show();
                }
                else
                {

                    if (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                    {
                        MessageBox.Message = App_GlobalResources.SR.WorkQueueBeingProcessed_CannotDelete;
                        MessageBox.MessageType =
                            MessageBox.MessageTypeEnum.ERROR;
                        MessageBox.Show();
                        return;
                    }

                    try
                    {
                        bool successful = false;
                        WorkQueueController controller = new WorkQueueController();
                        List<Model.WorkQueue> items = new List<Model.WorkQueue>();
                        items.Add(item);

                        successful = controller.DeleteWorkQueueItems(items);
                        if (successful)
                        {
                            Platform.Log(LogLevel.Info, "Work Queue item deleted by user : Item Key={0}", item.GetKey().Key);

                            if (WorkQueueItemDeleted != null)
                                WorkQueueItemDeleted(item);

                            if (OnHide != null) OnHide();
                        }
                        else
                        {
                            Platform.Log(LogLevel.Error,
                                         "PreResetConfirmDialog_Confirmed: Unable to delete work queue item. GUID={0}", item.GetKey().Key);

                            MessageBox.Message = App_GlobalResources.SR.WorkQueueDeleteFailed;
                            MessageBox.MessageType =
                                MessageBox.MessageTypeEnum.ERROR;
                            MessageBox.Show();
                        }
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error,
                                         "PreResetConfirmDialog_Confirmed: Unable to delete work queue item. GUID={0} : {1}", item.GetKey().Key, e.StackTrace);

                        MessageBox.Message = String.Format(App_GlobalResources.SR.WorkQueueDeleteFailed_WithException, e.Message);
                        MessageBox.MessageType =
                            MessageBox.MessageTypeEnum.ERROR;
                        MessageBox.Show();
                    }

                }
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
            }

            base.DataBind();
        }

        /// <summary>
        /// Displays the dialog box for deleting <see cref="WorkQueue"/> entry.
        /// </summary>
        /// <remarks>
        /// The <see cref="WorkQueueItemKey"/> to be deleted must be set prior to calling <see cref="Show"/>.
        /// </remarks>
        public void Show()
        {
            DataBind();

            if (_workQueue != null)
            {
                PreDeleteConfirmDialog.Data = WorkQueueItemKey;
                PreDeleteConfirmDialog.MessageType =
                    MessageBox.MessageTypeEnum.YESNO;
                PreDeleteConfirmDialog.Message = App_GlobalResources.SR.WorkQueueDeleteConfirm;
                PreDeleteConfirmDialog.Show();
            }

            if (OnShow != null) OnShow();
        }

        /// <summary>
        /// Closes the dialog box
        /// </summary>
        public void Hide()
        {
            if (OnHide != null) OnHide();
            
            PreDeleteConfirmDialog.Close();
            MessageBox.Close();
        }

        #endregion Public Methods
    }
}