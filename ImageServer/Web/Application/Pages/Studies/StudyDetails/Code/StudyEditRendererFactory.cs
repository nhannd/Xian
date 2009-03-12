using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;
using ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy;
using ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Controls;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Helper class used in rendering the information encoded of a "WebEdit"
    /// StudyHistory record.
    /// </summary>
    internal class StudyEditRendererFactory : IStudyHistoryColumnControlFactory
    {
        public Control GetChangeDescColumnControl(Control parent, StudyHistory historyRecord)
        {
            EditHistoryDetailsColumn control = parent.Page.LoadControl("~/Pages/Studies/StudyDetails/Controls/EditHistoryDetailsColumn.ascx") as EditHistoryDetailsColumn;
            control.HistoryRecord = historyRecord;
            return control;
        }
    }

    /// <summary>
    /// Helper class to decode the information of a "WebEdit" study history record.
    /// </summary>
    class WebEditStudyHistoryRecord : StudyHistoryRecordBase
    {
        #region Private Fields
        private StudyInformation _originalStudy;
        private WebEditStudyHistoryChangeDescription _updateDescription;

        #endregion

        #region Public Properties

        public StudyInformation OriginalStudyData
        {
            get { return _originalStudy; }
            set { _originalStudy = value; }
        }

        public WebEditStudyHistoryChangeDescription UpdateDescription
        {
            get { return _updateDescription; }
            set { _updateDescription = value; }
        }
        #endregion

        #region Constructors

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Trigger: {0}",
                            this.UpdateDescription.EditType == EditType.WebEdit ? "Manual (Web UI)" : "Unknown");
            sb.AppendLine();
            sb.AppendFormat("Updates:");
            sb.AppendLine();
            foreach (BaseImageLevelUpdateCommand cmd in UpdateDescription.UpdateCommands)
            {
                sb.AppendFormat("{0}", cmd);
                sb.AppendLine();
            }
            return sb.ToString();
        }

        #region Public Static Methods

        /// <summary>
        /// Creates an instance of <see cref="WebEditStudyHistoryRecord"/>
        /// from a <see cref="StudyHistory"/>
        /// </summary>
        /// <param name="historyRecord"></param>
        /// <returns></returns>
        #endregion
    }

}