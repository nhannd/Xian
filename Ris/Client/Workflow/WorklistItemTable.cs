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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class WorklistItemTable<TItem> : Table<TItem>
		where TItem: WorklistItemSummaryBase
	{
		private const int NumRows = 2;
		private const int DescriptionRow = 1;

		public WorklistItemTable()
			: base(NumRows)
		{
			// Visible Columns
			var priorityColumn = new TableColumn<TItem, IconSet>(SR.ColumnPriority, item => GetPriorityIcon(item.OrderPriority), 0.2f)
									{
										Comparison = ComparePriorities,
										ResourceResolver = new ResourceResolver(this.GetType().Assembly)
									};

			var mrnColumn = new TableColumn<TItem, string>(SR.ColumnMRN, item => MrnFormat.Format(item.Mrn), 0.9f);
			var nameColumn = new TableColumn<TItem, string>(SR.ColumnName, item => PersonNameFormat.Format(item.PatientName), 1.5f);
			var scheduledForColumn = new DateTimeTableColumn<TItem>(SR.ColumnTime, item => item.Time, 1.1f);
			var descriptionRow = new TableColumn<TItem, string>(SR.ColumnDescription, FormatDescription, 1.0f, DescriptionRow);

			// Invisible but sortable columns
			var patientClassColumn = new TableColumn<TItem, string>(SR.ColumnPatientClass, FormatPatientClass, 1.0f) { Visible = false };

			var accessionNumberColumn = new TableColumn<TItem, string>(SR.ColumnAccessionNumber,
				item => AccessionFormat.Format(item.AccessionNumber), 1.0f) { Visible = false };

			var procedureNameColumn = new TableColumn<TItem, string>(SR.ColumnProcedure, ProcedureFormat.Format, 1.0f) { Visible = false };

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

		private static int ComparePriorities(TItem item1, TItem item2)
		{
			return GetPriorityIndex(item1.OrderPriority) - GetPriorityIndex(item2.OrderPriority);
		}

		private static string FormatPatientClass(TItem item)
		{
			return item.PatientClass == null ? null : item.PatientClass.Value;
		}

		private static string FormatDescription(TItem item)
		{
			// if there is no accession number, this item represents a patient only, not an order
			return item.AccessionNumber == null ? null :
				string.Format("{0} {1}", AccessionFormat.Format(item.AccessionNumber), ProcedureFormat.Format(item));
		}

		private static int GetPriorityIndex(EnumValueInfo orderPriority)
		{
			if (orderPriority == null)
				return 0;

			switch (orderPriority.Code)
			{
				case "S": // Stat
					return 2;
				case "A": // Urgent
					return 1;
				default: // Routine
					return 0;
			}
		}

		private static IconSet GetPriorityIcon(EnumValueInfo orderPriority)
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

	public class RegistrationWorklistTable : WorklistItemTable<RegistrationWorklistItemSummary>
	{
	}

	public class PerformingWorklistTable : WorklistItemTable<ModalityWorklistItemSummary>
	{
	}

	public class ReportingWorklistTable : WorklistItemTable<ReportingWorklistItemSummary>
	{
		
	}
}
