using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;

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
                    //TODO: PersonNameDetail formatting
                    //return Format.Custom(vp.Practitioner.PersonNameDetail);
                    return String.Format("{0}, {1}", vp.Practitioner.PersonNameDetail.FamilyName, vp.Practitioner.PersonNameDetail.GivenName);
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
