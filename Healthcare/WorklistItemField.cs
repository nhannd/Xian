#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

namespace ClearCanvas.Healthcare
{
	public class WorklistItemFieldLevel
	{
		public static readonly WorklistItemFieldLevel Patient = new WorklistItemFieldLevel(0);
		public static readonly WorklistItemFieldLevel Procedure = new WorklistItemFieldLevel(1);
		public static readonly WorklistItemFieldLevel ProcedureStep = new WorklistItemFieldLevel(2);
		public static readonly WorklistItemFieldLevel Report = new WorklistItemFieldLevel(3);

		private readonly int _index;

		public WorklistItemFieldLevel(int index)
		{
			_index = index;
		}

		public bool Includes(WorklistItemFieldLevel level)
		{
			return _index >= level._index;
		}
	}

	/// <summary>
	/// Defines a set of constants that describe fields used by worklists.
	/// </summary>
	public class WorklistItemField
	{
		public class EntityRefField : WorklistItemField
		{
			public EntityRefField(WorklistItemFieldLevel level)
				:base(level)
			{
			}

			public override bool IsEntityRefField { get { return true; } }
		}


		#region EntityRef field constants

		public static readonly WorklistItemField ProcedureStep = new EntityRefField(WorklistItemFieldLevel.ProcedureStep);

		public static readonly WorklistItemField Procedure = new EntityRefField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField ProcedureCheckIn = new EntityRefField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField Protocol = new EntityRefField(WorklistItemFieldLevel.Procedure);

		public static readonly WorklistItemField Order = new EntityRefField(WorklistItemFieldLevel.Procedure);

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

		public WorklistItemFieldLevel Level
		{
			get { return _level; }
		}

		public virtual bool IsEntityRefField { get { return false; } }
	}
}
