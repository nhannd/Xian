using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    class FacilityTable : Table<Facility>
    {
        public FacilityTable()
        {
            this.Columns.Add(new TableColumn<Facility, string>(SR.ColumnName,
                delegate(Facility f) { return f.Name; },
                1.0f));
        }
    }
}
