using System;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    internal enum ReconcileAction
    {
        Discard,
        Merge,
        SplitStudies
    }

    internal class ReconcileStudyRendererFactory : IStudyHistoryColumnRendererFactory
    {
        public Control GetChangeDescColumnControl(StudyHistory historyRecord)
        {
            XmlDocument doc = historyRecord.ChangeDescription;
            Label lb = new Label();
            ReconcileHistoryRecord desc = StudyHistoryRecordDecoder.ReadReconcileRecord(historyRecord);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Action = {0}", desc.UpdateDescription.ReconcileAction));
            sb.AppendLine(String.Format("Automatic = {0}", desc.Automatic ? "Yes" : "No"));

            sb.AppendLine();
            foreach (BaseImageLevelUpdateCommand cmd in desc.UpdateDescription.UpdateCommands)
            {
                sb.AppendFormat("{0}", cmd.ToString());
                sb.AppendLine();
            }
            lb.Text = HtmlUtility.Encode(sb.ToString());

            return lb;
        }
    }

    internal class ReconcileHistoryRecord : StudyHistoryRecordBase
    {
        #region Private Fields

        private ReconcileHistoryChangeDescription _updateDescription;
        #endregion

        #region Public Properties

        public ReconcileHistoryChangeDescription UpdateDescription
        {
            get { return _updateDescription; }
            set { _updateDescription = value; }
        }

        #endregion

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("Method = {0}", UpdateDescription.ReconcileAction));
            sb.AppendLine(String.Format("Automatic = {0}", Automatic? "Yes":"No"));
            foreach (BaseImageLevelUpdateCommand cmd in UpdateDescription.UpdateCommands)
            {
                sb.AppendLine(cmd.ToString());
            }

            return sb.ToString();
        }
    }

}