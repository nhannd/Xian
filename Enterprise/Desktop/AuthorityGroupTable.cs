#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
            Columns.Add(new TableColumn<AuthorityGroupSummary, string>(
                            SR.ColumnAuthorityGroupName,
                            summary => summary.Name,
                            0.35f));

            Columns.Add(new TableColumn<AuthorityGroupSummary, string>(
                            SR.ColumnAuthorityGroupDescription,
                            summary => summary.Description,
                            0.5f));

            Columns.Add(new TableColumn<AuthorityGroupSummary, bool>(
                            SR.ColumnAuthorityGroupDataGroup,
                            summary => summary.DataGroup,
                            0.15f));
        }
    }
}
