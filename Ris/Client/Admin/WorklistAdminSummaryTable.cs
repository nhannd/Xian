using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    internal class WorklistAdminSummaryTable : Table<WorklistAdminSummary>
    {
        public WorklistAdminSummaryTable()
        {
            this.Columns.Add(new TableColumn<WorklistAdminSummary, string>("Name",
                delegate(WorklistAdminSummary summary) { return summary.Name; },
                0.5f));

            this.Columns.Add(new TableColumn<WorklistAdminSummary, string>("Type",
                delegate(WorklistAdminSummary summary) { return summary.WorklistType; },
                0.5f));

            this.Columns.Add(new TableColumn<WorklistAdminSummary, string>("Description",
                delegate(WorklistAdminSummary summary) { return summary.Description; },
                1.5f));
        }
    }
}