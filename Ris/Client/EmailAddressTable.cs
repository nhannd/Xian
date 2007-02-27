using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client
{
    public class EmailAddressTable : Table<EmailAddress>
    {
        public EmailAddressTable()
        {
            this.Columns.Add(new TableColumn<EmailAddress, string>(SR.ColumnAddress,
                delegate(EmailAddress ea) { return ea.Address; },
                2.2f));
            this.Columns.Add(new TableColumn<EmailAddress, string>(SR.ColumnExpiryDate, 
                delegate(EmailAddress ea) { return ea.ValidRange == null ? null : Format.Date(ea.ValidRange.Until); },
                0.9f));
        }
    }
}
