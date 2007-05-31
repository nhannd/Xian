using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting.Folders
{
    public class ScheduledInterpretationFolder : ReportingWorkflowFolder
    {
        public ScheduledInterpretationFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Scheduled Interpretation")
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+ScheduledInterpretation";
        }
    }

    public class MyInterpretationFolder : ReportingWorkflowFolder
    {
        public MyInterpretationFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "My Interpretation")
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+MyInterpretation";
        }
    }

    public class MyTranscriptionFolder : ReportingWorkflowFolder
    {
        public MyTranscriptionFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "My Transcription")
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+MyTranscription";
        }
    }

    public class MyVerificationFolder : ReportingWorkflowFolder
    {
        public MyVerificationFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "My Verification")
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+MyVerification";
        }
    }

    public class MyVerifiedFolder : ReportingWorkflowFolder
    {
        public MyVerifiedFolder(ReportingWorkflowFolderSystem folderSystem)
            : base(folderSystem, "My Verified")
        {
            this.WorklistClassName = "ClearCanvas.Healthcare.Workflow.Reporting.Worklists+MyVerified";
        }
    }
}
