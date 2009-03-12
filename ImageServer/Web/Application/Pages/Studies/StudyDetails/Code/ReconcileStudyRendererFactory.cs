using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    internal class ReconcileStudyRendererFactory : IStudyHistoryColumnControlFactory
    {
        public Control GetChangeDescColumnControl(Control parent, StudyHistory historyRecord)
        {
            ReconcileHistoryDetailsColumn control = parent.Page.LoadControl("~/Pages/Studies/StudyDetails/Controls/ReconcileHistoryDetailsColumn.ascx") as ReconcileHistoryDetailsColumn;
            control.HistoryRecord = historyRecord;
            return control;
        }
    }

    internal class ReconcileHistoryRecord : StudyHistoryRecordBase
    {
        #region Private Fields

        private ReconcileDescription _updateDescription;
        #endregion

        #region Public Properties

        public ReconcileDescription UpdateDescription
        {
            get { return _updateDescription; }
            set { _updateDescription = value; }
        }

        #endregion
    }

}