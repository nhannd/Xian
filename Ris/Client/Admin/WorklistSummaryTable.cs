using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    internal class WorklistSummaryTable : Table<WorklistSummary>
    {
        public WorklistSummaryTable()
        {
            this.Columns.Add(new TableColumn<WorklistSummary, string>("Name",
                delegate(WorklistSummary summary) { return summary.Name; },
                0.5f));

            this.Columns.Add(new TableColumn<WorklistSummary, string>("Description",
                delegate (WorklistSummary summary) { return summary.Description; },
                0.5f));

            this.Columns.Add(new TableColumn<WorklistSummary, string>("Type",
                delegate(WorklistSummary summary) { return summary.WorklistType; },
                0.5f));
        }
    }
}