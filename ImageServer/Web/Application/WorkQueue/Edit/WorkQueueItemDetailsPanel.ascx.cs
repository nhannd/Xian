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
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit
{
    /// <summary>
    /// The <see cref="WorkQueue"/> details panel
    /// </summary>
    public partial class WorkQueueItemDetailsPanel : UserControl
    {
        #region Private members

        private Model.WorkQueue _workQueue;
        private WorkQueueDetailsViewBase _detailsView;
        #endregion Private members

        #region Public Properties

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
            get
            {
                if (ViewState[ClientID+"_AutoRefresh"]==null)
                    return true;
                else
                    return (bool)ViewState[ClientID + "_AutoRefresh"];
            }
            set { ViewState[ClientID + "_AutoRefresh"] = value; }
        }

        #endregion Public Properties

        #region Events

        public delegate void RescheduleButtonClickListener();
        public delegate void ResetButtonClickListener();
        public delegate void DeleteButtonClickListener();

        /// <summary>
        /// Fired when user clicks on the Reschedule button
        /// </summary>
        public event RescheduleButtonClickListener RescheduleButtonClick;
        /// <summary>
        /// Fired when user clicks on the Reset button
        /// </summary>
        public event ResetButtonClickListener ResetButtonClick;

        /// <summary>
        /// Fired when user clicks on the Delete button
        /// </summary>
        public event DeleteButtonClickListener DeleteButtonClick;
        

        #endregion Events

        #region Protected Methods

        protected int GetRefreshInterval()
        {
            int interval = WorkQueueSettings.Default.NormalRefreshIntervalSeconds * 1000;

            if (WorkQueue != null)
            {
                // the refresh rate should be high if the item was scheduled to start soon..
                TimeSpan span = WorkQueue.ScheduledTime.Subtract(Platform.Time);
                if (span < TimeSpan.FromMinutes(1))
                {
                    interval = WorkQueueSettings.Default.FastRefreshIntervalSeconds * 1000;
                }
                
            }

            return interval;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (WorkQueue==null)
            {
                Visible = false;
            }

            RefreshTimer.Enabled = AutoRefresh && Visible;

            if (RefreshTimer.Enabled)
            {
                if (WorkQueue != null)
                {
                    RefreshTimer.Interval = GetRefreshInterval();
                }
            }


            UpdateToolBarButtons();

            base.OnPreRender(e);
        }

        

        protected void UpdateToolBarButtons()
        {
            RescheduleToolbarButton.Enabled = WorkQueue != null && WorkQueueController.CanReschedule(WorkQueue);
            ResetButton.Enabled = WorkQueue != null && WorkQueueController.CanReset(WorkQueue);
            DeleteButton.Enabled = WorkQueue != null && WorkQueueController.CanDelete(WorkQueue);
        }

        protected void Reschedule_Click(object sender, EventArgs arg)
        {
            if (RescheduleButtonClick != null)
                RescheduleButtonClick();
        }

        protected void Delete_Click(object sender, EventArgs arg)
        {
            if (DeleteButtonClick != null)
                DeleteButtonClick();
        }

        protected void Reset_Click(object sender, EventArgs arg)
        {
            if (ResetButtonClick != null)
                ResetButtonClick();
        }

        protected void RefreshTimer_Tick(object sender, EventArgs arg)
        {
            Refresh();
        }

        #endregion Protected Properties

        #region Public Methods

        public override void DataBind()
        {
            if (_detailsView==null && WorkQueue != null)
            {
                if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.GetEnum("AutoRoute"))
                {
                    _detailsView = LoadControl("AutoRouteWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                    
                }
                else if (WorkQueue.WorkQueueTypeEnum == WorkQueueTypeEnum.GetEnum("WebMoveStudy"))
                {
                    _detailsView = LoadControl("WebMoveStudyWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
                }
                else
                {
                    _detailsView = LoadControl("GeneralWorkQueueDetailsView.ascx") as WorkQueueDetailsViewBase;
                    WorkQueueDetailsViewPlaceHolder.Controls.Add(_detailsView);
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

    }
}