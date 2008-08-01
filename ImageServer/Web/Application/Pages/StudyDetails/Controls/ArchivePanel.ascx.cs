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
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.StudyDetails.Controls
{
    /// <summary>
    /// Study level detailed information panel within the <see cref="StudyDetailsPanel"/>
    /// </summary>
    public partial class ArchivePanel : System.Web.UI.UserControl
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

        public override void DataBind()
        {
            StudyController studyController = new StudyController();
            ArchiveQueueGridView.DataSource = studyController.GetArchiveQueueItems(_study);
            ArchiveStudyStorageDetailsView.DataSource = studyController.GetArchiveStudyStorage(_study);
            base.DataBind();
        }

        protected void ArchiveQueueGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void ArchiveQueueGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            ArchiveQueueGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void ArchiveStudyStorageDetailsView_DataBound(object sender, EventArgs e)
        {
            ArchiveStudyStorage archiveStudyStorage = ArchiveStudyStorageDetailsView.DataItem as ArchiveStudyStorage;
            if (archiveStudyStorage != null)
            {
                Label xmlText = ArchiveStudyStorageDetailsView.FindControl("XmlText") as Label;
                if (xmlText != null && archiveStudyStorage.ArchiveXml != null)
                {
                    xmlText.Text = XmlUtilities.GetXmlDocumentAsString(archiveStudyStorage.ArchiveXml, true);
                }
            }
        }
    }
}