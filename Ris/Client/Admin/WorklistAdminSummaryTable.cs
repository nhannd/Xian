#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Admin
{
    public class WorklistAdminSummaryTable : Table<WorklistAdminSummary>
    {
        public WorklistAdminSummaryTable()
        {
            this.Columns.Add(new TableColumn<WorklistAdminSummary, string>("Name",
                delegate(WorklistAdminSummary summary) { return summary.DisplayName; },
                0.5f));

            this.Columns.Add(new TableColumn<WorklistAdminSummary, string>("Class",
                delegate(WorklistAdminSummary summary)
                {
                     return string.Format("{0} - {1}", summary.WorklistClass.CategoryName, summary.WorklistClass.DisplayName);
                },
                0.5f));

            this.Columns.Add(new TableColumn<WorklistAdminSummary, string>("Description",
                delegate(WorklistAdminSummary summary) { return summary.Description; },
                1.5f));

            this.Columns.Add(new TableColumn<WorklistAdminSummary, string>("Owner",
                delegate(WorklistAdminSummary summary)
                {
                    if (summary.OwnerStaff != null)
                        return PersonNameFormat.Format(summary.OwnerStaff.Name);
                    else if (summary.OwnerGroup != null)
                        return summary.OwnerGroup.Name;
                    else
                        return "";
                },
                1.0f));
        }
    }
}