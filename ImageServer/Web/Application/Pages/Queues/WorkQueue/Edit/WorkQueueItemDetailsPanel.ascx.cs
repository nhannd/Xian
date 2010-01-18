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
using System.Web.UI;
using AjaxControlToolkit;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly:
    WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.js",
        "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit
{
    public class WorkQueueDetailsButtonEventArg : EventArgs
    {
        public WorkQueueDetailsButtonEventArg(Model.WorkQueue item)
        {
            WorkQueueItem = item;
        }

        public Model.WorkQueue WorkQueueItem { get; set; }
    }

    [ClientScriptResource(
        ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel",
        ResourcePath =
            "ClearCanvas.ImageServer.Web.Application.Pages.Queues.WorkQueue.Edit.WorkQueueItemDetailsPanel.js")]
    /// <summary>
        /// The <see cref="WorkQueue"/> details panel
        /// </summary>
    public partial class WorkQueueItemDetailsPanel : AJAXScriptControl
    {
        #region Private members

        private EventHandler<WorkQueueDetailsButtonEventArg> _deleteClickHandler;
        private WorkQueueDetailsViewBase _detailsView;
        private EventHandler<WorkQueueDetailsButtonEventArg> _reprocessClickHandler;

        private EventHandler<WorkQueueDetailsButtonEventArg> _rescheduleClickHandler;
        private EventHandler<WorkQueueDetailsButtonEventArg> _resetClickHandler;
        private Model.WorkQueue _workQueue;

        #endregion Private members

        #region Public Properties

        public Study ItemStudy
        {
            get { return _workQueue.Study; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ViewStudiesButtonClientID")]
        public string ViewStudiesButtonClientID
        {
            get { return StudyDetailsButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("OpenStudyPageUrl")]
        public string OpenStudyPageUrl
        {
            get { return Page.ResolveClientUrl(ImageServerConstants.PageURLs.StudyDetailsPage); }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("StudyInstanceUid")]
        public string StudyInstanceUid
        {
            get { return ItemStudy.StudyInstanceUid; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ServerAE")]
        public string ServerAE
        {
            get
            {
                ServerPartition partition = ServerPartition.Load(ItemStudy.ServerPartitionKey);
                return partition.AeTitle;
            }
        }

        /// <summary>
        /// Gets/Sets the <see cref="WorkQueue"/> item displayed in the panel
        /// </summary>
        /// <remarks>
        /// <see cref="DataBind"/> must be called to bind the work queue item data
        /// </remarks>
        public Model.WorkQueue WorkQueue
        {
            get { return _workQueue; }
            set { _workQueue = value; }
        }

        public bool AutoRefresh
        {
            get { return ViewState["AutoRefresh"] == null || (bool) ViewState["AutoRefresh"]; }
            set { ViewState["AutoRefresh"] = value; }
        }

        #endregion Public Properties

        #region Events

        /// <summary>
        /// Fired when user clicks on the Reschedule button
        /// </summary>
        public event EventHandler<WorkQueueDetailsButtonEventArg> RescheduleButtonClick
        {
            add { _rescheduleClickHandler += value; }
            remove { _rescheduleClickHandler -= value; }
        }

        /// <summary>
        /// Fired when user clicks on the Reset button
        /// </summary>
        public event EventHandler<WorkQueueDetailsButtonEventArg> ResetButtonClick
        {
            add { _resetClickHandler += value; }
            remove { _resetClickHandler -= value; }
        }

        /// <summary>
        /// Fired when user clicks on the Delete button
        /// </summary>
        public event EventHandler<WorkQueueDetailsButtonEventArg> DeleteButtonClick
        {
            add { _deleteClickHandler += value; }
            remove { _deleteClickHandler -= value; }
        }


        public event EventHandler<WorkQueueDetailsButtonEventArg> ReprocessButtonClick
        {
            add { _reprocessClickHandler += value; }
            remove { _reprocessClickHandler -= value; }
        }

        #endregion Events

        #region Protected Methods

        protected int GetRefreshInterval()
        {
            int interval = WorkQueueSettings.Default.NormalRefreshIntervalSeconds*1000;

            if (WorkQueue != null)
            {
                // the refresh rate should be high if the item was scheduled to start soon..
                TimeSpan span = WorkQueue.ScheduledTime.Subtract(Platform.Time);
                if (span < TimeSpan.FromMinutes(1))
                {
                    interval = WorkQueueSettings.Default.FastRefreshIntervalSeconds*1000;
                }
            }

            return interval;
        }

        public void ResetRefresh(bool enableAutoRefresh)
        {
            AutoRefresh = enableAutoRefresh;
            RefreshTimer.Reset(AutoRefresh);
        }


        protected override void OnPreRender(EventArgs e)
        {
            if (WorkQueue == null)
            {
                Visible = false;
            }

            if (RefreshTimer.Enabled)
            {
                if (WorkQueue != null)
                {
                    RefreshTimer.Interval = GetRefreshInterval();
                }
            }

            UpdateToolBarButtons();

            AutoRefreshIndicator.Visible = RefreshTimer.Enabled;
            base.OnPreRender(e);
        }

        protected void UpdateToolBarButtons()
        {
            RescheduleToolbarButton.Enabled = WorkQueue != null && WorkQueueController.CanReschedule(WorkQueue);
            ResetButton.Enabled = WorkQueue != null && WorkQueueController.CanReset(WorkQueue);
            DeleteButton.Enabled = WorkQueue != null && WorkQueueController.CanDelete(WorkQueue);
            ReprocessButton.Enabled = WorkQueue != null && WorkQueueController.CanReprocess(WorkQueue);
        }

        protected void Reschedule_Click(object sender, EventArgs arg)
        {
            EventsHelper.Fire(_rescheduleClickHandler, ReprocessButton, new WorkQueueDetailsButtonEventArg(WorkQueue));
        }

        protected void Delete_Click(object sender, EventArgs arg)
        {
            EventsHelper.Fire(_deleteClickHandler, ReprocessButton, new WorkQueueDetailsButtonEventArg(WorkQueue));
        }

        protected void Reprocess_Click(object sender, EventArgs arg)
        {
            EventsHelper.Fire(_reprocessClickHandler, ReprocessButton, new WorkQueueDetailsButtonEventArg(WorkQueue));
        }

        protected void Reset_Click(object sender, EventArgs arg)
        {
            EventsHelper.Fire(_resetClickHandler, ReprocessButton, new WorkQueueDetailsButtonEventArg(WorkQueue));
        }

        protected void RefreshTimer_Tick(object sender, EventArgs arg)
        {
            Refresh();
        }

        #endregion Protected Properties

        #region Public Methods

        public override void DataBind()
        {
            if (_detailsView == null && WorkQueue != null)
            {
                if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.AutoRoute)
                {
                    _detailsView = LoadControl("AutoRouteWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.WebMoveStudy)
                {
                    _detailsView = LoadControl("WebMoveStudyWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.WebEditStudy)
                {
                    _detailsView = LoadControl("WebEditStudyWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.MigrateStudy)
                {
                    _detailsView = LoadControl("TierMigrationWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.ProcessDuplicate)
                {
                    _detailsView = LoadControl("ProcessDuplicateWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else
                {
                    _detailsView = LoadControl("GeneralWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }

                // If the entry isn't failed but hasn't been updated for some time, display the alert message
                WorkQueueAlertPanelRow.Visible = false;
                if (!WorkQueue.WorkQueueStatusEnum.Equals(WorkQueueStatusEnum.Failed) &&
                    !ServerPlatform.IsActiveWorkQueue(WorkQueue))
                {
                    WorkQueueAlertPanelRow.Visible = true;
                    WorkQueueAlertPanel.Text =
                        WorkQueue.LastUpdatedTime > DateTime.MinValue
                            ? String.Format(
                                  "There does not seem to be any activity for this entry since {0}. The server may not be running or there is a problem with this entry.",
                                  WorkQueue.LastUpdatedTime)
                            : "There does not seem to be any activity for this entry. The server may not be running or there is a problem with this entry.";
                }
            }
            if (_detailsView != null)
                _detailsView.WorkQueue = WorkQueue;
            base.DataBind();
        }

        /// <summary>
        /// Refreshes the Work Queue Item Details Panel UI
        /// </summary>
        public void Refresh()
        {
            UpdatePanel.Update();
        }

        #endregion Public Methods

        protected void OnAutoRefreshDisabled(object sender, TimerEventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "AutoRefreshOff",
                                                "RaiseAppAlert('Auto refresh has been turned off due to inactivity.', 3000);",
                                                true);
        }
    }
}