using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Modality
{
    public abstract class ModalityWorkflowFolder : WorkflowFolder<WorklistQueryResult>
    {
        private ModalityWorkflowFolderSystem _folderSystem;

        public ModalityWorkflowFolder(ModalityWorkflowFolderSystem folderSystem, string folderName)
            :base(folderSystem, folderName, new ModalityWorklistTable(folderSystem.WorkflowService.GetOrderPriorityEnumTable()))
        {
            _folderSystem = folderSystem;
        }

        protected IModalityWorkflowService WorkflowService
        {
            get { return _folderSystem.WorkflowService; }
        }

        protected override bool CanAcceptDrop(WorklistQueryResult item)
        {
            return false;
        }

        protected override bool ConfirmAcceptDrop(ICollection<WorklistQueryResult> items)
        {
            return false;
        }

        protected override bool ProcessDrop(WorklistQueryResult item)
        {
            return false;
        }
    }
}
