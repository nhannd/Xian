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

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
    public class ProtocolGroupTable : Table<ProtocolGroupSummary>
    {
        public ProtocolGroupTable()
        {
            this.Columns.Add(new TableColumn<ProtocolGroupSummary, string>(SR.ColumnName,
                                                                           delegate(ProtocolGroupSummary summary) { return summary.Name; },
                                                                           0.5f));

            this.Columns.Add(new TableColumn<ProtocolGroupSummary, string>(SR.ColumnDescription,
                                                                           delegate(ProtocolGroupSummary summary) { return summary.Description; },
                                                                           1.0f));
        }
    }
}