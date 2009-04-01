using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Patient Record Audit Message Helper
	/// </summary>
	/// <remarks>
	/// <para>
	/// This message describes the event of a patient record being created, modified, accessed, or deleted.
	/// </para>
	/// <para>
	/// Note: There are several types of patient records managed by both DICOM and non-DICOM system. DICOM
	/// applications often manipulate patient records managed by a variety of systems, and thus may be
	/// obligated by site security policies to record such events in the audit logs. This audit event can be used to
	/// record the access or manipulation of patient records where specific DICOM SOP Instances are not
	/// involved.
	/// </para>
	/// </remarks>
	public class PatientRecordAuditHelper : DicomAuditHelper
	{
		public PatientRecordAuditHelper(DicomAuditSource auditSource, EventIdentificationTypeEventOutcomeIndicator outcome, 
			EventIdentificationTypeEventActionCode code)
		{
			AuditMessage.EventIdentification = new EventIdentificationType();
			AuditMessage.EventIdentification.EventID = CodedValueType.PatientRecord;
			AuditMessage.EventIdentification.EventActionCode = code;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddAuditSource(auditSource);
		}

		/// <summary>
		/// The identity of the person or process manipulating the data. If both
		/// the person and the process are known, both shall be included.
		/// </summary>
		/// <param name="participant">The participant to add.</param>
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
			InternalAddParticipantObject(patient.PatientId + patient.PatientsName,patient);
		}
	}
}
