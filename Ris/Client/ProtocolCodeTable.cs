#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class ProtocolCodeTable : Table<ProtocolCodeSummary>
    {
        public ProtocolCodeTable()
        {
            this.Columns.Add(new TableColumn<ProtocolCodeSummary, string>("Code",
											 delegate(ProtocolCodeSummary detail)
                                                 { return detail.Name; },
                                             0.5f));
        }
    }
}