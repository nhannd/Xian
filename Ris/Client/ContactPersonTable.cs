using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client
{
    class ContactPersonTable : Table<ContactPersonDetail>
    {
        public ContactPersonTable()
        {
            this.Columns.Add(new TableColumn<ContactPersonDetail, string>(SR.ColumnContactType,
                delegate(ContactPersonDetail c) { return c.Type.Value; },
                1.0f));
            this.Columns.Add(new TableColumn<ContactPersonDetail, string>(SR.ColumnName,
                delegate(ContactPersonDetail c) { return c.Name; },
                3.0f));
        }
    }
}
