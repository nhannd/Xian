using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public class TelephoneNumberTable : Table<TelephoneDetail>
    {
        public TelephoneNumberTable()
        {
            this.Columns.Add(new TableColumn<TelephoneDetail, string>(SR.ColumnType,
                delegate(TelephoneDetail t) { return t.Type.Value; }, 
                1.1f));
            this.Columns.Add(new TableColumn<TelephoneDetail, string>(SR.ColumnNumber,
                delegate(TelephoneDetail pn) { return Format.Custom(pn); },
                2.2f));
            this.Columns.Add(new TableColumn<TelephoneDetail, string>(SR.ColumnExpiryDate,
                delegate(TelephoneDetail pn) { return Format.Date(pn.ValidRangeUntil); }, 
                0.9f));
        }
    }
}
