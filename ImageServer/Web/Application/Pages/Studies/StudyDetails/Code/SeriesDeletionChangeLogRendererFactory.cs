using System.Web.UI;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.WebDeleteStudy.Extensions.LogHistory;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Helper class used in rendering the information encoded of a "SeriesDeleted"
    /// StudyHistory record.
    /// </summary>
    internal class SeriesDeletionChangeLogRendererFactory : IStudyHistoryColumnControlFactory
    {
        public Control GetChangeDescColumnControl(Control parent, StudyHistory historyRecord)
        {
            SeriesDeleteChangeLog control = parent.Page.LoadControl("~/Pages/Studies/StudyDetails/Controls/SeriesDeleteChangeLog.ascx") as SeriesDeleteChangeLog;
            control.ChangeLog = XmlUtils.Deserialize<SeriesDeletionChangeLog>(historyRecord.ChangeDescription);
            return control;
        }
    }
}