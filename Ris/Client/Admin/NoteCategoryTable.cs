#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client.Admin
{
    public class NoteCategoryTable : Table<PatientNoteCategorySummary>
    {
        public NoteCategoryTable()
        {
            this.Columns.Add(new TableColumn<PatientNoteCategorySummary, string>(SR.ColumnSeverity,
                delegate(PatientNoteCategorySummary category) { return category.Severity.Value; },
                0.2f));

            this.Columns.Add(new TableColumn<PatientNoteCategorySummary, string>(SR.ColumnCategory,
                delegate(PatientNoteCategorySummary category) { return category.Name; },
                0.5f));

            this.Columns.Add(new TableColumn<PatientNoteCategorySummary, string>(SR.ColumnDescription,
                delegate(PatientNoteCategorySummary category) { return category.Description; },
                1.0f));
        }
    }
}
