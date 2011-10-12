#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.Dicom
{
	public static class AssociationAuditLogger
	{
		public static void BeginInstancesTransferAuditLogger(List<StorageInstance> instances, AssociationParameters parms)
		{
			Dictionary<string, AuditPatientParticipantObject> list = new Dictionary<string, AuditPatientParticipantObject>();

			foreach (StorageInstance instance in instances)
			{
				string key = instance.PatientId + instance.PatientsName;
				if (!list.ContainsKey(key))
				{
					AuditPatientParticipantObject patient =
						new AuditPatientParticipantObject(instance.PatientsName, instance.PatientId);
					list.Add(key, patient);
				}
			}

			foreach (AuditPatientParticipantObject patient in list.Values)
			{
				// Audit Log
				BeginTransferringDicomInstancesAuditHelper audit =
					new BeginTransferringDicomInstancesAuditHelper(ServerPlatform.AuditSource,
					                                               EventIdentificationContentsEventOutcomeIndicator.Success,
					                                               parms, patient);

				foreach (StorageInstance instance in instances)
				{
					if (patient.PatientId.Equals(instance.PatientId)
					    && patient.PatientsName.Equals(instance.PatientsName))
					{
						audit.AddStorageInstance(instance);
					}
				}

				ServerPlatform.LogAuditMessage(audit);
			}
		}

		public static void InstancesTransferredAuditLogger(DicomScpContext context, ServerAssociationParameters assocParams, List<StorageInstance> instances)
		{
			Dictionary<string, AuditPatientParticipantObject> list = new Dictionary<string, AuditPatientParticipantObject>();

			foreach (StorageInstance instance in instances)
			{
				string key = instance.PatientId + instance.PatientsName;
				if (!list.ContainsKey(key))
				{
					AuditPatientParticipantObject patient =
						new AuditPatientParticipantObject(instance.PatientsName, instance.PatientId);
					list.Add(key, patient);
				}
			}

			foreach (AuditPatientParticipantObject patient in list.Values)
			{
				// Audit Log
				DicomInstancesTransferredAuditHelper helper =
					new DicomInstancesTransferredAuditHelper(ServerPlatform.AuditSource,
					                                         EventIdentificationContentsEventOutcomeIndicator.Success,
					                                         EventIdentificationContentsEventActionCode.E,
					                                         assocParams);

				foreach (StorageInstance instance in instances)
				{
					if (patient.PatientId.Equals(instance.PatientId)
					    && patient.PatientsName.Equals(instance.PatientsName))
					{
						helper.AddStorageInstance(instance);
					}
				}

				ServerPlatform.LogAuditMessage(helper);
			}
		}
	}
}
