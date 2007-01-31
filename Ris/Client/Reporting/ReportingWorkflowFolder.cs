using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Reporting
{
    /*
    public abstract class ReportingWorkflowFolder : WorkflowFolder<ReportingWorklistQueryResult>
    {
        public ReportingWorkflowFolder(ModalityWorkflowFolderSystem folderSystem, string folderName)
            :base(folderSystem, folderName, new ReportingWorklistTable(folderSystem.WorkflowService.GetOrderPriorityEnumTable()))
        {
        }

        protected override bool CanAcceptDrop(ReportingWorklistQueryResult item)
        {
            return false;
        }

        protected override bool ConfirmAcceptDrop(ICollection<ReportingWorklistQueryResult> items)
        {
            return false;
        }

        protected override bool ProcessDrop(ReportingWorklistQueryResult item)
        {
            return false;
        }
    }
     */
}
