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
        private readonly ProtocolEditorMode _mode;

        #endregion

        #region Constructor

        public ProtocolEditorComponentDocument(ReportingWorklistItem item, ProtocolEditorMode mode, IReportingWorkflowItemToolContext context)
            : base(null, context.DesktopWindow)
        {
            _item = item;
            _mode = mode;
        }

        #endregion

        #region Document overrides

        public override string GetTitle()
        {
            return ProtocolEditorComponentDocument.GetTitle(_item);
        }

        public override IApplicationComponent GetComponent()
        {
            return new ProtocolEditorComponent(_item, _mode);
        }

        #endregion

        public static string GetTitle(ReportingWorklistItem item)
        {
            return string.Format("A# {0} - {1}, {2}", item.AccessionNumber, item.PatientName.FamilyName, item.PatientName.GivenName);
        }
    }
}
