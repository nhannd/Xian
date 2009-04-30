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
using System.IO;
using System.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Utilities;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.FileSystemQueueGridView.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls.FileSystemQueueGridView",
                       ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Scripts.FileSystemQueueGridView.js")]
    
    /// <summary>
    /// list panel within the <see cref="StudyDetailsPanel"/> that contains all of the File System queue
    /// entries for this study.
    /// </summary>
    public partial class FileSystemQueueGridView : ScriptUserControl
    {
        #region Private members

        private Study _study = null;

        #endregion Private members

        #region Public properties

        /// <summary>
        /// Gets or sets the list of series to be displayed
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }      

        public Web.Common.WebControls.UI.GridView FSQueueListControl
        {
            get { return FSQueueGridView; }
        }

        #endregion Public properties

        #region Constructors

        public FileSystemQueueGridView()
            : base(false, HtmlTextWriterTag.Div)
            {
            }

        #endregion Constructors

            
        #region Protected methods

        protected void FSQueueGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void FSQueueGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            FSQueueGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void FSQueueGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            FilesystemQueue fsq = e.Row.DataItem as FilesystemQueue;
            if (fsq != null)
            {
                Label xmlText = e.Row.FindControl("XmlText") as Label;
                if (xmlText != null && fsq.QueueXml != null)
                {
                    xmlText.Text = XmlUtils.GetXmlDocumentAsString(fsq.QueueXml, true);
                }
            }
        }

        #endregion Protected methods

        #region Public methods

        public override void DataBind()
        {
            if (Study != null)
            {
                StudyController controller = new StudyController();
                FSQueueGridView.DataSource = controller.GetFileSystemQueueItems(Study);
            }

            base.DataBind();
        }

        #endregion Public methods

    }
}