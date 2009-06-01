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
					                                               EventIdentificationTypeEventOutcomeIndicator.Success,
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
					                                         EventIdentificationTypeEventOutcomeIndicator.Success,
					                                         EventIdentificationTypeEventActionCode.E,
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
