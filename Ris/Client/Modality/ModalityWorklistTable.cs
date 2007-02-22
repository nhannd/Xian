using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Modality
{
    public class ModalityWorklistTable : Table<WorklistQueryResult>
    {
        public ModalityWorklistTable(OrderPriorityEnumTable orderPriorities)
        {
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("MRN",
                delegate(WorklistQueryResult item) { return Format.Custom(item.Mrn); }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Name",
                delegate(WorklistQueryResult item) { return Format.Custom(item.PatientName); }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Accession #",
                delegate(WorklistQueryResult item) { return item.AccessionNumber; }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Procedure Step",
                delegate(WorklistQueryResult item) { return item.ModalityProcedureStepName; }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Modality",
                delegate(WorklistQueryResult item) { return item.ModalityName; }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Priority",
                delegate(WorklistQueryResult item) { return orderPriorities[item.Priority].Value; }));
        }
   }
}
