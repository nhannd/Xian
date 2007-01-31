using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Modality
{
    public class ModalityWorklistTable : Table<ModalityWorklistQueryResult>
    {
        public ModalityWorklistTable(OrderPriorityEnumTable orderPriorities)
        {
            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>("MRN",
                delegate(ModalityWorklistQueryResult item) { return Format.Custom(item.Mrn); }));
            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>("Name",
                delegate(ModalityWorklistQueryResult item) { return Format.Custom(item.PatientName); }));
            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>("Accession #",
                delegate(ModalityWorklistQueryResult item) { return item.AccessionNumber; }));
            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>("Procedure Step",
                delegate(ModalityWorklistQueryResult item) { return item.ModalityProcedureStepName; }));
            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>("Modality",
                delegate(ModalityWorklistQueryResult item) { return item.ModalityName; }));
            this.Columns.Add(new TableColumn<ModalityWorklistQueryResult, string>("Priority",
                delegate(ModalityWorklistQueryResult item) { return orderPriorities[item.Priority].Value; }));
        }
   }
}
