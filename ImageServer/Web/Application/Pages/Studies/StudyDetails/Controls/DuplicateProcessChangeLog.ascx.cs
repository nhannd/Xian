using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
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
                else
                {
                    switch (ChangeLog.Action)
                    {
                        case ProcessDuplicateAction.Delete:
                            return "Delete duplicate SOPs.";
                        case ProcessDuplicateAction.OverwriteAsIs:
                            return "Accept As Is.";
                        case ProcessDuplicateAction.OverwriteUseDuplicates:
                            return "Accept + Update Study";

                        case ProcessDuplicateAction.OverwriteUseExisting:
                            return "Accept Modified.";

                        default:
                            return ChangeLog.Action.ToString();

                    }
                }
            }
        }

    }
}