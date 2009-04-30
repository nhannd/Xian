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
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class StudyStateAlertPanel : System.Web.UI.UserControl
    {
        private StudySummary _studySummary;

        /// <summary>
        /// Message displayed
        /// </summary>
        protected Label Message;

        public StudySummary Study
        {
            get { return _studySummary; }
            set { _studySummary = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            Visible = false;
            Message.Text = String.Empty;
            base.OnInit(e);
        }

        public override void DataBind()
        {
            if (_studySummary!=null)
            {
                if (_studySummary.IsProcessing)
                {
                    ShowAlert(App_GlobalResources.SR.StudyBeingProcessed);
                }
                else if (_studySummary.IsLocked)
                {
                    ShowAlert(_studySummary.QueueStudyStateEnum.LongDescription);
                }
                else if (_studySummary.IsNearline)
                {
                    ShowAlert(App_GlobalResources.SR.StudyIsNearline);
                }
                else if (_studySummary.IsReconcileRequired)
                {
                    ShowAlert(App_GlobalResources.SR.StudyRequiresReconcilie);
                }
            }

            base.DataBind();
        }

        private void ShowAlert(string message)
        {
            Message.Text = message;
            Visible = true;
        }
    }
}