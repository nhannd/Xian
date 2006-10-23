using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.HL7;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    public class HL7QueueItemTableData : Table<HL7QueueItem>
    {
        public HL7QueueItemTableData(IHL7ServiceLayer service)
        {
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>("Direction",
                    delegate(HL7QueueItem item) { return service.GetHL7MessageDirectionEnumTable()[item.Direction].Value; }, 0.5f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>("Peer",
                    delegate(HL7QueueItem item) { return service.GetHL7MessagePeerEnumTable()[item.Message.Peer].Value; }, 0.5f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>("Type",
                    delegate(HL7QueueItem item) { return item.Message.MessageType; }, 0.5f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>("Status",
                    delegate(HL7QueueItem item) { return service.GetHL7MessageStatusCodeEnumTable()[item.Status.Code].Value; }, 0.5f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>("Status Description",
                    delegate(HL7QueueItem item) { return item.Status.Description; }, 0.5f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>("Created On",
                    delegate(HL7QueueItem item) { return ClearCanvas.Desktop.Format.DateTime(item.Status.CreationDateTime); }, 0.5f));
            this.Columns.Add(
                new TableColumn<HL7QueueItem, string>("Updated On",
                    delegate(HL7QueueItem item) { return ClearCanvas.Desktop.Format.DateTime(item.Status.UpdateDateTime); }, 0.5f));
        }
    }
}
