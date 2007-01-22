using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Client.Common
{
    public class WorklistItemTable : Table<ModalityWorklistQueryResult>
    {
        public WorklistItemTable()
        {
            IOrderEntryService orderEntryService = ApplicationContext.GetService<IOrderEntryService>();
            OrderPriorityEnumTable orderPriorities = orderEntryService.GetOrderPriorityEnumTable();

            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnMRN,
                delegate(ModalityWorklistQueryResult item) { return Format.Custom(item.Mrn); }));
            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnName,
                delegate(ModalityWorklistQueryResult item) { return Format.Custom(item.PatientName); }));
            //this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnVisitNumber,
            //    delegate(WorklistItem item) { return item.VisitNumber.Format(); }));
            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnAccessionNumber,
                delegate(ModalityWorklistQueryResult item) { return item.AccessionNumber; }));
            //this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnDiagnosticService,
            //    delegate(WorklistItem item) { return item.DiagnosticService; }));
            //this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnProcedure,
            //    delegate(WorklistItem item) { return item.Procedure; }));
            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnScheduledStep,
                delegate(ModalityWorklistQueryResult item) { return item.ModalityProcedureStepName; }));
            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnModality,
                delegate(ModalityWorklistQueryResult item) { return item.ModalityName; }));
            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>(SR.ColumnPriority,
                delegate(ModalityWorklistQueryResult item) { return orderPriorities[item.Priority].Value; }));
        }
    }

    class TestWorkflowFolders
    {
        public class ScheduledItemsFolder : WorkflowFolder<ModalityWorklistQueryResult>
        {
            public ScheduledItemsFolder()
                : base("Scheduled", new WorklistItemTable())
            {

            }

            protected override IList<ModalityWorklistQueryResult> QueryItems()
            {
                ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
                criteria.Status.EqualTo(ActivityStatus.SC);

                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                return service.GetWorklist(criteria);
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

        public class InProgressItemsFolder : WorkflowFolder<ModalityWorklistQueryResult>
        {
            public InProgressItemsFolder()
                : base("In Progress", new WorklistItemTable())
            {

            }

            protected override IList<ModalityWorklistQueryResult> QueryItems()
            {
                ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
                criteria.Status.EqualTo(ActivityStatus.IP);

                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                return service.GetWorklist(criteria);
            }

            protected override bool CanAcceptDrop(ModalityWorklistQueryResult item)
            {
                return item.Status == ActivityStatus.SC;
            }

            protected override bool ConfirmAcceptDrop(ICollection<ModalityWorklistQueryResult> items)
            {
                DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to start these procedures?", MessageBoxActions.YesNo);
                return (result == DialogBoxAction.Yes);
            }

            protected override bool ProcessDrop(ModalityWorklistQueryResult item)
            {
                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                service.StartProcedureStep(item.WorkflowStep);
                return true;
            }
        }

        public class CompletedItemsFolder : WorkflowFolder<ModalityWorklistQueryResult>
        {
            public CompletedItemsFolder()
                : base("Completed", new WorklistItemTable())
            {

            }

            protected override IList<ModalityWorklistQueryResult> QueryItems()
            {
                ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
                criteria.Status.EqualTo(ActivityStatus.CM);

                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                return service.GetWorklist(criteria);
            }

            protected override bool CanAcceptDrop(ModalityWorklistQueryResult item)
            {
                return item.Status == ActivityStatus.IP;
            }

            protected override bool ConfirmAcceptDrop(ICollection<ModalityWorklistQueryResult> items)
            {
                DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to complete these procedures?", MessageBoxActions.YesNo);
                return (result == DialogBoxAction.Yes);
            }

            protected override bool ProcessDrop(ModalityWorklistQueryResult item)
            {
                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                service.CompleteProcedureStep(item.WorkflowStep);
                return true;
            }
        }

        public class CancelledItemsFolder : WorkflowFolder<ModalityWorklistQueryResult>
        {
            public CancelledItemsFolder()
                : base("Cancelled", new WorklistItemTable())
            {

            }

            protected override IList<ModalityWorklistQueryResult> QueryItems()
            {
                ModalityProcedureStepSearchCriteria criteria = new ModalityProcedureStepSearchCriteria();
                criteria.Status.EqualTo(ActivityStatus.DC);

                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                return service.GetWorklist(criteria);
            }

            protected override bool CanAcceptDrop(ModalityWorklistQueryResult item)
            {
                return item.Status == ActivityStatus.IP || item.Status == ActivityStatus.SC;
            }

            protected override bool ConfirmAcceptDrop(ICollection<ModalityWorklistQueryResult> items)
            {
                DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to cancel these procedures?", MessageBoxActions.YesNo);
                return (result == DialogBoxAction.Yes);
            }

            protected override bool ProcessDrop(ModalityWorklistQueryResult item)
            {
                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                service.CancelProcedureStep(item.WorkflowStep);
                return true;
            }
        }


    }
}
