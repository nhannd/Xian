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
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using MessageBox=ClearCanvas.ImageServer.Web.Application.Controls.MessageBox;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    /// <summary>
    /// A dialog box that prompts users for confirmation to reset a work queue entry and carries out the reset if users do so.
    /// </summary>
    /// <remarks>
    /// To use this dialog, caller must indicate the <see cref="WorkQueue"/> entry through the <see cref="WorkQueueItemKey"/> property then
    /// call <see cref="Show"/> to display the dialog. Optionally, caller can register an event listener for <see cref="WorkQueueItemResetListener"/>
    /// which is fired when users confirmed to reset the entry and it was sucessfully reset.
    /// </remarks>
    public partial class ResetWorkQueueDialog : UserControl
    {
        #region Private Members
        private Model.WorkQueue _workQueue;
        #endregion Private Members


        #region Events
        public new event EventHandler<WorkQueueItemResetErrorEventArgs> Error;


        public delegate void WorkQueueItemResetListener(Model.WorkQueue item);
        public event WorkQueueItemResetListener WorkQueueItemReseted;

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
            get
            {
                if (ViewState["WorkQueueItemKey"] == null) return null;
                else return (ServerEntityKey)ViewState["WorkQueueItemKey"];
            }
            set
            {
                ViewState["WorkQueueItemKey"] = value;
            }

        }
        public bool IsShown
        {
            get
            {
                if (ViewState["IsShown"] == null) return false;
                else return (bool)ViewState["IsShown"];
            }
            set
            {
                ViewState["IsShown"] = value;
            }
        }



        #endregion Public Properties


        Model.WorkQueue WorkQueue
        {
            get
            {
                if (_workQueue == null)
                {
                    if (WorkQueueItemKey != null)
                    {
                        WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
                        _workQueue = adaptor.Get(WorkQueueItemKey);
                    }
                }

                return _workQueue;
            }
        }

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PreResetConfirmDialog.Confirmed += PreResetConfirmDialog_Confirmed;
            PreResetConfirmDialog.Cancel += Hide;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack && IsShown)
            {
                DataBind();
            }
        }

        #endregion Protected Methods

        #region Private Methods
        void PreResetConfirmDialog_Confirmed(object data)
        {
            Hide();

            ServerEntityKey key = data as ServerEntityKey;

            if (key != null)
            {
                WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
                Model.WorkQueue item = adaptor.Get(key);
                if (item == null)
                {
                    String errorMessage = String.Format(App_GlobalResources.SR.WorkQueueNotAvailable);
                    EventsHelper.Fire(Error, this, new WorkQueueItemResetErrorEventArgs(errorMessage, null));
                }
                else
                {
                    WorkQueueController controller = new WorkQueueController();
                    DateTime scheduledTime = item.ScheduledTime;
                    if (scheduledTime < Platform.Time)
                        scheduledTime = Platform.Time.AddSeconds(WorkQueueSettings.Default.WorkQueueProcessDelaySeconds);

                    DateTime expirationTime = item.ExpirationTime;
                    if (expirationTime < scheduledTime)
                        expirationTime = scheduledTime.AddSeconds(WorkQueueSettings.Default.WorkQueueExpireDelaySeconds);

                    try
                    {
                        List<Model.WorkQueue> items = new List<Model.WorkQueue>();
                        items.Add(item);

                        controller.ResetWorkQueueItems(items, scheduledTime, expirationTime);

                        Platform.Log(LogLevel.Info, "{0} Work Queue item reset:  Key={1}.", item.WorkQueueTypeEnum, item.GetKey() );
                        if (WorkQueueItemReseted != null)
                            WorkQueueItemReseted(item);

                        if (OnHide != null) OnHide();
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unable to reset {0} work queue item. Key={1}.", item.WorkQueueTypeEnum, item.GetKey());

                        String errorMessage = String.Format(App_GlobalResources.SR.WorkQueueResetFailed, e.Message);

                        EventsHelper.Fire(Error, this, new WorkQueueItemResetErrorEventArgs(errorMessage, e));
                    }

                }
            }


        }

        #endregion Private Methods

        #region Public Methods

        public override void DataBind()
        {
            if (WorkQueue != null)
            {

                if (WorkQueue.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                {
                    if (!String.IsNullOrEmpty(_workQueue.ProcessorID)) // somebody has claimed it
                    {
                        PreResetConfirmDialog.MessageType =
                        MessageBox.MessageTypeEnum.INFORMATION;
                        PreResetConfirmDialog.Message = App_GlobalResources.SR.WorkQueueBeingProcessed;
                        
                    }
                }
                else
                {
                    PreResetConfirmDialog.Data = WorkQueueItemKey;
                    PreResetConfirmDialog.MessageType =
                        MessageBox.MessageTypeEnum.YESNO;
                    PreResetConfirmDialog.Message = App_GlobalResources.SR.WorkQueueResetConfirm;
                }
            }

            base.DataBind();
        }

        /// <summary>
        /// Displays the dialog box for reseting the <see cref="WorkQueue"/> entry.
        /// </summary>
        /// <remarks>
        /// The <see cref="WorkQueueItemKey"/> to be deleted must be set prior to calling <see cref="Show"/>.
        /// </remarks>
       
        public void Show()
        {
            IsShown = true;
            
            DataBind();

            if (OnShow != null) OnShow();
            
        }

        /// <summary>
        /// Closes the dialog box
        /// </summary>
        public void Hide()
        {
            IsShown = false;
            if (OnHide != null) OnHide();
        }

        protected override void OnPreRender(EventArgs e)
        {

            PreResetConfirmDialog.Close();
            if (IsShown)
            {
                if (WorkQueue != null)
                {

                    PreResetConfirmDialog.Show();
                }
            }
            base.OnPreRender(e);
        }

        #endregion Public Methods

    }

    public class WorkQueueItemResetErrorEventArgs:EventArgs
    {
        private string _errorMessage;
        private Exception _exception;

        public WorkQueueItemResetErrorEventArgs(string error, Exception exception)
        {
            ErrorMessage = error;
            Exception = exception;
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }
    }
}