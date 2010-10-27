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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client
{
    public class ContactPersonTable : Table<ContactPersonDetail>
    {
        public ContactPersonTable()
        {
            this.Columns.Add(new TableColumn<ContactPersonDetail, string>(SR.ColumnContactType,
                delegate(ContactPersonDetail c) { return c.Type.Value; },
                1.0f));
            this.Columns.Add(new TableColumn<ContactPersonDetail, string>(SR.ColumnName,
                delegate(ContactPersonDetail c) { return c.Name; },
                3.0f));
        }
    }
}
