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

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
	[DataContract]
	public class OrderNoteboxItemSummary : DataContractBase, IVersionedEquatable<OrderNoteboxItemSummary>
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="noteRef"></param>
		/// <param name="orderRef"></param>
		/// <param name="patientRef"></param>
		/// <param name="patientProfileRef"></param>
		/// <param name="mrn"></param>
		/// <param name="patientName"></param>
		/// <param name="dateOfBirth"></param>
		/// <param name="accessionNumber"></param>
		/// <param name="diagnosticServiceName"></param>
		/// <param name="category"></param>
		/// <param name="urgent"></param>
		/// <param name="postTime"></param>
		/// <param name="author"></param>
		/// <param name="onBehalfOfGroup"></param>
		/// <param name="isAcknowledged"></param>
		/// <param name="staffRecipients"></param>
		/// <param name="groupRecipients"></param>
		public OrderNoteboxItemSummary(
			EntityRef noteRef,
			EntityRef orderRef,
			EntityRef patientRef,
			EntityRef patientProfileRef,
			CompositeIdentifierDetail mrn,
			PersonNameDetail patientName,
			DateTime? dateOfBirth,
			string accessionNumber,
			string diagnosticServiceName,
			string category,
			bool urgent,
			DateTime? postTime,
			StaffSummary author,
			StaffGroupSummary onBehalfOfGroup,
			bool isAcknowledged,
			List<StaffSummary> staffRecipients,
			List<StaffGroupSummary> groupRecipients)
		{
			NoteRef = noteRef;
			OrderRef = orderRef;
			PatientRef = patientRef;
			PatientProfileRef = patientProfileRef;
			Mrn = mrn;
			PatientName = patientName;
			DateOfBirth = dateOfBirth;
			AccessionNumber = accessionNumber;
			DiagnosticServiceName = diagnosticServiceName;
			Category = category;
			Urgent = urgent;
			PostTime = postTime;
			Author = author;
			OnBehalfOfGroup = onBehalfOfGroup;
			IsAcknowledged = isAcknowledged;
			StaffRecipients = staffRecipients;
			GroupRecipients = groupRecipients;
		}
		/// <summary>
		/// Gets a reference to the note.
		/// </summary>
		[DataMember]
		public EntityRef NoteRef;

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
		/// Gets a value indicating whether the note is considered urgent or not.
		/// </summary>
		[DataMember]
		public bool Urgent;

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
		/// Gets the group on behalf of which the note was sent, or null if none.
		/// </summary>
		[DataMember]
		public StaffGroupSummary OnBehalfOfGroup;

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

		public override int GetHashCode()
		{
			return NoteRef.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as OrderNoteboxItemSummary);
		}

		public bool Equals(OrderNoteboxItemSummary orderNoteboxItemSummary)
		{
			return Equals(orderNoteboxItemSummary, false);
		}

		#region IVersionedEquatable<OrderNoteboxItemSummary> Members

		public bool Equals(OrderNoteboxItemSummary other, bool ignoreVersion)
		{
			if (other == null) return false;
			return this.NoteRef.Equals(other.NoteRef, ignoreVersion);
		}

		#endregion

		#region IVersionedEquatable Members

		public bool Equals(object other, bool ignoreVersion)
		{
			return Equals(other as OrderNoteboxItemSummary, ignoreVersion);
		}

		#endregion
	}
}
