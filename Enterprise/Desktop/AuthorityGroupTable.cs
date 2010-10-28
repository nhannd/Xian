#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;

namespace ClearCanvas.Enterprise.Desktop
{
    public class AuthorityGroupTable : Table<AuthorityGroupSummary>
    {
        public AuthorityGroupTable()
        {
            this.Columns.Add(new TableColumn<AuthorityGroupSummary, string>(
                SR.ColumnAuthorityGroupName,
                delegate(AuthorityGroupSummary summary)
                {
                    return summary.Name;
                },
                0.5f));
        }
    }
}
