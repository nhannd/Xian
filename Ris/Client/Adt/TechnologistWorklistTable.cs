using System;

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    class ModalityWorklistTable : DecoratedTable<ModalityWorklistItem>
    {
        private static readonly uint NumRows = 2;
        private static readonly uint ProcedureDescriptionRow = 1;

        public ModalityWorklistTable()
            : this(NumRows)
        {
        }

        private ModalityWorklistTable(uint cellRowCount)
            : base(cellRowCount)
        {
            this.Columns.Add(new TableColumn<ModalityWorklistItem, string>("MRN",
                delegate(ModalityWorklistItem item) { return string.Format("{0} {1}", item.MrnID, item.MrnAssigningAuthority); }, 
                0.5f));

            this.Columns.Add(new TableColumn<ModalityWorklistItem, string>("Name",
                delegate(ModalityWorklistItem item) { return string.Format("{0}, {1}", item.PersonNameDetail.FamilyName, item.PersonNameDetail.GivenName); },
                1.5f));

            this.Columns.Add(new TableColumn<ModalityWorklistItem, string>("Accession",
                delegate(ModalityWorklistItem item) { return item.AccessionNumber; },
                0.5f));

            TableColumn<ModalityWorklistItem, string> priorityColumn = new TableColumn<ModalityWorklistItem, string>("Priority",
                delegate(ModalityWorklistItem item) { return item.Priority.Value; },
                0.5f);
            priorityColumn.Visible = false;
            this.Columns.Add(priorityColumn);

            //this.Columns.Add(new TableColumn<ModalityWorklistItem, string>("Priority",
            //    delegate(ModalityWorklistItem item) { return item.Priority.Value; },
            //    0.5f));

            //this.Columns.Add(new SummaryTableColumn<ModalityWorklistItem, string>("Requested Procedure Step",
            //    delegate(ModalityWorklistItem item) { return item.RequestedProcedureStepName; },
            //    0.5f,
            //    1));

            //this.Columns.Add(new SummaryTableColumn<ModalityWorklistItem, string>("Modality Procedure Step",
            //    delegate(ModalityWorklistItem item) { return item.ModalityProcedureStepName; },
            //    0.5f,
            //    1));

            this.Columns.Add(new DecoratedTableColumn<ModalityWorklistItem, string>("Procedure Description",
                delegate(ModalityWorklistItem item) 
                { 
                    return string.Format("{0} - {1}", item.RequestedProcedureStepName, item.ModalityProcedureStepName); 
                },
                0.5f,
                ProcedureDescriptionRow));

            this.OutlineColorSelector = delegate(object o)
            {
                ModalityWorklistItem item = o as ModalityWorklistItem;
                if (item != null)
                {
                    //if (item.MrnID == "7001222")
                    if (item.Priority.Code != "R")
                    {
                        return "Gray";
                    }
                    else
                    {
                        return "Empty";
                    }
                }
                else
                {
                    return "Empty";
                }
            };

            this.BackgroundColorSelector = delegate(object o)
            {
                ModalityWorklistItem item = o as ModalityWorklistItem;
                if (item != null)
                {
                    switch (item.Priority.Code)
                    {
                        case "S":
                            return "Red";
                        case "A":
                            return "Yellow";
                        case "R":
                        default:
                            return "Empty";
                    }
                }
                else
                {
                    return "Empty";
                }
            };

            this.Sort(new TableSortParams(priorityColumn, true));
        }
    }
}
