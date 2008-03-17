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

namespace ClearCanvas.ImageServer.Web.Application.WorkQueue.Edit
{

    public partial class WorkQueueItemDetailsPanel : System.Web.UI.UserControl
    {
        #region Private members

        private Model.WorkQueue _workQueue;
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

        public delegate void OnRescheduleButtonClickHandler();

        /// <summary>
        /// Fired when user clicks on the Reschedule button
        /// </summary>
        public event OnRescheduleButtonClickHandler RescheduleButtonClick;

        #endregion Events

        #region Protected Methods

        protected override void OnPreRender(EventArgs e)
        {
            if (WorkQueue==null)
            {
                this.Visible = false;
            }

            RefreshTimer.Enabled = AutoRefresh && Visible;


            base.OnPreRender(e);
        }

        protected void Reschedule_Click(object sender, EventArgs arg)
        {
            if (RescheduleButtonClick != null)
                RescheduleButtonClick();
        }

        protected void RefreshTimer_Tick(object sender, EventArgs arg)
        {
            Refresh();
        }

        #endregion Protected Properties


        #region Public Methods

        public override void DataBind()
        {
            WorkQueueDetailsView.WorkQueue = WorkQueue;

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