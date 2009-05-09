using System.Web.UI;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Helper class used in rendering the information encoded of a "WebEdit"
    /// StudyHistory record.
    /// </summary>
    internal class ProcessDuplicateChangeLogRendererFactory : IStudyHistoryColumnControlFactory
    {
        public Control GetChangeDescColumnControl(Control parent, StudyHistory historyRecord)
        {
            DuplicateProcessChangeLog control = parent.Page.LoadControl("~/Pages/Studies/StudyDetails/Controls/DuplicateProcessChangeLog.ascx") as DuplicateProcessChangeLog;
            control.HistoryRecord = historyRecord;
            return control;
        }
    }
}