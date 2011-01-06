#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
    public class AddressTable : Table<AddressDetail>
    {
        public AddressTable()
        {
            this.Columns.Add(new TableColumn<AddressDetail, string>(SR.ColumnType,
                delegate(AddressDetail a) { return a.Type.Value; }, 
                1.1f));
            this.Columns.Add(new TableColumn<AddressDetail, string>(SR.ColumnAddress,
                delegate(AddressDetail a) { return AddressFormat.Format(a); }, 
                2.2f));
            this.Columns.Add(new DateTableColumn<AddressDetail>(SR.ColumnExpiryDate,
                delegate(AddressDetail a) { return a.ValidRangeUntil; }, 
                0.9f));

        }
    }
}
