#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class EditHistoryDetailsColumn : System.Web.UI.UserControl
    {
        private StudyHistory _historyRecord;
        private WebEditStudyHistoryChangeDescription _description;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public StudyHistory HistoryRecord
        {
            set { _historyRecord = value; }
        }

        public WebEditStudyHistoryChangeDescription EditHistory
        {
            get
            {
                if (_description == null && _historyRecord != null)
                {
                    _description = XmlUtils.Deserialize<WebEditStudyHistoryChangeDescription>(_historyRecord.ChangeDescription.DocumentElement);
                }
                return _description;
            }
        }

        public string GetReason(string reasonString)
        {
            if (string.IsNullOrEmpty(reasonString)) return "None Specified";
            string[] reason = reasonString.Split(ImageServerConstants.ReasonCommentSeparator, StringSplitOptions.None);
            return reason[0];
        }

        public string GetComment(string reasonString)
        {
            if (string.IsNullOrEmpty(reasonString)) return "None Specified";
            string[] reason = reasonString.Split(ImageServerConstants.ReasonCommentSeparator, StringSplitOptions.None);
            return reason[1];
        }
    }
}