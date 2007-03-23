using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Adt
{
    public class HL7QueueItemSummaryTable : Table<HL7QueueItemSummary>
    {
        public HL7QueueItemSummaryTable()
        {
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnDirection,
                    delegate(HL7QueueItemSummary item) { return item.Direction; }, 0.75f));
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnPeer,
                    delegate(HL7QueueItemSummary item) { return item.Peer; }, 0.90f));
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnType,
                    delegate(HL7QueueItemSummary item) { return item.MessageType; }, 0.75f));
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnStatus,
                    delegate(HL7QueueItemSummary item) { return item.StatusCode; }, 0.75f));
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnStatusDescription,
                    delegate(HL7QueueItemSummary item) { return item.StatusDescription; }, 3.0f));
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnCreatedOn,
                    delegate(HL7QueueItemSummary item) { return Format.Date(item.CreationDateTime); }, 1.5f));
            this.Columns.Add(
                new TableColumn<HL7QueueItemSummary, string>(SR.ColumnUpdatedOn,
                    delegate(HL7QueueItemSummary item) { return Format.DateTime(item.UpdateDateTime); }, 1.5f));
        }
    }
}
