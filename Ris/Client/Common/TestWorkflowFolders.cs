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

namespace ClearCanvas.Ris.Client.Common
{
    public class WorklistItemTable : Table<AcquisitionWorklistItem>
    {
        public WorklistItemTable()
        {
            IOrderEntryService orderEntryService = ApplicationContext.GetService<IOrderEntryService>();
            OrderPriorityEnumTable orderPriorities = orderEntryService.GetOrderPriorityEnumTable();

            this.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnMRN,
                delegate(AcquisitionWorklistItem item) { return item.Mrn.Format(); }));
            this.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnName,
                delegate(AcquisitionWorklistItem item) { return item.PatientName.Format(); }));
            //this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnVisitNumber,
            //    delegate(WorklistItem item) { return item.VisitNumber.Format(); }));
            this.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnAccessionNumber,
                delegate(AcquisitionWorklistItem item) { return item.AccessionNumber; }));
            //this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnDiagnosticService,
            //    delegate(WorklistItem item) { return item.DiagnosticService; }));
            //this.Columns.Add(new TableColumn<WorklistItem, string>(SR.ColumnProcedure,
            //    delegate(WorklistItem item) { return item.Procedure; }));
            this.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnScheduledStep,
                delegate(AcquisitionWorklistItem item) { return item.ScheduledStep; }));
            this.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnModality,
                delegate(AcquisitionWorklistItem item) { return item.Modality; }));
            this.Columns.Add(new TableColumn<AcquisitionWorklistItem, string>(SR.ColumnPriority,
                delegate(AcquisitionWorklistItem item) { return orderPriorities[item.Priority].Value; }));
        }
    }

    class TestWorkflowFolders
    {
        public class ScheduledItemsFolder : WorkflowFolder<AcquisitionWorklistItem>
        {
            public ScheduledItemsFolder()
                : base("Scheduled", new WorklistItemTable())
            {

            }

            protected override IList<AcquisitionWorklistItem> QueryItems()
            {
                ScheduledProcedureStepSearchCriteria criteria = new ScheduledProcedureStepSearchCriteria();
                criteria.Status.EqualTo(ScheduledProcedureStepStatus.SCHEDULED);

                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                return service.GetWorklist(criteria);
            }

            protected override bool CanAcceptDrop(AcquisitionWorklistItem item)
            {
                return false;
            }

            protected override bool ConfirmAcceptDrop(ICollection<AcquisitionWorklistItem> items)
            {
                return false;
            }

            protected override bool ProcessDrop(AcquisitionWorklistItem item)
            {
                return false;
            }
        }

        public class InProgressItemsFolder : WorkflowFolder<AcquisitionWorklistItem>
        {
            public InProgressItemsFolder()
                : base("In Progress", new WorklistItemTable())
            {

            }

            protected override IList<AcquisitionWorklistItem> QueryItems()
            {
                ScheduledProcedureStepSearchCriteria criteria = new ScheduledProcedureStepSearchCriteria();
                criteria.Status.EqualTo(ScheduledProcedureStepStatus.INPROGRESS);

                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                return service.GetWorklist(criteria);
            }

            protected override bool CanAcceptDrop(AcquisitionWorklistItem item)
            {
                return item.Status == ScheduledProcedureStepStatus.SCHEDULED;
            }

            protected override bool ConfirmAcceptDrop(ICollection<AcquisitionWorklistItem> items)
            {
                DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to start these procedures?", MessageBoxActions.YesNo);
                return (result == DialogBoxAction.Yes);
            }

            protected override bool ProcessDrop(AcquisitionWorklistItem item)
            {
                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                service.StartProcedureStep(item.WorkflowStep);
                return true;
            }
        }

        public class CompletedItemsFolder : WorkflowFolder<AcquisitionWorklistItem>
        {
            public CompletedItemsFolder()
                : base("Completed", new WorklistItemTable())
            {

            }

            protected override IList<AcquisitionWorklistItem> QueryItems()
            {
                ScheduledProcedureStepSearchCriteria criteria = new ScheduledProcedureStepSearchCriteria();
                criteria.Status.EqualTo(ScheduledProcedureStepStatus.COMPLETED);

                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                return service.GetWorklist(criteria);
            }

            protected override bool CanAcceptDrop(AcquisitionWorklistItem item)
            {
                return item.Status == ScheduledProcedureStepStatus.INPROGRESS;
            }

            protected override bool ConfirmAcceptDrop(ICollection<AcquisitionWorklistItem> items)
            {
                DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to complete these procedures?", MessageBoxActions.YesNo);
                return (result == DialogBoxAction.Yes);
            }

            protected override bool ProcessDrop(AcquisitionWorklistItem item)
            {
                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                service.CompleteProcedureStep(item.WorkflowStep);
                return true;
            }
        }

        public class CancelledItemsFolder : WorkflowFolder<AcquisitionWorklistItem>
        {
            public CancelledItemsFolder()
                : base("Cancelled", new WorklistItemTable())
            {

            }

            protected override IList<AcquisitionWorklistItem> QueryItems()
            {
                ScheduledProcedureStepSearchCriteria criteria = new ScheduledProcedureStepSearchCriteria();
                criteria.Status.EqualTo(ScheduledProcedureStepStatus.DISCONTINUED);

                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                return service.GetWorklist(criteria);
            }

            protected override bool CanAcceptDrop(AcquisitionWorklistItem item)
            {
                return item.Status == ScheduledProcedureStepStatus.INPROGRESS || item.Status == ScheduledProcedureStepStatus.SCHEDULED;
            }

            protected override bool ConfirmAcceptDrop(ICollection<AcquisitionWorklistItem> items)
            {
                DialogBoxAction result = Platform.ShowMessageBox("Are you sure you want to cancel these procedures?", MessageBoxActions.YesNo);
                return (result == DialogBoxAction.Yes);
            }

            protected override bool ProcessDrop(AcquisitionWorklistItem item)
            {
                IAcquisitionWorkflowService service = ApplicationContext.GetService<IAcquisitionWorkflowService>();
                service.CancelProcedureStep(item.WorkflowStep);
                return true;
            }
        }


    }
}
