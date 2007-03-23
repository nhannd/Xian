using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Adt
{
    public class VisitLocationTable : Table<VisitLocationDetail>
    {
        public VisitLocationTable()
        {
            this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
                SR.ColumnRole,
                delegate(VisitLocationDetail vl)
                {
                    return vl.Role.Value;
                },
                0.8f));
            this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
                SR.ColumnLocation,
                delegate(VisitLocationDetail vl)
                {
                    return vl.Location.ToString();
                },
                2.5f));
            this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
                SR.ColumnStartTime,
                delegate(VisitLocationDetail vl)
                {
                    return Format.DateTime(vl.StartTime);
                },
                0.8f));
            this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
                SR.ColumnEndTime,
                delegate(VisitLocationDetail vl)
                {
                    return Format.DateTime(vl.EndTime);
                },
                0.8f));
        }
    }
}
