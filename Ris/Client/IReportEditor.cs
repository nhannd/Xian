using System;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public interface IReportEditor : IApplicationComponent
    {
        ReportingWorklistItem WorklistItem { set; }
        ReportDetail Report { set; }
        ReportPartDetail ReportPart { set; }
        StaffSummary Supervisor { get; set; }
        string ReportContent { get; }

        bool IsEditingAddendum { set; }
        bool VerifyEnabled { set; }
        bool SendToVerifyEnabled { set; }
        bool SendToTranscriptionEnabled { set; }

        event EventHandler VerifyRequested;
        event EventHandler SendToVerifyRequested;
        event EventHandler SendToTranscriptionRequested;
        event EventHandler SaveRequested;
        event EventHandler CancelRequested;
    }

    [ExtensionPoint]
    public class ReportEditorExtensionPoint : ExtensionPoint<IReportEditor>
    {
    }
}
