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
using ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy.Extensions.LogHistory;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class SeriesDeleteChangeLog : System.Web.UI.UserControl
    {
        private SeriesDeletionChangeLog _changeLog;

        public SeriesDeletionChangeLog ChangeLog
        {
            get { return _changeLog; }
            set { _changeLog = value; }
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
            if (reason.Length == 1) return "None Specified";
            return reason[1];
        }
    }
}