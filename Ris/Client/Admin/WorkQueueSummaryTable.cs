#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
	public class WorkQueueSummaryTable : Table<WorkQueueItemSummary>
	{
		public WorkQueueSummaryTable()
		{
			this.Columns.Add(new DateTimeTableColumn<WorkQueueItemSummary>(SR.ColumnCreationTime, item => item.CreationTime));
			this.Columns.Add(new DateTimeTableColumn<WorkQueueItemSummary>(SR.ColumnScheduledTime, item => item.ScheduledTime));
			//this.Columns.Add(new DateTimeTableColumn<WorkQueueItemSummary>(SR.ColumnExpirationTime, item => item.ExpirationTime));
			//this.Columns.Add(new TableColumn<WorkQueueItemSummary, string>(SR.ColumnUser, item => item.User));
			this.Columns.Add(new TableColumn<WorkQueueItemSummary, string>(SR.ColumnType, item => item.Type));
			this.Columns.Add(new TableColumn<WorkQueueItemSummary, string>(SR.ColumnStatus, item => item.Status.Value));
			this.Columns.Add(new DateTimeTableColumn<WorkQueueItemSummary>(SR.ColumnProcessedTime, item => item.ProcessedTime));
			//this.Columns.Add(new TableColumn<WorkQueueItemSummary, int>(SR.ColumnFailureCount, item => item.FailureCount));
			//this.Columns.Add(new TableColumn<WorkQueueItemSummary, string>(SR.ColumnFailureDescription, item => item.FailureDescription));
		}
	}
}