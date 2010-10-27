#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	public class PerformingDocumentationMppsSummaryTable : Table<ModalityPerformedProcedureStepDetail>
	{
		public PerformingDocumentationMppsSummaryTable()
		{
			this.Columns.Add(new TableColumn<ModalityPerformedProcedureStepDetail, string>(
								 SR.ColumnName,
								 FormatDescription,
								 5.0f));

			this.Columns.Add(new TableColumn<ModalityPerformedProcedureStepDetail, string>(
								 SR.ColumnState,
								 FormatStatus,
								 1.2f));

			var sortColumn = 
				new DateTimeTableColumn<ModalityPerformedProcedureStepDetail>(
								 SR.ColumnStartTime,
								 mpps => mpps.StartTime,
								 1.5f);

			this.Columns.Add(sortColumn);
			this.Sort(new TableSortParams(sortColumn, true));

			var endTimeColumn = new DateTimeTableColumn<ModalityPerformedProcedureStepDetail>(
								 SR.ColumnEndTime,
								 mpps => mpps.EndTime,
								 1.5f);

			this.Columns.Add(endTimeColumn);
		}

		private static string FormatStatus(ModalityPerformedProcedureStepDetail mpps)
		{
			return mpps.State.Code == "CM" ? "Performed" : mpps.State.Value;
		}

		private static string FormatDescription(ModalityPerformedProcedureStepDetail mpps)
		{
			var description = StringUtilities.Combine(mpps.ModalityProcedureSteps, " / ",
				delegate(ModalityProcedureStepSummary mps)
				{
					var modifier = ProcedureFormat.FormatModifier(mps.Procedure.Portable, mps.Procedure.Laterality);
					return string.IsNullOrEmpty(modifier) 
						? mps.Description 
						: string.Format("{0} ({1})", mps.Description, modifier);
				});

			return description;
		}
	}
}