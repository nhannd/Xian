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
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
    public class PerformingWorklistTable : Table<ModalityWorklistItemSummary>
    {
        private static readonly int NumRows = 2;
        private static readonly int DescriptionRow = 1;

        public PerformingWorklistTable()
            : this(NumRows)
        {
        }

        private PerformingWorklistTable(int cellRowCount)
            : base(cellRowCount)
        {
            // Visible Columns
            TableColumn<ModalityWorklistItemSummary, IconSet> priorityColumn = new TableColumn<ModalityWorklistItemSummary, IconSet>(
                SR.ColumnPriority,
                delegate(ModalityWorklistItemSummary item) { return GetOrderPriorityIcon(item.OrderPriority.Code); },
                0.2f);
            priorityColumn.Comparison = delegate(ModalityWorklistItemSummary item1, ModalityWorklistItemSummary item2)
                { return GetOrderPriorityIndex(item1.OrderPriority.Code) - GetOrderPriorityIndex(item2.OrderPriority.Code); };
            priorityColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);

            TableColumn<ModalityWorklistItemSummary, string> mrnColumn = new TableColumn<ModalityWorklistItemSummary, string>(
                SR.ColumnMRN,
                delegate(ModalityWorklistItemSummary item) { return MrnFormat.Format(item.Mrn); },
                0.9f);

            TableColumn<ModalityWorklistItemSummary, string> nameColumn = new TableColumn<ModalityWorklistItemSummary, string>(
                SR.ColumnName,
                delegate(ModalityWorklistItemSummary item) { return PersonNameFormat.Format(item.PatientName); },
                1.5f);

			DateTimeTableColumn<ModalityWorklistItemSummary> scheduledForColumn = new DateTimeTableColumn<ModalityWorklistItemSummary>(
                SR.ColumnTime,
                delegate(ModalityWorklistItemSummary item) { return item.Time; },
                1.1f);

            TableColumn<ModalityWorklistItemSummary, string> descriptionRow = new TableColumn<ModalityWorklistItemSummary, string>(
                SR.ColumnDescription,
                delegate(ModalityWorklistItemSummary item)
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
            TableColumn<ModalityWorklistItemSummary, string> patientClassColumn = new TableColumn<ModalityWorklistItemSummary, string>(
                SR.ColumnPatientClass,
                delegate(ModalityWorklistItemSummary item) { return item.PatientClass == null ? null : item.PatientClass.Value; },
                0.5f);
            patientClassColumn.Visible = false;

            TableColumn<ModalityWorklistItemSummary, string> accessionNumberColumn = new TableColumn<ModalityWorklistItemSummary, string>(
                SR.ColumnAccessionNumber,
                delegate(ModalityWorklistItemSummary item) { return AccessionFormat.Format(item.AccessionNumber); },
                0.75f);
            accessionNumberColumn.Visible = false;

            TableColumn<ModalityWorklistItemSummary, string> procedureNameColumn = new TableColumn<ModalityWorklistItemSummary, string>(
                SR.ColumnProcedure,
				delegate(ModalityWorklistItemSummary item) { return ProcedureFormat.Format(item); },
                1.0f);
            procedureNameColumn.Visible = false;

            // The order of the addition determines the order of SortBy dropdown
            this.Columns.Add(priorityColumn);
            this.Columns.Add(mrnColumn);
            this.Columns.Add(nameColumn);
            this.Columns.Add(patientClassColumn);
            this.Columns.Add(accessionNumberColumn);
            this.Columns.Add(procedureNameColumn);
            this.Columns.Add(scheduledForColumn);
            this.Columns.Add(descriptionRow);

            // Sort by Scheduled Time initially
            int sortColumnIndex = this.Columns.FindIndex(delegate(TableColumnBase<ModalityWorklistItemSummary> column)
                { return column.Name.Equals(SR.ColumnTime); });

            this.Sort(new TableSortParams(this.Columns[sortColumnIndex], true));

            #region Unused OutlineColorSelector

            //this.OutlineColorSelector = delegate(object o)
            //{
            //    ModalityWorklistItemSummary item = o as ModalityWorklistItemSummary;
            //    if (item != null)
            //    {
            //        switch (item.Priority.Code)
            //        {
            //            case "S":
            //                return "Red";
            //            case "A":
            //                return "Yellow";
            //            case "R":
            //            default:
            //                return "Empty";
            //        }
            //    }
            //    else
            //    {
            //        return "Empty";
            //    }
            //};

            //this.BackgroundColorSelector = delegate(object o)
            //{
            //    ModalityWorklistItemSummary item = o as ModalityWorklistItemSummary;
            //    if (item != null)
            //    {
            //        switch (item.Priority.Code)
            //        {
            //            case "S":
            //                return "Red";
            //            case "A":
            //                return "Yellow";
            //            case "R":
            //            default:
            //                return "Empty";
            //        }
            //    }
            //    else
            //    {
            //        return "Empty";
            //    }
            //};

            #endregion
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
