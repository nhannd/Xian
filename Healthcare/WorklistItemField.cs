#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Defines a set of constants that group <see cref="WorklistItemField"/> instances into "levels",
	/// according to the healthcare entity with which they are most closely associated.
	/// </summary>
	public class WorklistItemFieldLevel
	{
		/// <summary>
		/// Patient level - includes patient and patient profile data.
		/// </summary>
		public static readonly WorklistItemFieldLevel Patient = new WorklistItemFieldLevel(0);

		/// <summary>
		/// Procedure level - includes procedure, order and visit data.
		/// </summary>
		public static readonly WorklistItemFieldLevel Procedure = new WorklistItemFieldLevel(1);

		/// <summary>
		/// Procedure step level - includes procedure step data.
		/// </summary>
		public static readonly WorklistItemFieldLevel ProcedureStep = new WorklistItemFieldLevel(2);

		/// <summary>
		/// Report level - includes report data.
		/// </summary>
		public static readonly WorklistItemFieldLevel Report = new WorklistItemFieldLevel(3);

		private readonly int _index;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="index"></param>
		private WorklistItemFieldLevel(int index)
		{
			_index = index;
		}

		/// <summary>
		/// Tests whether this level includes the specified level.
		/// </summary>
		/// <remarks>
		/// Levels can be thought of as nested sets, where each level includes all the levels below it.
		/// This method determines whether the specified level at the same level, or below, as this instance.
		/// </remarks>
		/// <param name="level"></param>
		/// <returns></returns>
		public bool Includes(WorklistItemFieldLevel level)
		{
			return _index >= level._index;
		}
	}

	/// <summary>
	/// Defines a set of constants that represent fields that can appear in worklist items.
	/// </summary>
	public class WorklistItemField
	{
		/// <summary>
		/// Subclass of <see cref="WorklistItemField"/> used to distinguish entity-ref fields from data fields.
		/// </summary>
		private class EntityRefField : WorklistItemField
		{
			internal EntityRefField(WorklistItemFieldLevel level)
				:base(level)
			{
			}

			public override bool IsEntityRefField { get { return true; } }
		}


		#region EntityRef field constants

		public static readonly WorklistItemField ProcedureStep = new EntityRefField(WorklistItemFieldLevel.ProcedureStep);

		public static readonly WorklistItemField Procedure = new EntityRefField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureType = new EntityRefField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureCheckIn = new EntityRefField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField Protocol = new EntityRefField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField Order = new EntityRefField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField DiagnosticService = new EntityRefField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField Visit = new EntityRefField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField Patient = new EntityRefField(WorklistItemFieldLevel.Patient);

		public static readonly WorklistItemField PatientProfile = new EntityRefField(WorklistItemFieldLevel.Patient);

		#endregion


		#region Common value field constants

		public static readonly WorklistItemField Mrn = new WorklistItemField(WorklistItemFieldLevel.Patient);

		public static readonly WorklistItemField PatientName = new WorklistItemField(WorklistItemFieldLevel.Patient);

		public static readonly WorklistItemField AccessionNumber = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField Priority = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField PatientClass = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField DiagnosticServiceName = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureTypeName = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedurePortable = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureLaterality = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureStepName = new WorklistItemField(WorklistItemFieldLevel.ProcedureStep);

		public static readonly WorklistItemField ProcedureStepState = new WorklistItemField(WorklistItemFieldLevel.ProcedureStep);

        public static readonly WorklistItemField ProcedureStepId = new WorklistItemField(WorklistItemFieldLevel.ProcedureStep);
		
		
		#endregion

		#region Reporting-specific field constants

		public static readonly WorklistItemField Report = new EntityRefField(WorklistItemFieldLevel.Report);

		public static readonly WorklistItemField ReportPart = new EntityRefField(WorklistItemFieldLevel.Report);

		public static readonly WorklistItemField ReportPartIndex = new WorklistItemField(WorklistItemFieldLevel.Report);

		public static readonly WorklistItemField ReportPartHasErrors = new WorklistItemField(WorklistItemFieldLevel.Report);

		#endregion


		#region Time field constants

		public static readonly WorklistItemField OrderSchedulingRequestTime = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureScheduledStartTime = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureCheckInTime = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureCheckOutTime = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureStartTime = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureEndTime = new WorklistItemField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureStepCreationTime = new WorklistItemField(WorklistItemFieldLevel.ProcedureStep);

		public static readonly WorklistItemField ProcedureStepScheduledStartTime = new WorklistItemField(WorklistItemFieldLevel.ProcedureStep);

		public static readonly WorklistItemField ProcedureStepStartTime = new WorklistItemField(WorklistItemFieldLevel.ProcedureStep);

		public static readonly WorklistItemField ProcedureStepEndTime = new WorklistItemField(WorklistItemFieldLevel.ProcedureStep);

		public static readonly WorklistItemField ReportPartPreliminaryTime = new WorklistItemField(WorklistItemFieldLevel.Report);

		public static readonly WorklistItemField ReportPartCompletedTime = new WorklistItemField(WorklistItemFieldLevel.Report);

		#endregion


		private readonly WorklistItemFieldLevel _level;

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorklistItemField(WorklistItemFieldLevel level)
		{
			_level = level;
		}

		/// <summary>
		/// Gets the level that this field is associated with.
		/// </summary>
		public WorklistItemFieldLevel Level
		{
			get { return _level; }
		}

		/// <summary>
		/// Gets a value indicating whether this field is an entity-ref field.
		/// </summary>
		public virtual bool IsEntityRefField { get { return false; } }
	}
}
