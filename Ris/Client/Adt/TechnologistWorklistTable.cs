using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Client.Formatting;

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
            this.Columns.Add(new TableColumn<ModalityWorklistItem, string>(SR.ColumnMRN,
                delegate(ModalityWorklistItem item) { return MrnFormat.Format(item.Mrn); }, 
                0.5f));

            this.Columns.Add(new TableColumn<ModalityWorklistItem, string>(SR.ColumnName,
                delegate(ModalityWorklistItem item) { return PersonNameFormat.Format(item.PersonNameDetail); },
                1.5f));

            this.Columns.Add(new TableColumn<ModalityWorklistItem, string>(SR.ColumnAccessionNumber,
                delegate(ModalityWorklistItem item) { return item.AccessionNumber; },
                0.5f));

            TableColumn<ModalityWorklistItem, string> priorityColumn = new TableColumn<ModalityWorklistItem, string>(SR.ColumnPriority,
                delegate(ModalityWorklistItem item) { return item.Priority.Value; },
                0.5f);
            priorityColumn.Visible = false;
            this.Columns.Add(priorityColumn);

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

            //this.BackgroundColorSelector = delegate(object o)
            //{
            //    ModalityWorklistItem item = o as ModalityWorklistItem;
            //    if (item != null)
            //    {
            //        switch (item.Priority.Code)
            //        {
            //            case "S":
            //                return "Red";
            //            case "A":
            //                return "Yellow";
            //            case "R":
            //            default:
            //                return "Empty";
            //        }
            //    }
            //    else
            //    {
            //        return "Empty";
            //    }
            //};
        }
    }
}
