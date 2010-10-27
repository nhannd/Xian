#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Order Record Audit
	/// </summary>
	/// <remarks>
	/// <para>
	/// This message describes the event of an order being created, modified, accessed, or deleted. This
	/// message may only include information about a single patient.
	/// </para>
	/// <para>
	/// Note: An order record typically is managed by a non-DICOM system. However, DICOM applications often
	/// manipulate order records, and thus may be obligated by site security policies to record such events in the
	/// audit logs.
	/// </para>
	/// </remarks>
	public class OrderRecordAuditHelper : DicomAuditHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="auditSource"></param>
		/// <param name="outcome"></param>
		/// <param name="code"></param>
		public OrderRecordAuditHelper(DicomAuditSource auditSource, EventIdentificationTypeEventOutcomeIndicator outcome,
			EventIdentificationTypeEventActionCode code)
			: base("OrderRecord")
		{
			AuditMessage.EventIdentification = new EventIdentificationType();
			AuditMessage.EventIdentification.EventID = CodedValueType.OrderRecord;
			AuditMessage.EventIdentification.EventActionCode = code;
			AuditMessage.EventIdentification.EventActionCodeSpecified = true;
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
