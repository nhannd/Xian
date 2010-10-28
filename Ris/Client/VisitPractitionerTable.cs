#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
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
			this.Columns.Add(new DateTimeTableColumn<VisitPractitionerDetail>(
                SR.ColumnStartTime,
                delegate(VisitPractitionerDetail vp) { return vp.StartTime; },
                0.8f));
			this.Columns.Add(new DateTimeTableColumn<VisitPractitionerDetail>(
                SR.ColumnEndTime,
                delegate(VisitPractitionerDetail vp) { return vp.EndTime; },
                0.8f));
        }
    }
}
