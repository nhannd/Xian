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
            get { return Boolean.Parse(ViewState[ "DeleteAll"].ToString()); }
            set { ViewState[ "DeleteAll"] = value; }
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