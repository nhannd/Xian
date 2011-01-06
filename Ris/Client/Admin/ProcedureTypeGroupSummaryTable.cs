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

namespace ClearCanvas.Ris.Client.Admin
{
    public class ProcedureTypeGroupSummaryTable : Table<ProcedureTypeGroupSummary>
    {
        public ProcedureTypeGroupSummaryTable()
        {
            this.Columns.Add(new TableColumn<ProcedureTypeGroupSummary, string>(SR.ColumnName,
                delegate(ProcedureTypeGroupSummary summary) { return summary.Name; },
                0.5f));

            this.Columns.Add(new TableColumn<ProcedureTypeGroupSummary, string>(SR.ColumnCategory,
                delegate(ProcedureTypeGroupSummary summary) { return summary.Category.Value; },
                0.5f));

            this.Columns.Add(new TableColumn<ProcedureTypeGroupSummary, string>(SR.ColumnDescription,
                delegate(ProcedureTypeGroupSummary summary) { return summary.Description; },
                1.5f));
        }
    }
}