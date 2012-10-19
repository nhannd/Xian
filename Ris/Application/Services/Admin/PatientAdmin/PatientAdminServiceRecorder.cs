#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

namespace ClearCanvas.Ris.Application.Services.Admin.PatientAdmin
{
	class PatientAdminServiceRecorder
	{
		static class Operations
		{
			public const string New = "Patient:New";
			public const string OpenForUpdate = "PatientProfile:OpenForUpdate";
			public const string Update = "PatientProfile:Update";
		}

		internal class AddPatient : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var response = (AddPatientResponse)recorderContext.Response;
				var patientProfile = persistenceContext.Load<PatientProfile>(response.PatientProfile.PatientProfileRef, EntityLoadFlags.None);

				return new OperationData(Operations.New, patientProfile);
			}
		}

		internal class LoadPatientProfileForEdit : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var response = (LoadPatientProfileForEditResponse)recorderContext.Response;
				var patientProfile = persistenceContext.Load<PatientProfile>(response.PatientProfileRef, EntityLoadFlags.None);

				return new OperationData(Operations.OpenForUpdate, patientProfile);
			}
		}

		internal class UpdatePatientProfile : RisServiceOperationRecorderBase
		{
			protected override OperationData Capture(IServiceOperationRecorderContext recorderContext, IPersistenceContext persistenceContext)
			{
				var response = (UpdatePatientProfileResponse)recorderContext.Response;
				var patientProfile = persistenceContext.Load<PatientProfile>(response.PatientProfile.PatientProfileRef, EntityLoadFlags.None);

				IncludeChangeSetFor(patientProfile);
				IncludeChangeSetFor(patientProfile.Patient);

				return new OperationData(Operations.Update, patientProfile);
			}
		}
	}
}
