using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
	[DataContract]
	public class OrderNoteboxItemSummary : DataContractBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="orderNoteRef"></param>
		/// <param name="orderRef"></param>
		/// <param name="patientRef"></param>
		/// <param name="patientProfileRef"></param>
		/// <param name="mrn"></param>
		/// <param name="patientName"></param>
		/// <param name="dateOfBirth"></param>
		/// <param name="accessionNumber"></param>
		/// <param name="diagnosticServiceName"></param>
		/// <param name="category"></param>
		/// <param name="postTime"></param>
		/// <param name="author"></param>
		/// <param name="isAcknowledged"></param>
		/// <param name="staffRecipients"></param>
		/// <param name="groupRecipients"></param>
		public OrderNoteboxItemSummary(EntityRef orderNoteRef, EntityRef orderRef, EntityRef patientRef, EntityRef patientProfileRef,
			CompositeIdentifierDetail mrn, PersonNameDetail patientName, DateTime? dateOfBirth, string accessionNumber,
			string diagnosticServiceName, string category, DateTime? postTime, StaffSummary author, bool isAcknowledged,
			List<StaffSummary> staffRecipients,
			List<StaffGroupSummary> groupRecipients)
		{
			OrderNoteRef = orderNoteRef;
			OrderRef = orderRef;
			PatientRef = patientRef;
			PatientProfileRef = patientProfileRef;
			Mrn = mrn;
			PatientName = patientName;
			DateOfBirth = dateOfBirth;
			AccessionNumber = accessionNumber;
			DiagnosticServiceName = diagnosticServiceName;
			Category = category;
			PostTime = postTime;
			Author = author;
			IsAcknowledged = isAcknowledged;
			StaffRecipients = staffRecipients;
			GroupRecipients = groupRecipients;
		}

		/// <summary>
		/// Gets a reference to the order note.
		/// </summary>
		[DataMember]
		public EntityRef OrderNoteRef;

		/// <summary>
		/// Gets a reference to the order.
		/// </summary>
		[DataMember]
		public EntityRef OrderRef;

		/// <summary>
		/// Gets a reference to the patient.
		/// </summary>
		[DataMember]
		public EntityRef PatientRef;

		/// <summary>
		/// Gets a reference to the patient profile.
		/// </summary>
		[DataMember]
		public EntityRef PatientProfileRef;

		/// <summary>
		/// Gets the patient MRN.
		/// </summary>
		[DataMember]
		public CompositeIdentifierDetail Mrn;

		/// <summary>
		/// Gets the patient name.
		/// </summary>
		[DataMember]
		public PersonNameDetail PatientName;

		/// <summary>
		/// Gets the patient date of birth.
		/// </summary>
		[DataMember]
		public DateTime? DateOfBirth;

		/// <summary>
		/// Gets the order accession number.
		/// </summary>
		[DataMember]
		public string AccessionNumber;

		/// <summary>
		/// Gets the diagnostic service name.
		/// </summary>
		[DataMember]
		public string DiagnosticServiceName;

		/// <summary>
		/// Gets the note category.
		/// </summary>
		[DataMember]
		public string Category;

		/// <summary>
		/// Gets the time the note was posted.
		/// </summary>
		[DataMember]
		public DateTime? PostTime;

		/// <summary>
		/// Gets the note author.
		/// </summary>
		[DataMember]
		public StaffSummary Author;

		/// <summary>
		/// Gets a value indicating whether the note has been acknowledged.
		/// For "Sent Items", this value is only true if the note has been acknowledged by all recipients.
		/// </summary>
		[DataMember]
		public bool IsAcknowledged;

		/// <summary>
		/// Gets the list of staff recipients.
		/// </summary>
		[DataMember]
		public List<StaffSummary> StaffRecipients;

		/// <summary>
		/// Gets the list of group recipients.
		/// </summary>
		[DataMember]
		public List<StaffGroupSummary> GroupRecipients;
	}
}
