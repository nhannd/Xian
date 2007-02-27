using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Common
{
    class ContactPersonTable : Table<ContactPerson>
    {
        public ContactPersonTable()
        {
            IAdtService _adtService = ApplicationContext.GetService<IAdtService>();
            ContactPersonTypeEnumTable _contactTypes = _adtService.GetContactPersonTypeEnumTable();

            this.Columns.Add(new TableColumn<ContactPerson, string>(SR.ColumnContactType,
                delegate(ContactPerson c) { return _contactTypes[c.Type].Value; },
                1.0f));
            this.Columns.Add(new TableColumn<ContactPerson, string>(SR.ColumnName,
                delegate(ContactPerson c) { return c.Name; },
                3.0f));
        }
    }
}
