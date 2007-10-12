#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Client.Adt
{
    public class RegistrationWorklistTable : Table<RegistrationWorklistItem>
    {
        public RegistrationWorklistTable()
        {
            // Patient related info
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnMRN,
                delegate(RegistrationWorklistItem item) { return MrnFormat.Format(item.Mrn); }, 1.0f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnName,
                delegate(RegistrationWorklistItem item) { return PersonNameFormat.Format(item.Name); }, 1.5f));

            // Order related info
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnAccessionNumber,
                delegate(RegistrationWorklistItem item) { return String.IsNullOrEmpty(item.AccessionNumber) ? "-" : item.AccessionNumber; }, 0.75f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnPatientClass,
                delegate(RegistrationWorklistItem item) { return String.IsNullOrEmpty(item.PatientClass) ? "-" : item.PatientClass; }, 0.5f));
            this.Columns.Add(new TableColumn<RegistrationWorklistItem, string>(SR.ColumnScheduledFor,
                delegate(RegistrationWorklistItem item) { return item.EarliestScheduledTime == null ? "-" : Format.Time(item.EarliestScheduledTime); }, 0.5f));

            TableColumn<RegistrationWorklistItem, IconSet> priorityColumn = new TableColumn<RegistrationWorklistItem, IconSet>(
                SR.ColumnPriority, delegate(RegistrationWorklistItem item) { return GetOrderPriorityIcon(item.OrderPriority.Code); }, 0.5f);
            priorityColumn.Comparison = delegate(RegistrationWorklistItem item1, RegistrationWorklistItem item2)
                                            {
                                                return GetOrderPriorityIndex(item1.OrderPriority.Code) - GetOrderPriorityIndex(item2.OrderPriority.Code);
                                            };
            priorityColumn.ResourceResolver = new ResourceResolver(this.GetType().Assembly);
            this.Columns.Add(priorityColumn);

            // Sort the table by Scheduled Time initially
            int sortColumnIndex = this.Columns.FindIndex(delegate(TableColumnBase<RegistrationWorklistItem> column)
                { return column.Name.Equals(SR.ColumnScheduledFor); });

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
