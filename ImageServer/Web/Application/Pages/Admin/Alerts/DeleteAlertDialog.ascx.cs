using System;
using System.Collections.Generic;
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using MessageBox=ClearCanvas.ImageServer.Web.Application.Controls.MessageBox;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts
{
    /// <summary>
    /// A dialog box that prompts users for confirmation to delete a work queue entry and carries out the deletion if users do so.
    /// </summary>
    /// <remarks>
    /// To use this dialog, caller must indicate the <see cref="Alert"/> entry through the <see cref="AlertItemKey"/> property then
    /// call <see cref="Show"/> to display the dialog. Optionally, caller can register an event listener for <see cref="AlertItemDeleted"/>
    /// which is fired when users confirmed to delete the entry and it was sucessfully deleted.
    /// </remarks>
    public partial class DeleteAlertDialog : UserControl
    {
        #region Private Members
        private ServerEntityKey _alertItemKey;
        private Model.Alert _alert;
        #endregion Private Members

        #region Events

        /// <summary>
        /// Fired when the <see cref="Alert"/> object associated with this dialog box is deleted.
        /// </summary>
        public event AlertItemDeletedListener AlertItemDeleted;
        
        
        /// <summary>
        /// Defines handler for <see cref="AlertItemDeleted"/> event.
        /// </summary>
        /// <param name="item"></param>
        public delegate void AlertItemDeletedListener(Model.Alert item);

        
        #endregion Events

        #region Public Properties

        /// <summary>
        /// Sets / Gets the <see cref="ServerEntityKey"/> of the <see cref="Alert"/> item associated with this dialog
        /// </summary>
        public ServerEntityKey AlertItemKey
        {
            get { return _alertItemKey; }
            set { _alertItemKey = value; }
        }

        /// <summary>
        /// Sets / Gets a boolean indicating whether a single item will be deleted, or all items.
        /// </summary>
        public bool DeleteAll
        {
            get { return Boolean.Parse(ViewState[ClientID + "_DeleteAll"].ToString()); }
            set { ViewState[ClientID + "_DeleteAll"] = value; }
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
            if (DeleteAll)
            {
                AlertController controller = new AlertController();
                bool successful = controller.DeleteAllAlerts();

                if (successful)
                {
                    Platform.Log(LogLevel.Info, "All Alert items deleted by user.");
                }
                else
                {
                    Platform.Log(LogLevel.Error,
                                 "PreResetConfirmDialog_Confirmed: Unable to delete all Alert items.");

                    MessageBox.Message = App_GlobalResources.ErrorMessages.AlertDeleteFailed;
                    MessageBox.MessageType =
                        MessageBox.MessageTypeEnum.ERROR;
                    MessageBox.Show();
                }
                
            }
            else
            {
                ServerEntityKey key = data as ServerEntityKey;

                if (key != null)
                {
                    AlertAdaptor adaptor = new AlertAdaptor();
                    Model.Alert item = adaptor.Get(key);
                    if (item == null)
                    {
                        MessageBox.Message = App_GlobalResources.ErrorMessages.AlertNotAvailable;
                        MessageBox.MessageType =
                            MessageBox.MessageTypeEnum.ERROR;
                        MessageBox.Show();
                    }
                    else
                    {
                        try
                        {
                            bool successful = false;
                            AlertController controller = new AlertController();
                            List<Model.Alert> items = new List<Model.Alert>();
                            items.Add(item);

                            successful = controller.DeleteAlertItems(items);
                            if (successful)
                            {
                                Platform.Log(LogLevel.Info, "Alert item deleted by user : Alert Key={0}",
                                             item.GetKey().Key);

                                if (AlertItemDeleted != null)
                                    AlertItemDeleted(item);
                            }
                            else
                            {
                                Platform.Log(LogLevel.Error,
                                             "PreResetConfirmDialog_Confirmed: Unable to delete Alert item. GUID={0}",
                                             item.GetKey().Key);

                                MessageBox.Message = App_GlobalResources.ErrorMessages.AlertDeleteFailed;
                                MessageBox.MessageType =
                                    MessageBox.MessageTypeEnum.ERROR;
                                MessageBox.Show();
                            }
                        }
                        catch (Exception e)
                        {
                            Platform.Log(LogLevel.Error,
                                         "PreResetConfirmDialog_Confirmed: Unable to delete Alert item. GUID={0} : {1}",
                                         item.GetKey().Key, e.StackTrace);

                            MessageBox.Message =
                                String.Format(App_GlobalResources.ErrorMessages.AlertDeleteFailed_WithException,
                                              e.Message);
                            MessageBox.MessageType =
                                MessageBox.MessageTypeEnum.ERROR;
                            MessageBox.Show();
                        }
                    }

                    ((Default) Page).UpdateUI();
                }
            }
        }

        #endregion Private Methods

        #region Public Methods

        public override void DataBind()
        {
            if (AlertItemKey != null)
            {
                AlertAdaptor adaptor = new AlertAdaptor();
                _alert = adaptor.Get(AlertItemKey);
            }

            base.DataBind();
        }

        /// <summary>
        /// Displays the dialog box for deleting <see cref="Alert"/> entry.
        /// </summary>
        /// <remarks>
        /// The <see cref="AlertItemKey"/> to be deleted must be set prior to calling <see cref="Show"/>.
        /// </remarks>
        public void Show(bool deleteAll)
        {
            this.DeleteAll = deleteAll;
            
            DataBind();
            
            if(deleteAll)
            {
                PreDeleteConfirmDialog.MessageType =
                    MessageBox.MessageTypeEnum.YESNO;
                PreDeleteConfirmDialog.Message = App_GlobalResources.SR.AlertDeleteAllConfirm;
                PreDeleteConfirmDialog.Show();       
            }
            else
            {
                if (_alert != null)
                {
                    PreDeleteConfirmDialog.Data = AlertItemKey;
                    PreDeleteConfirmDialog.MessageType =
                        MessageBox.MessageTypeEnum.YESNO;
                    PreDeleteConfirmDialog.Message = App_GlobalResources.SR.AlertDeleteConfirm;
                    PreDeleteConfirmDialog.Show();
                }                
            }
        }

        /// <summary>
        /// Closes the dialog box
        /// </summary>
        public void Hide()
        {
            PreDeleteConfirmDialog.Close();
            MessageBox.Close();
        }

        #endregion Public Methods
    }
}