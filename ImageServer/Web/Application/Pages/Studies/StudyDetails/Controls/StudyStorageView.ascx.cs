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
using System.Web.UI.WebControls;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    /// <summary>
    /// Study level detailed information panel within the <see cref="StudyDetailsPanel"/>
    /// </summary>
    public partial class StudyStorageView : System.Web.UI.UserControl
    {
        #region Private members

        private Unit _width;
        private Study _study;

        #endregion Private members
        
        #region Public Properties

        /// <summary>
        /// Sets or gets the Study
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public Unit Width
        {
            get { return _width; }
            set { _width = value; }
        }

        #endregion Public Properties

        #region Protected Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public override void DataBind()
        {           
            StudyStorageViewControl.DataSource = null;
            StudyController studyController = new StudyController();
            StudyStorageViewControl.DataSource = studyController.GetStudyStorageLocation(_study);    
            
            base.DataBind();
        }

        protected void StudyStorageView_DataBound(object sender, EventArgs e)
        {
            StudyStorageLocation ssl = (StudyStorageViewControl.DataItem) as StudyStorageLocation;
            if (ssl != null)
            {
                Label statusLabel = StudyStorageViewControl.FindControl("Status") as Label;
                if (statusLabel != null)
                {
                    statusLabel.Text = ssl.StudyStatusEnum.Description;
                }
				Label queueStateLable = StudyStorageViewControl.FindControl("QueueState") as Label;
				if (queueStateLable != null)
				{
					queueStateLable.Text = ssl.QueueStudyStateEnum.Description;
				}
				Label studyFolder = StudyStorageViewControl.FindControl("StudyFolder") as Label;
                if (studyFolder != null)
                {
                    studyFolder.Text = ssl.GetStudyPath();
                }
                Label transferSyntaxUID = StudyStorageViewControl.FindControl("TransferSyntaxUID") as Label;
                if (transferSyntaxUID != null)
                {
                    transferSyntaxUID.Text = TransferSyntax.GetTransferSyntax(ssl.TransferSyntaxUid).Name;
                }
				Label tier = StudyStorageViewControl.FindControl("Tier") as Label;
				if (tier != null)
				{
					tier.Text = ssl.FilesystemTierEnum.Description;
				}
			}
        }

        #endregion Protected Methods

    }
}