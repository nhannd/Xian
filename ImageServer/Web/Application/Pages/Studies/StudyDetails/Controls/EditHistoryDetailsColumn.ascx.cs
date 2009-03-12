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
    }
}