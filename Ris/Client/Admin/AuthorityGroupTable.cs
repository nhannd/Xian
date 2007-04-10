using System;

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;

namespace ClearCanvas.Ris.Client.Admin
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
