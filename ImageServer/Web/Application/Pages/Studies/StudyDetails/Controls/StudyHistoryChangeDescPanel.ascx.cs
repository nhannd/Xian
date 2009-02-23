using System;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls
{
    public partial class StudyHistoryChangeDescPanel : System.Web.UI.UserControl
    {
        private StudyHistory _historyRecord;

        public StudyHistory HistoryRecord
        {
            get { return _historyRecord; }
            set { _historyRecord = value; }
        }

        public override void DataBind()
        {
            if (_historyRecord != null)
            {
                // Use different control to render the content of the ChangeDescription column.
                IStudyHistoryColumnRendererFactory render = GetRenderer(_historyRecord);
                Control ctrl = render.GetChangeDescColumnControl(_historyRecord);
                SummaryPlaceHolder.Controls.Add(ctrl);
            }
            base.DataBind();
        } 

        private IStudyHistoryColumnRendererFactory GetRenderer(StudyHistory record)
        {
            if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.StudyReconciled)
                return new ReconcileStudyRendererFactory();
            else if (record.StudyHistoryTypeEnum == StudyHistoryTypeEnum.WebEdited)
                return new StudyEditRendererFactory();
            else
                return new DefaultStudyHistoryRendererFactory();
        }

    }
}