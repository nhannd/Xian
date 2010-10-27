#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Data;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class DuplicateProcessChangeLog : System.Web.UI.UserControl
    {
        private ProcessDuplicateChangeLog _changeLog;
        public StudyHistory HistoryRecord;

        protected ProcessDuplicateChangeLog ChangeLog
        {
            get
            {
                if (_changeLog==null)
                {
                    _changeLog = XmlUtils.Deserialize<ProcessDuplicateChangeLog>(HistoryRecord.ChangeDescription);
                }

                return _changeLog;
            }
        }

        /// <summary>
        /// </summary>
        protected String ActionDescription
        {
            get
            {
                if (ChangeLog == null)
                {
                    return "N/A";
                }
                else
                {
                    switch(ChangeLog.Action)
                    {
                        case ProcessDuplicateAction.Delete:
                            return "Delete duplicate SOPs.";
                        case ProcessDuplicateAction.OverwriteAsIs:
                            return "Overwrite existing SOPs and preserve data in duplicates.";
                        case ProcessDuplicateAction.OverwriteUseDuplicates:
                            return "Overwrite existing SOPs and update study with information in duplicates.";

                        case ProcessDuplicateAction.OverwriteUseExisting:
                            return "Overwrite existing SOPs and use the existing study information.";
                    
                        default:
                            return ChangeLog.Action.ToString();

                    }
                }
            }
        }

        protected String ChangeLogShortDescription
        {
            get
            {
                if (ChangeLog == null)
                {
                    return "N/A";
                }

                switch (ChangeLog.Action)
                {
                    case ProcessDuplicateAction.Delete:
                        return "Delete Duplicates.";
                    case ProcessDuplicateAction.OverwriteAsIs:
                        return "Accept Duplicates As Is.";
                    case ProcessDuplicateAction.OverwriteUseDuplicates:
                        return "Accept Duplicates and Update Existing Study";

                    case ProcessDuplicateAction.OverwriteUseExisting:
                        return "Accept Modified Duplicates.";

                    default:
                        return ChangeLog.Action.ToString();

                }
            }
        }

    }
}