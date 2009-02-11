#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Client.Formatting;
using System;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
    public class ReportingWorklistTable : Table<ReportingWorklistItem>
    {
        private static readonly int NumRows = 2;
        private static readonly int DescriptionRow = 1;

        public ReportingWorklistTable()
            : this(NumRows)
        {
        }

        private ReportingWorklistTable(int cellRowCount)
            : base(cellRowCount)
        {
            // Visible Columns
            TableColumn<ReportingWorklistItem, IconSet> priorityColumn = new TableColumn<ReportingWorklistItem, IconSet>(
                SR.ColumnPriority,
                delegate(ReportingWorklistItem item) { return GetOrderPriorityIcon(item.OrderPriority.Code); },
                0.2f);
            priorityColumn.Comparison = delegate(ReportingWorklistItem item1, ReportingWorklistItem item2)
                { return GetOrderPriorityIndex(item1.OrderPriority.Code) - GetOrderPriorityIndex(item2.OrderPriority.Code); };
            priorityColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);

            TableColumn<ReportingWorklistItem, string> mrnColumn = new TableColumn<ReportingWorklistItem, string>(
                SR.ColumnMRN,
                delegate(ReportingWorklistItem item) { return MrnFormat.Format(item.Mrn); },
                0.9f);

            TableColumn<ReportingWorklistItem, string> nameColumn = new TableColumn<ReportingWorklistItem, string>(
                SR.ColumnName,
                delegate(ReportingWorklistItem item) { return PersonNameFormat.Format(item.PatientName); },
                1.5f);

            // Currently the creation time of the interpretation step
            DateTimeTableColumn<ReportingWorklistItem> procedureEndTimeColumn = new DateTimeTableColumn<ReportingWorklistItem>(
                SR.ColumnTime,
                delegate(ReportingWorklistItem item) { return item.Time; },
                1.1f);

            TableColumn<ReportingWorklistItem, string> descriptionRow = new TableColumn<ReportingWorklistItem, string>(
                SR.ColumnDescription,
                delegate(ReportingWorklistItem item)
                {
					// if there is no accession number, this item represents a patient only, not an order
					if (item.AccessionNumber == null)
						return null;
					else
						return string.Format("{0} {1}", AccessionFormat.Format(item.AccessionNumber), ProcedureFormat.Format(item));
				},
                1.0f,
                DescriptionRow);

            // Invisible but sortable columns
            TableColumn<ReportingWorklistItem, string> patientClassColumn = new TableColumn<ReportingWorklistItem, string>(
                SR.ColumnPatientClass,
                delegate(ReportingWorklistItem item) { return item.PatientClass == null ? null : item.PatientClass.Value; },
                1.0f);
            patientClassColumn.Visible = false;

            TableColumn<ReportingWorklistItem, string> accessionNumberColumn = new TableColumn<ReportingWorklistItem, string>(
                SR.ColumnAccessionNumber,
                delegate(ReportingWorklistItem item) { return AccessionFormat.Format(item.AccessionNumber); },
                1.0f);
            accessionNumberColumn.Visible = false;

            TableColumn<ReportingWorklistItem, string> procedureNameColumn = new TableColumn<ReportingWorklistItem, string>(
                SR.ColumnProcedure,
                delegate(ReportingWorklistItem item) { return ProcedureFormat.Format(item); },
                1.0f);
            procedureNameColumn.Visible = false;

            // The order of the addition determines the order of SortBy dropdown
            this.Columns.Add(priorityColumn);
            this.Columns.Add(mrnColumn);
            this.Columns.Add(nameColumn);
            this.Columns.Add(patientClassColumn);
            this.Columns.Add(accessionNumberColumn);
            this.Columns.Add(procedureNameColumn);
            this.Columns.Add(procedureEndTimeColumn);
            this.Columns.Add(descriptionRow);

            // Sort by Scheduled Time initially
            int sortColumnIndex = this.Columns.FindIndex(delegate(TableColumnBase<ReportingWorklistItem> column)
                { return column.Name.Equals(SR.ColumnTime); });

            this.Sort(new TableSortParams(this.Columns[sortColumnIndex], true));
        }

        private static int GetOrderPriorityIndex(string orderPriorityCode)
        {
            if (String.IsNullOrEmpty(orderPriorityCode))
                return 0;

            switch (orderPriorityCode)
            {
                case "S": // Stats
                    return 2;
                case "A": // Urgent
                    return 1;
                default: // Routine
                    return 0;
            }
        }

        private static IconSet GetOrderPriorityIcon(string orderPriorityCode)
        {
            switch (orderPriorityCode)
            {
                case "S": // Stats
                    return new IconSet("DoubleExclamation.png");
                case "A": // Urgent
                    return new IconSet("SingleExclamation.png");
                default:
                    return null;
            }
        }
    }
}
