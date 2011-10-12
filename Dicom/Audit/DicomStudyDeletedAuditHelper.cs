#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Dicom.Network.Scu;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// DICOM Study Deleted
	/// </summary>
	/// <remarks>
	/// This message describes the event of deletion of one or more studies and all associated SOP Instances in
	/// a single action. This message may only include information about a single patient.
	/// </remarks>
	public class DicomStudyDeletedAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public DicomStudyDeletedAuditHelper(DicomAuditSource auditSource, EventIdentificationContentsEventOutcomeIndicator outcome)
			: base("DicomStudyDeleted")
		{
			AuditMessage.EventIdentification = new EventIdentificationContents();
			AuditMessage.EventIdentification.EventID = EventID.DICOMStudyDeleted;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationContentsEventActionCode.D;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddAuditSource(auditSource);
		}

		/// <summary>
		/// Add the ID of person or process deleting the study.  If both the person
		/// and process are known, both shall be included.
		/// </summary>
		/// <param name="participant">The participant.</param>
		public void AddUserParticipant(AuditActiveParticipant participant)
		{
			participant.UserIsRequestor = true;

			InternalAddActiveParticipant(participant);
		}

		/// <summary>
		/// Add details of a Patient.
		/// </summary>
		/// <param name="study"></param>
		public void AddPatientParticipantObject(AuditPatientParticipantObject patient)
		{
			InternalAddParticipantObject(patient.PatientId + patient.PatientsName, patient);
		}

		/// <summary>
		/// Add details of a study.
		/// </summary>
		/// <param name="study"></param>
		public void AddStudyParticipantObject(AuditStudyParticipantObject study)
		{
			InternalAddParticipantObject(study.StudyInstanceUid, study);
		}

		/// <summary>
		/// Add details of images within a study.  SOP Class information is automatically updated.
		/// </summary>
		/// <param name="instance">Descriptive object being audited</param>
		public void AddStorageInstance(StorageInstance instance)
		{
			InternalAddStorageInstance(instance);
		}
	}
}
