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

namespace ClearCanvas.Ris.Client
{
    public class EmailAddressTable : Table<EmailAddressDetail>
    {
        public EmailAddressTable()
        {
            this.Columns.Add(new TableColumn<EmailAddressDetail, string>(SR.ColumnAddress,
                delegate(EmailAddressDetail ea) { return ea.Address; },
                2.2f));
            this.Columns.Add(new DateTableColumn<EmailAddressDetail>(SR.ColumnExpiryDate,
                delegate(EmailAddressDetail ea) { return ea.ValidRangeUntil; },
                0.9f));
        }
    }
}
