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

using System;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
    public class PerformingWorklistTable : Table<ModalityWorklistItem>
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
            TableColumn<ModalityWorklistItem, IconSet> priorityColumn = new TableColumn<ModalityWorklistItem, IconSet>(
                SR.ColumnPriority,
                delegate(ModalityWorklistItem item) { return GetOrderPriorityIcon(item.OrderPriority.Code); },
                0.2f);
            priorityColumn.Comparison = delegate(ModalityWorklistItem item1, ModalityWorklistItem item2)
                { return GetOrderPriorityIndex(item1.OrderPriority.Code) - GetOrderPriorityIndex(item2.OrderPriority.Code); };
            priorityColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);

            TableColumn<ModalityWorklistItem, string> mrnColumn = new TableColumn<ModalityWorklistItem, string>(
                SR.ColumnMRN,
                delegate(ModalityWorklistItem item) { return MrnFormat.Format(item.Mrn); },
                0.9f);

            TableColumn<ModalityWorklistItem, string> nameColumn = new TableColumn<ModalityWorklistItem, string>(
                SR.ColumnName,
                delegate(ModalityWorklistItem item) { return PersonNameFormat.Format(item.PatientName); },
                1.5f);

			DateTimeTableColumn<ModalityWorklistItem> scheduledForColumn = new DateTimeTableColumn<ModalityWorklistItem>(
                SR.ColumnTime,
                delegate(ModalityWorklistItem item) { return item.Time; },
                1.1f);

            TableColumn<ModalityWorklistItem, string> descriptionRow = new TableColumn<ModalityWorklistItem, string>(
                SR.ColumnDescription,
                delegate(ModalityWorklistItem item)
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
            TableColumn<ModalityWorklistItem, string> patientClassColumn = new TableColumn<ModalityWorklistItem, string>(
                SR.ColumnPatientClass,
                delegate(ModalityWorklistItem item) { return item.PatientClass == null ? null : item.PatientClass.Value; },
                0.5f);
            patientClassColumn.Visible = false;

            TableColumn<ModalityWorklistItem, string> accessionNumberColumn = new TableColumn<ModalityWorklistItem, string>(
                SR.ColumnAccessionNumber,
                delegate(ModalityWorklistItem item) { return AccessionFormat.Format(item.AccessionNumber); },
                0.75f);
            accessionNumberColumn.Visible = false;

            TableColumn<ModalityWorklistItem, string> procedureNameColumn = new TableColumn<ModalityWorklistItem, string>(
                SR.ColumnProcedure,
				delegate(ModalityWorklistItem item) { return ProcedureFormat.Format(item); },
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
            int sortColumnIndex = this.Columns.FindIndex(delegate(TableColumnBase<ModalityWorklistItem> column)
                { return column.Name.Equals(SR.ColumnTime); });

            this.Sort(new TableSortParams(this.Columns[sortColumnIndex], true));

            #region Unused OutlineColorSelector

            //this.OutlineColorSelector = delegate(object o)
            //{
            //    ModalityWorklistItem item = o as ModalityWorklistItem;
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
            //    ModalityWorklistItem item = o as ModalityWorklistItem;
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
