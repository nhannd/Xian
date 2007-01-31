using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Client.Common;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Modality
{
    public abstract class ModalityWorkflowFolder : WorkflowFolder<ModalityWorklistQueryResult>
    {
        private ModalityWorkflowFolderSystem _folderSystem;

        public ModalityWorkflowFolder(ModalityWorkflowFolderSystem folderSystem, string folderName)
            :base(folderSystem, folderName, new ModalityWorklistTable(folderSystem.WorkflowService.GetOrderPriorityEnumTable()))
        {
            _folderSystem = folderSystem;
        }

        protected IAcquisitionWorkflowService WorkflowService
        {
            get { return _folderSystem.WorkflowService; }
        }

        protected override bool CanAcceptDrop(ModalityWorklistQueryResult item)
        {
            return false;
        }

        protected override bool ConfirmAcceptDrop(ICollection<ModalityWorklistQueryResult> items)
        {
            return false;
        }

        protected override bool ProcessDrop(ModalityWorklistQueryResult item)
        {
            return false;
        }
    }
}
