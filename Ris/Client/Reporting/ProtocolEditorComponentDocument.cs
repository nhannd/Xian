using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Document container for <see cref="ProtocolEditorComponent"/>
    /// </summary>
    class ProtocolEditorComponentDocument : Document
    {
        #region Private Members

        private readonly ReportingWorklistItem _item;

        #endregion

        #region Constructor

        public ProtocolEditorComponentDocument(ReportingWorklistItem item, IReportingWorkflowItemToolContext context)
            : base(item.OrderRef, context.DesktopWindow)
        {
            _item = item;
        }

        #endregion

        #region Document overrides

        public override string GetTitle()
        {
            return ProtocolEditorComponentDocument.GetTitle(_item);
        }

        public override IApplicationComponent GetComponent()
        {
            return new ProtocolEditorComponent(_item);
        }

        #endregion

        public static string GetTitle(ReportingWorklistItem item)
        {
            return string.Format("A# {0} - {1}, {2}", item.AccessionNumber, item.PatientName.FamilyName, item.PatientName.GivenName);
        }
    }
}
