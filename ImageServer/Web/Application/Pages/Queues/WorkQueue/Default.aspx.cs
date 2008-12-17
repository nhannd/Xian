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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue
{
    /// <summary>
    /// Work Queue Search Page
    /// </summary>
    public partial class Default : BasePage
    {
        #region Private members

        #endregion

        public Edit.ScheduleWorkQueueDialog ScheduleWorkQueueItemDialog
        {
            get { return this.ScheduleWorkQueueDialog; }
        }

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
                DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ConfirmRescheduleDialog.Confirmed += ConfirmationContinueDialog_Confirmed;
            ScheduleWorkQueueDialog.WorkQueueUpdated += ScheduleWorkQueueDialog_OnWorkQueueUpdated;
            ResetWorkQueueDialog.WorkQueueItemReseted += ResetWorkQueueDialog_WorkQueueItemReseted;
            DeleteWorkQueueDialog.WorkQueueItemDeleted += DeleteWorkQueueDialog_WorkQueueItemDeleted;
            ServerPartitionTabs.SetupLoadPartitionTabs(delegate(ServerPartition partition)
                                                           {
                                                               SearchPanel panel =
                                                                   LoadControl("SearchPanel.ascx") as SearchPanel;
                                                               panel.ServerPartition = partition;
                                                               panel.ID = "SearchPanel_" + partition.AeTitle;

                                                               panel.EnclosingPage = this;

                                                               return panel;
                                                           });
        }

       
        #endregion Protected Methods


        #region Public Methods

        /// <summary>
        /// Open a new page to display the work queue item details.
        /// </summary>
        /// <param name="itemKey"></param>
        public void ViewWorkQueueItem(ServerEntityKey itemKey)
        {
            string url = String.Format("{0}?uid={1}", ResolveClientUrl(ImageServerConstants.PageURLs.WorkQueueItemDetailsPage), itemKey.Key);

            string script =
            @"      
                    //note: this will not work with IE 7 tabs. Users must enable [Always switch to new tab] in IE settings
                    popupWin = window.open('@@URL@@'); 
                    setTimeout(""popupWin.focus()"", 500)
                    
            ";

            script = script.Replace("@@URL@@", url);

            ScriptManager.RegisterStartupScript(this, GetType(), itemKey.Key.ToString(), script, true);
        }

        /// <summary>
        /// Popup a dialog to reschedule the work queue item.
        /// </summary>
        /// <param name="itemKey"></param>
        public void RescheduleWorkQueueItem(ServerEntityKey itemKey)
        {
            if (itemKey == null)
                return;

            WorkQueueAdaptor adaptor = new WorkQueueAdaptor();

            Model.WorkQueue item = adaptor.Get(itemKey);

            if (item==null)
            {
                InformationDialog.Message = App_GlobalResources.SR.WorkQueueNotAvailable;
                InformationDialog.Show();

            }
            else
            {
                if (item.WorkQueueStatusEnum == WorkQueueStatusEnum.InProgress)
                {
                    // prompt the user first
                    InformationDialog.Message = App_GlobalResources.SR.WorkQueueBeingProcessed_CannotReschedule;
                    InformationDialog.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    InformationDialog.Show();
                    return;

                }
                else if (item.WorkQueueStatusEnum == WorkQueueStatusEnum.Failed)
                {
                    InformationDialog.Message = App_GlobalResources.SR.WorkQueueFailed_CannotReschedule;
                    InformationDialog.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    InformationDialog.Show();
                    return;
                }


                ScheduleWorkQueueDialog.WorkQueueKeys = new List<ServerEntityKey>();
                ScheduleWorkQueueDialog.WorkQueueKeys.Add(itemKey);
                ScheduleWorkQueueDialog.Show();
                
            }
        }


        public void ResetWorkQueueItem(ServerEntityKey itemKey)
        {
            if (itemKey != null)
            {
                ResetWorkQueueDialog.WorkQueueItemKey = itemKey;
                ResetWorkQueueDialog.Show();
            }
        }

        public void DeleteWorkQueueItem(ServerEntityKey itemKey)
        {
            if (itemKey != null)
            {
                DeleteWorkQueueDialog.WorkQueueItemKey = itemKey;
                DeleteWorkQueueDialog.Show();
            }
        }

        public void ReprocessWorkQueueItem(ServerEntityKey itemKey)
        {
            if (itemKey != null)
            {
                Model.WorkQueue item = Model.WorkQueue.Load(itemKey);
                WorkQueueController controller = new WorkQueueController();
                if (controller.ReprocessWorkQueueItem(item))
                {
                    InformationDialog.Message = App_GlobalResources.SR.ReprocessOK;
                    InformationDialog.MessageType = MessageBox.MessageTypeEnum.INFORMATION;
                    InformationDialog.Show();
                }
                else
                {
                    InformationDialog.Message = App_GlobalResources.SR.ReprocessFailed;
                    InformationDialog.MessageType = MessageBox.MessageTypeEnum.ERROR;
                    InformationDialog.Show();
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void ScheduleWorkQueueDialog_OnWorkQueueUpdated(List<Model.WorkQueue> workqueueItems)
        {
            List<ServerEntityKey> updatedPartitions = new List<ServerEntityKey>();
            foreach (Model.WorkQueue item in workqueueItems)
            {
                ServerEntityKey partitionKey = item.ServerPartitionKey;
                if (!updatedPartitions.Contains(partitionKey))
                {
                    updatedPartitions.Add(partitionKey);

                    ServerPartitionTabs.Update(partitionKey);
                }
            }
        }

        


        void ResetWorkQueueDialog_WorkQueueItemReseted(Model.WorkQueue item)
        {
            ServerEntityKey partitionKey = item.ServerPartitionKey;
            ServerPartitionTabs.Update(partitionKey);
        }

        void DeleteWorkQueueDialog_WorkQueueItemDeleted(Model.WorkQueue item)
        {
            ServerEntityKey partitionKey = item.ServerPartitionKey;
            ServerPartitionTabs.Update(partitionKey);
        }


        void ConfirmationContinueDialog_Confirmed(object data)
        {
            DataBind();
            ScheduleWorkQueueDialog.Show();
        }

        #endregion Private Methods
    }
}
