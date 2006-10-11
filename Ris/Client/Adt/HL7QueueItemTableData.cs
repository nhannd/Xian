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
                new TableColumn<HL7QueueItem, string>("Type",
                    delegate(HL7QueueItem item) { return item.Message.MessageType; }, 0.5f));

        }
    }
}
