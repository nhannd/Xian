using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public class EmailAddressTable : Table<EmailAddressDetail>
    {
        public EmailAddressTable()
        {
            this.Columns.Add(new TableColumn<EmailAddressDetail, string>(SR.ColumnAddress,
                delegate(EmailAddressDetail ea) { return ea.Address; },
                2.2f));
            this.Columns.Add(new TableColumn<EmailAddressDetail, string>(SR.ColumnExpiryDate,
                delegate(EmailAddressDetail ea) { return Format.Date(ea.ValidRangeUntil); },
                0.9f));
        }
    }
}
