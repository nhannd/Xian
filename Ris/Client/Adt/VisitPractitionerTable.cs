using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    public class VisitPractitionerTable : Table<VisitPractitionerDetail>
    {
        public VisitPractitionerTable()
        {
            this.Columns.Add(new TableColumn<VisitPractitionerDetail, string>(
                SR.ColumnRole,
                delegate(VisitPractitionerDetail vp)
                {
                    return vp.Role.Value;
                },
                0.8f));
            this.Columns.Add(new TableColumn<VisitPractitionerDetail, string>(
                SR.ColumnPractitioner,
                delegate(VisitPractitionerDetail vp)
                {
                    return PersonNameFormat.Format(vp.Practitioner.Name);
                },
                2.5f));
            this.Columns.Add(new TableColumn<VisitPractitionerDetail, string>(
                SR.ColumnStartTime,
                delegate(VisitPractitionerDetail vp)
                {
                    return Format.DateTime(vp.StartTime);
                },
                0.8f));
            this.Columns.Add(new TableColumn<VisitPractitionerDetail, string>(
                SR.ColumnEndTime,
                delegate(VisitPractitionerDetail vp)
                {
                    return Format.DateTime(vp.EndTime);
                },
                0.8f));
        }
    }
}
