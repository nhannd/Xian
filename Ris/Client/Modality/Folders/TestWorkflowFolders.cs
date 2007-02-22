using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Client.Modality.Folders
{
    public class ScheduledFolder : ModalityWorkflowFolder
    {
        public ScheduledFolder(ModalityWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Scheduled")
        {

        }

        protected override IList<WorklistQueryResult> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            criteria.State.EqualTo(ActivityStatus.SC);

            return this.WorkflowService.GetWorklist(criteria);
        }

        protected override bool IsMember(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.SC;
        }
    }

    public class InProgressFolder : ModalityWorkflowFolder
    {
        public InProgressFolder(ModalityWorkflowFolderSystem folderSystem)
            : base(folderSystem, "In Progress")
        {

        }

        protected override IList<WorklistQueryResult> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            criteria.State.EqualTo(ActivityStatus.IP);

            return this.WorkflowService.GetWorklist(criteria);
        }

        protected override bool IsMember(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.IP;
        }

        protected override bool CanAcceptDrop(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.SC;
        }

        protected override bool ConfirmAcceptDrop(ICollection<WorklistQueryResult> items)
        {
            DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to start these procedures?", MessageBoxActions.YesNo);
            return (result == DialogBoxAction.Yes);
        }

        protected override bool ProcessDrop(WorklistQueryResult item)
        {
            IModalityWorkflowService service = ApplicationContext.GetService<IModalityWorkflowService>();
            //service.StartProcedureStep(item.ProcedureStep);
            return true;
        }
    }

    public class CompletedFolder : ModalityWorkflowFolder
    {
        public CompletedFolder(ModalityWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Completed")
        {

        }

        protected override IList<WorklistQueryResult> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            criteria.State.EqualTo(ActivityStatus.CM);

            return this.WorkflowService.GetWorklist(criteria);
        }

        protected override bool IsMember(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.CM;
        }

        protected override bool CanAcceptDrop(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.IP;
        }

        protected override bool ConfirmAcceptDrop(ICollection<WorklistQueryResult> items)
        {
            DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to complete these procedures?", MessageBoxActions.YesNo);
            return (result == DialogBoxAction.Yes);
        }

        protected override bool ProcessDrop(WorklistQueryResult item)
        {
            IModalityWorkflowService service = ApplicationContext.GetService<IModalityWorkflowService>();
            //service.CompleteProcedureStep(item.ProcedureStep);
            return true;
        }
    }

    public class CancelledFolder : ModalityWorkflowFolder
    {
        public CancelledFolder(ModalityWorkflowFolderSystem folderSystem)
            : base(folderSystem, "Cancelled")
        {

        }

        protected override IList<WorklistQueryResult> QueryItems()
        {
            ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
            criteria.State.EqualTo(ActivityStatus.DC);

            return this.WorkflowService.GetWorklist(criteria);
        }

        protected override bool IsMember(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.DC;
        }

        protected override bool CanAcceptDrop(WorklistQueryResult item)
        {
            return item.Status == ActivityStatus.IP || item.Status == ActivityStatus.SC;
        }

        protected override bool ConfirmAcceptDrop(ICollection<WorklistQueryResult> items)
        {
            DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to cancel these procedures?", MessageBoxActions.YesNo);
            return (result == DialogBoxAction.Yes);
        }

        protected override bool ProcessDrop(WorklistQueryResult item)
        {
            IModalityWorkflowService service = ApplicationContext.GetService<IModalityWorkflowService>();
            //service.CancelProcedureStep(item.ProcedureStep);
            return true;
        }
    }
}
