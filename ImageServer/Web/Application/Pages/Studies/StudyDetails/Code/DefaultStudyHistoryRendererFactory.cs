using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Studies.StudyDetails.Code
{
    /// <summary>
    /// Helper class used in rendering the information encoded in the ChangeDescription column
    /// of a StudyHistory record.
    /// </summary>
    internal class DefaultStudyHistoryRendererFactory : IStudyHistoryColumnRendererFactory
    {
        public Control GetChangeDescColumnControl(StudyHistory historyRecord)
        {
            Label lb = new Label();
            lb.Text = XmlUtils.GetXmlDocumentAsString(historyRecord.ChangeDescription, true);
            return lb;
        }
    }
}