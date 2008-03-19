using System;
using System.Collections.Generic;
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit
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
                    ConfirmationDialog.Message = SR.WorkQueueNotAvailable;
                    ConfirmationDialog.MessageType =
                        ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog.MessageTypeEnum.ERROR;
                    ConfirmationDialog.Show();
                }
                else
                {

                    if (item.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("In Progress"))
                    {
                        ConfirmationDialog.Message = SR.WorkQueueBeingProcessed_CannotDelete;
                        ConfirmationDialog.MessageType =
                            ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog.MessageTypeEnum.ERROR;
                        ConfirmationDialog.Show();
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
                            Platform.Log(LogLevel.Info, "Work Queue item deleted by user : Item GUID={0}", item.GetKey().Key);

                            if (WorkQueueItemDeleted != null)
                                WorkQueueItemDeleted(item);
                        }
                        else
                        {
                            Platform.Log(LogLevel.Error,
                                         "PreResetConfirmDialog_Confirmed: Unable to delete work queue item. GUID={0}", item.GetKey().Key);

                            ConfirmationDialog.Message = SR.WorkQueueDeleteFailed;
                            ConfirmationDialog.MessageType =
                                ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog.MessageTypeEnum.ERROR;
                            ConfirmationDialog.Show();
                        }
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error,
                                         "PreResetConfirmDialog_Confirmed: Unable to delete work queue item. GUID={0} : {1}", item.GetKey().Key, e.StackTrace);

                        ConfirmationDialog.Message = String.Format(SR.WorkQueueDeleteFailed_WithException, e.Message);
                        ConfirmationDialog.MessageType =
                            ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog.MessageTypeEnum.ERROR;
                        ConfirmationDialog.Show();
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
                    ClearCanvas.ImageServer.Web.Application.Common.ConfirmationDialog.MessageTypeEnum.YESNO;
                PreDeleteConfirmDialog.Message = SR.WorkQueueDeleteConfirm;
                PreDeleteConfirmDialog.Show();
            }
        }

        /// <summary>
        /// Closes the dialog box
        /// </summary>
        public void Hide()
        {
            PreDeleteConfirmDialog.Close();
            ConfirmationDialog.Close();
        }

        #endregion Public Methods
    }
}