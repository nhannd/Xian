#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	public class AttachmentSummaryTable : Table<AttachmentSummary>
	{
		public AttachmentSummaryTable()
		{
			var receivedDateColumn = new DateTableColumn<AttachmentSummary>(
				SR.ColumnReceivedDate,
				summary => summary.Document.ReceivedTime,
				0.2f);
			this.Columns.Add(receivedDateColumn);
			this.Columns.Add(new TableColumn<AttachmentSummary, string>(
								SR.ColumnCategory,
								summary => summary.Category.Value,
								0.2f));
			this.Columns.Add(new TableColumn<AttachmentSummary, string>(
								SR.ColumnAttachedBy,
								summary => summary.AttachedBy == null ? "me" : PersonNameFormat.Format(summary.AttachedBy.Name),
								0.2f));
			this.Columns.Add(new TableColumn<AttachmentSummary, string>(
								SR.ColumnAttachmentType,
								summary => summary.Document.DocumentTypeName,
								0.2f));

			this.Sort(new TableSortParams(receivedDateColumn, false));
		}
	}
}