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
using System.Security.Permissions;
using System.Web.UI;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;
using AuthorityTokens=ClearCanvas.ImageServer.Enterprise.Authentication.AuthorityTokens;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue
{
    [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.WorkQueue.Search)]
    public partial class Default : BasePage
    {

        #region Private members

        #endregion

        /// <summary>
        /// Sets/Gets a value which indicates whether auto refresh is on
        /// </summary>
        public bool AutoRefresh
        {
            get
            {
                if (ViewState["AutoRefresh"] == null)
                    return false;
                else
                    return (bool)ViewState["AutoRefresh"];
            }
            set
            {
                ViewState["AutoRefresh"] = value;
                RefreshTimer.Reset(value);
            }
        }

        public int RefreshRate
        {
            get
            {
                if (ViewState["RefreshRate"] == null)
                    return WorkQueueSettings.Default.NormalRefreshIntervalSeconds;
                else
                    return (int)ViewState["RefreshRate"];
            }
            set { ViewState["RefreshRate"] = value; }
        }


        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            string controlName = Request.Params.Get("__EVENTTARGET");
            if(controlName != null && controlName.Equals(RefreshRateTextBox.ClientID))
            {
                RefreshRate = Int32.Parse(RefreshRateTextBox.Text);                
            }

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            RefreshTimer.AutoDisabled += new EventHandler<TimerEventArgs>(RefreshTimer_AutoDisabled);
            ConfirmRescheduleDialog.Confirmed += ConfirmationContinueDialog_Confirmed;
            ScheduleWorkQueueDialog.WorkQueueUpdated += ScheduleWorkQueueDialog_OnWorkQueueUpdated;
            ScheduleWorkQueueDialog.OnShow += DisableRefresh;
            ScheduleWorkQueueDialog.OnHide += delegate() { RefreshTimer.Reset(AutoRefresh); };
            ResetWorkQueueDialog.WorkQueueItemReseted += ResetWorkQueueDialog_WorkQueueItemReseted;
            ResetWorkQueueDialog.OnShow += DisableRefresh;
            ResetWorkQueueDialog.OnHide += delegate() { RefreshTimer.Reset(AutoRefresh); };
            DeleteWorkQueueDialog.WorkQueueItemDeleted += DeleteWorkQueueDialog_WorkQueueItemDeleted;
            DeleteWorkQueueDialog.OnShow += DisableRefresh;
            DeleteWorkQueueDialog.OnHide += delegate() { RefreshTimer.Reset(AutoRefresh); };
            InformationDialog.OnShow += DisableRefresh;
            InformationDialog.OnHide += delegate() { RefreshTimer.Reset(AutoRefresh); };
            ServerPartitionTabs.SetupLoadPartitionTabs(delegate(ServerPartition partition)
                                                           {
                                                               SearchPanel panel =
                                                                   LoadControl("SearchPanel.ascx") as SearchPanel;
                                                               panel.ServerPartition = partition;
                                                               panel.ID = "SearchPanel_" + partition.AeTitle;

                                                               panel.EnclosingPage = this;

                                                               return panel;
                                                           });

            if (!Page.IsPostBack)
            {
                AutoRefresh = true;
                RefreshTimer.Enabled = true;
                RefreshTimer.Interval = (int)TimeSpan.FromSeconds(Math.Max(WorkQueueSettings.Default.NormalRefreshIntervalSeconds, 5)).TotalMilliseconds;// min refresh rate: every 5 sec 
                RefreshRateTextBox.Text = TimeSpan.FromMilliseconds(RefreshTimer.Interval).TotalSeconds.ToString();
                DataBind();
            }
            
            Page.Title = App_GlobalResources.Titles.WorkQueuePageTitle;
            
        }

        void RefreshTimer_AutoDisabled(object sender, TimerEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "AutoRefreshOff",
                     "RaiseAppAlert('Auto refresh has been turned off due to inactivity.', 3000);",
                     true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            RefreshTimer.Enabled &= AutoRefresh && !DeleteWorkQueueDialog.IsShown && !ScheduleWorkQueueDialog.IsShown;
            RefreshRateEnabled.SelectedValue = AutoRefresh && RefreshTimer.Enabled ? "Y" : "N";
            RefreshTimer.Interval = (int)TimeSpan.FromSeconds(Int32.Parse(RefreshRateTextBox.Text)).TotalMilliseconds;// min refresh rate: every 5 sec 
            RefreshIntervalPanel.Visible = RefreshRateEnabled.SelectedValue == "Y";

            // If the request is casued by the refresh timer consecutively N times, 
            // it is an indication that users are not using the screen, we should turn off the auto-refresh

            base.OnPreRender(e);
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

        private void DisableRefresh()
        {
            RefreshTimer.Reset(false);
        }

        #endregion Private Methods

        protected void RefreshRate_IndexChanged(Object sender, EventArgs arg)
        {
            if (RefreshRateEnabled.SelectedItem.Value.Equals("Y"))
            {
                AutoRefresh = true;
                RefreshRate = Int32.Parse(RefreshRateTextBox.Text);
                RefreshRateTextBox.Enabled = true;
            }
            else
            {
                AutoRefresh = false;
                RefreshRateTextBox.Enabled = false;
            }
        }

        protected void RefreshTimer_Tick(object sender, EventArgs e)
        {
            ServerPartitionTabs.Update(true);
        }

        protected void OnResetWorkQueueError(object sender, WorkQueueItemResetErrorEventArgs e)
        {
            MessageBox.MessageType = MessageBox.MessageTypeEnum.ERROR;
            MessageBox.Message = e.ErrorMessage;
            MessageBox.Show();
            UpdatePanel.Update();
        }
    }
}
