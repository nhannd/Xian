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
    public partial class StudyReprocessChangeLogControl : System.Web.UI.UserControl
    {
        private ReprocessStudyChangeLog _changeLog;

        public StudyHistory HistoryRecord;

        protected ReprocessStudyChangeLog ChangeLog
        {
            get
            {
                if (_changeLog == null)
                {
                    _changeLog = XmlUtils.Deserialize<ReprocessStudyChangeLog>(HistoryRecord.ChangeDescription);
                }

                return _changeLog;
            }
        }

    }
}