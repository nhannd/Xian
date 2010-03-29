#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
    public class RegistrationWorklistTable : Table<RegistrationWorklistItemSummary>
    {
        private static readonly int NumRows = 2;
        private static readonly int DescriptionRow = 1;

        public RegistrationWorklistTable()
            : this(NumRows)
        {
        }

        private RegistrationWorklistTable(int cellRowCount)
            : base(cellRowCount)
        {
            // Visible Columns
            TableColumn<RegistrationWorklistItemSummary, IconSet> priorityColumn = new TableColumn<RegistrationWorklistItemSummary, IconSet>(
                SR.ColumnPriority,
                delegate(RegistrationWorklistItemSummary item) { return GetOrderPriorityIcon(item.OrderPriority); },
                0.2f);
            priorityColumn.Comparison = delegate(RegistrationWorklistItemSummary item1, RegistrationWorklistItemSummary item2)
                {
                    return GetOrderPriorityIndex(item1.OrderPriority) - GetOrderPriorityIndex(item2.OrderPriority);
                };
            priorityColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);

            TableColumn<RegistrationWorklistItemSummary, string> mrnColumn = new TableColumn<RegistrationWorklistItemSummary, string>(
                SR.ColumnMRN,
                delegate(RegistrationWorklistItemSummary item) { return MrnFormat.Format(item.Mrn); },
                0.9f);

            TableColumn<RegistrationWorklistItemSummary, string> nameColumn = new TableColumn<RegistrationWorklistItemSummary, string>(
                SR.ColumnName,
                delegate(RegistrationWorklistItemSummary item) { return PersonNameFormat.Format(item.PatientName); },
                1.5f);

            DateTimeTableColumn<RegistrationWorklistItemSummary> scheduledForColumn = new DateTimeTableColumn<RegistrationWorklistItemSummary>(
                SR.ColumnTime,
                delegate(RegistrationWorklistItemSummary item) { return item.Time; },
                1.1f);

            TableColumn<RegistrationWorklistItemSummary, string> descriptionRow = new TableColumn<RegistrationWorklistItemSummary, string>(
                SR.ColumnDescription,
                delegate(RegistrationWorklistItemSummary item)
                {
                    // if there is no accession number, this item represents a patient only, not an order
                    if (item.AccessionNumber == null) 
						return null;
                    else
						return string.Format("{0} {1}", AccessionFormat.Format(item.AccessionNumber), ProcedureFormat.Format(item));
                },
                1.0f,
                DescriptionRow);
            descriptionRow.Comparison = null;

            // Invisible but sortable columns
            TableColumn<RegistrationWorklistItemSummary, string> patientClassColumn = new TableColumn<RegistrationWorklistItemSummary, string>(
                SR.ColumnPatientClass,
                delegate(RegistrationWorklistItemSummary item) { return item.PatientClass == null ? null : item.PatientClass.Value; },
                1.0f);
            patientClassColumn.Visible = false;

            TableColumn<RegistrationWorklistItemSummary, string> accessionNumberColumn = new TableColumn<RegistrationWorklistItemSummary, string>(
                SR.ColumnAccessionNumber,
                delegate(RegistrationWorklistItemSummary item) { return AccessionFormat.Format(item.AccessionNumber); },
                1.0f);
            accessionNumberColumn.Visible = false;

			TableColumn<RegistrationWorklistItemSummary, string> procedureNameColumn = new TableColumn<RegistrationWorklistItemSummary, string>(
				SR.ColumnProcedure,
				delegate(RegistrationWorklistItemSummary item) { return ProcedureFormat.Format(item); },
				1.0f);
			procedureNameColumn.Visible = false;

            // The order of the addition determines the order of SortBy dropdown
            this.Columns.Add(priorityColumn);
            this.Columns.Add(mrnColumn);
            this.Columns.Add(nameColumn);
            this.Columns.Add(patientClassColumn);
			this.Columns.Add(procedureNameColumn);
			this.Columns.Add(accessionNumberColumn);
            this.Columns.Add(scheduledForColumn);
            this.Columns.Add(descriptionRow);

            // Sort the table by Scheduled Time initially
            this.Sort(new TableSortParams(scheduledForColumn, true));
        }

        private static int GetOrderPriorityIndex(EnumValueInfo orderPriority)
        {
            if (orderPriority == null)
                return 0;

            switch (orderPriority.Code)
            {
                case "S": // Stats
                    return 2;
                case "A": // Urgent
                    return 1;
                default: // Routine
                    return 0;
            }
        }

        private static IconSet GetOrderPriorityIcon(EnumValueInfo orderPriority)
        {
            if (orderPriority == null)
                return null;

            switch (orderPriority.Code)
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
