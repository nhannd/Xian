using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    public class HL7QueueItemTableData : Table<HL7QueueItem>
    {
        public HL7QueueItemTableData(IHL7ServiceLayer service)
        {
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>(SR.ColumnDirection,
                    delegate(HL7QueueItem item) { return service.GetHL7MessageDirectionEnumTable()[item.Direction].Value; }, 0.75f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>(SR.ColumnPeer,
                    delegate(HL7QueueItem item) { return service.GetHL7MessagePeerEnumTable()[item.Message.Peer].Value; }, 0.90f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>(SR.ColumnType,
                    delegate(HL7QueueItem item) { return item.Message.MessageType; }, 0.75f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>(SR.ColumnStatus,
                    delegate(HL7QueueItem item) { return service.GetHL7MessageStatusCodeEnumTable()[item.Status.Code].Value; }, 0.75f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>(SR.ColumnStatusDescription,
                    delegate(HL7QueueItem item) { return item.Status.Description; }, 3.0f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>(SR.ColumnCreatedOn,
                    delegate(HL7QueueItem item) { return ClearCanvas.Desktop.Format.DateTime(item.Status.CreationDateTime); }, 1.5f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>(SR.ColumnUpdatedOn,
                    delegate(HL7QueueItem item) { return ClearCanvas.Desktop.Format.DateTime(item.Status.UpdateDateTime); }, 1.5f));
        }
    }
}
