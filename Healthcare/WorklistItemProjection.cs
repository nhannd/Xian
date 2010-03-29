using System;
using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// Instances of this class are immutable.
	/// </remarks>
	public class WorklistItemProjection
	{
		#region Private Static

		private static readonly WorklistItemProjection _procedureStepBase;
		private static readonly WorklistItemProjection _reportingBase;

		static WorklistItemProjection()
		{
			_procedureStepBase = new WorklistItemProjection(
				new [] {
					WorklistItemField.ProcedureStep,
					WorklistItemField.ProcedureStepName,
					WorklistItemField.ProcedureStepState,
                    WorklistItemField.Procedure,
                    WorklistItemField.Order,
                    WorklistItemField.Patient,
                    WorklistItemField.PatientProfile,
                    WorklistItemField.Mrn,
                    WorklistItemField.PatientName,
                    WorklistItemField.AccessionNumber,
                    WorklistItemField.Priority,
                    WorklistItemField.PatientClass,
                    WorklistItemField.DiagnosticServiceName,
                    WorklistItemField.ProcedureTypeName,
					WorklistItemField.ProcedurePortable,
					WorklistItemField.ProcedureLaterality,
				});

			_reportingBase = _procedureStepBase.AddFields(
				new [] {
					WorklistItemField.Report,
					WorklistItemField.ReportPartIndex
				});

			// initialize the "search" projections for each type of worklist

			ModalityWorklistSearch = GetProcedureStepProjection(WorklistItemField.ProcedureScheduledStartTime);

			// need to display the correct time field
			// ProcedureScheduledStartTime seems like a reasonable choice for registration homepage search,
			// as it gives a general sense of when the procedure occurs in time
			RegistrationWorklistSearch = GetProcedureStepProjection(WorklistItemField.ProcedureScheduledStartTime);

			ReportingWorklistSearch = GetReportingProjection(WorklistItemField.ProcedureStartTime);

			// TODO: this timefield is the value from when this broker was part of ReportingWorklistBroker,
			// but what should it really be?  
			ProtocolWorklistSearch = GetProcedureStepProjection(WorklistItemField.ProcedureStartTime);

		}

		#endregion

		public static readonly WorklistItemProjection ModalityWorklistSearch;
		public static readonly WorklistItemProjection RegistrationWorklistSearch;
		public static readonly WorklistItemProjection ReportingWorklistSearch;
		public static readonly WorklistItemProjection ProtocolWorklistSearch;


		public static WorklistItemProjection GetProcedureStepProjection(WorklistItemField timeField)
		{
			return _procedureStepBase.AddFields(new[] { timeField });
		}

		public static WorklistItemProjection GetReportingProjection(WorklistItemField timeField)
		{
			return _reportingBase.AddFields(new[] { timeField });
		}

		private readonly List<WorklistItemField> _fields;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="fields"></param>
		public WorklistItemProjection(IList<WorklistItemField> fields)
		{
			_fields = new List<WorklistItemField>(fields);
		}

		/// <summary>
		/// Get a read-only list of the fields specified in this projection.
		/// </summary>
		public IList<WorklistItemField> Fields
		{
			get { return _fields.AsReadOnly(); }
		}

		/// <summary>
		/// Returns a new projection which contains both the fields of this projection and the specified fields.
		/// </summary>
		/// <param name="fields"></param>
		/// <returns></returns>
		public WorklistItemProjection AddFields(IList<WorklistItemField> fields)
		{
			var copy = new List<WorklistItemField>(_fields);
			copy.AddRange(fields);
			return new WorklistItemProjection(copy);
		}

		/// <summary>
		/// Returns a new projection which contains only the fields in this projection that satisfy the specified filter function.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		public WorklistItemProjection Filter(Predicate<WorklistItemField> filter)
		{
			var filtered = CollectionUtils.Select(_fields, filter);
			return new WorklistItemProjection(filtered);
		}
		
	}
}
