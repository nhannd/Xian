#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	class ProcedureRequisitionTable : Table<ProcedureRequisition>
	{
		private readonly TableColumn<ProcedureRequisition, string> _procedureColumn;
		private readonly TableColumn<ProcedureRequisition, string> _facilityColumn;
		private readonly TableColumn<ProcedureRequisition, string> _scheduledTimeColumn;
		private readonly TableColumn<ProcedureRequisition, string> _scheduledDurationColumn;
		private readonly TableColumn<ProcedureRequisition, string> _modalityColumn;

		public ProcedureRequisitionTable()
		{
			this.Columns.Add(_procedureColumn = new TableColumn<ProcedureRequisition, string>("Procedure", ProcedureFormat.Format));
			this.Columns.Add(_facilityColumn = new TableColumn<ProcedureRequisition, string>("Facility", FormatPerformingFacility));
			this.Columns.Add(_scheduledTimeColumn = new TableColumn<ProcedureRequisition, string>("Scheduled Time", FormatScheduledTime));
			this.Columns.Add(_scheduledDurationColumn = new TableColumn<ProcedureRequisition, string>("Scheduled Duration", FormatScheduledDuration));
			this.Columns.Add(_modalityColumn = new TableColumn<ProcedureRequisition, string>("Modality", FormatScheduledModality));
		}

		public TableColumn<ProcedureRequisition, string> ScheduledDurationColumn
		{
			get { return _scheduledDurationColumn; }
		}

		public TableColumn<ProcedureRequisition, string> ModalityColumn
		{
			get { return _modalityColumn; }
		}

		private string FormatPerformingFacility(ProcedureRequisition requisition)
		{
			var sb = new StringBuilder();
			if (requisition.PerformingFacility != null)
			{
				sb.Append(requisition.PerformingFacility.Name);
			}
			if (requisition.PerformingDepartment != null)
			{
				sb.Append(" (" + requisition.PerformingDepartment.Name + ")");
			}
			return sb.ToString();
		}

		private string FormatScheduledModality(ProcedureRequisition procedureRequisition)
		{
			return procedureRequisition.Modality != null ? procedureRequisition.Modality.Name : null;
		}

		private static string FormatScheduledTime(ProcedureRequisition item)
		{
			// if new or scheduled
			if (item.Status != null && item.Status.Code != "SC")
				return item.Status.Value;

			if (item.Cancelled)
				return "Cancel Pending";

			return Format.DateTime(item.ScheduledTime);
		}

		private string FormatScheduledDuration(ProcedureRequisition procedureRequisition)
		{
			return string.Format("{0} min", procedureRequisition.ScheduledDuration);
		}

	}
}
