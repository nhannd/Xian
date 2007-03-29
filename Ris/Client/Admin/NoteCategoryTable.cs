using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client.Admin
{
    class NoteCategoryTable : Table<NoteCategorySummary>
    {
        public NoteCategoryTable()
        {
            this.Columns.Add(new TableColumn<NoteCategorySummary, string>(SR.ColumnSeverity,
                delegate(NoteCategorySummary category) { return category.Severity.Value; },
                0.2f));

            this.Columns.Add(new TableColumn<NoteCategorySummary, string>(SR.ColumnName,
                delegate(NoteCategorySummary category) { return category.Name; },
                0.5f));

            this.Columns.Add(new TableColumn<NoteCategorySummary, string>(SR.ColumnDescription,
                delegate(NoteCategorySummary category) { return category.Description; },
                1.0f));
        }
    }
}
