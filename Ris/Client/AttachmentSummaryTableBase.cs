using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public abstract class AttachmentSummaryTableBase<TAttachmentSummary> : Table<TAttachmentSummary>
		where TAttachmentSummary : AttachmentSummary
	{
		protected AttachmentSummaryTableBase()
		{
			var receivedDateColumn = new DateTableColumn<TAttachmentSummary>(
				SR.ColumnReceivedDate,
				summary => summary.Document.ReceivedTime,
				0.2f);
			this.Columns.Add(receivedDateColumn);
			this.Columns.Add(new TableColumn<TAttachmentSummary, string>(
								SR.ColumnCategory,
								summary => summary.Category.Value,
								0.2f));
			this.Columns.Add(new TableColumn<TAttachmentSummary, string>(
								SR.ColumnAttachedBy,
								summary => summary.AttachedBy == null ? "me" : PersonNameFormat.Format(summary.AttachedBy.Name),
								0.2f));
			this.Columns.Add(new TableColumn<TAttachmentSummary, string>(
								SR.ColumnAttachmentType,
								summary => summary.Document.DocumentTypeName,
								0.2f));

			this.Sort(new TableSortParams(receivedDateColumn, false));
		}
	}
}