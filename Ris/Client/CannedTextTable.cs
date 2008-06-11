using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.CannedTextService;

namespace ClearCanvas.Ris.Client
{
	public class CannedTextTable : Table<CannedTextSummary>
	{
		public CannedTextTable()
		{
			this.Columns.Add(new TableColumn<CannedTextSummary, string>(SR.ColumnName,
				delegate(CannedTextSummary c) { return c.Name; },
				0.5f));
			this.Columns.Add(new TableColumn<CannedTextSummary, string>(SR.ColumnCategory,
				delegate(CannedTextSummary c) { return c.Path; },
				0.5f));
			this.Columns.Add(new TableColumn<CannedTextSummary, string>(SR.ColumnText,
				delegate(CannedTextSummary c) { return c.Text; },
				1.0f));
			this.Columns.Add(new TableColumn<CannedTextSummary, string>(SR.ColumnGroup,
				delegate(CannedTextSummary item) { return item.IsPersonal ? SR.ColumnPersonal : item.StaffGroup.Name; }, 1.0f));
		}
	}
}
