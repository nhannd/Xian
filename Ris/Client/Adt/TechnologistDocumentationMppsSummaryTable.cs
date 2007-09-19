using System;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow.TechnologistDocumentation;

namespace ClearCanvas.Ris.Client.Adt
{
    public class TechnologistDocumentationMppsSummaryTable : Table<ModalityPerformedProcedureStepSummary>
    {
        public TechnologistDocumentationMppsSummaryTable()
        {
            this.Columns.Add(new TableColumn<ModalityPerformedProcedureStepSummary, string>(
                                 "Name",
                                 delegate(ModalityPerformedProcedureStepSummary mpps) { return mpps.InheritedName; },
                                 5.0f));

            this.Columns.Add(new TableColumn<ModalityPerformedProcedureStepSummary, string>(
                                 "State",
                                 delegate(ModalityPerformedProcedureStepSummary mpps) { return mpps.State.Value; },
                                 1.2f));

            this.Columns.Add(new TableColumn<ModalityPerformedProcedureStepSummary, DateTime>(
                                 "Start Time",
                                 delegate(ModalityPerformedProcedureStepSummary mpps) { return mpps.StartTime; },
                                 1.5f));

            this.Columns.Add(new TableColumn<ModalityPerformedProcedureStepSummary, DateTime?>(
                                 "End Time",
                                 delegate(ModalityPerformedProcedureStepSummary mpps) { return mpps.EndTime; },
                                 1.5f));
        }
    }
}