#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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

using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.Audit
{
	/// <summary>
	/// Begin Transferring DICOM Instances
	/// </summary>
	/// <remarks>
	/// This message describes the event of a system begining to transfer a set of DICOM instances from one
	/// node to another node within control of the system’s security domain. This message may only include
	/// information about a single patient.
	/// </remarks>
	public class BeginTransferringDicomInstancesAuditHelper : DicomAuditHelper
	{
		public BeginTransferringDicomInstancesAuditHelper(EventIdentificationTypeEventOutcomeIndicator outcome,
			AssociationParameters parms)
		{
			AuditMessage.EventIdentification = new EventIdentificationType();
			AuditMessage.EventIdentification.EventID = CodedValueType.BeginTransferringDICOMInstances;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationTypeEventActionCode.E;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddActiveDicomParticipant(parms);
		}

		/// <summary>
		/// (Optional) The identity of any other participants that might be involved andknown, especially third parties that are the requestor
		/// </summary>
		/// <param name="userId">The user ID.</param>
		/// <param name="userName">The name of the user.</param>
		public void AddParticipant(string userId, string userName)
		{
			InternalAddActiveParticipant(null, userId, null, userName);
		}

		/// <summary>
		/// Add details about the patient affected.
		/// </summary>
		/// <param name="patientId"></param>
		/// <param name="patientName"></param>
		public void AddPatientParticipant(string patientId, string patientName)
		{
			InternalAddPatientParticipantObject(patientId, patientName);
		}

		/// <summary>
		/// Add details of a study.
		/// </summary>
		/// <param name="studyInstanceUid"></param>
		public void AddStudyParticipant(string studyInstanceUid)
		{
			InternalAddStudyParticipantObject(studyInstanceUid);
		}

		/// <summary>
		/// Add details of a Study
		/// </summary>
		/// <param name="studyInstanceUid">Required Study Instance UID.</param>
		/// <param name="mppsUid"></param>
		/// <param name="accessionNumber"></param>
		/// <param name="sopClasses"></param>
		public void AddStudyParticipant(string studyInstanceUid, string mppsUid, string accessionNumber, AuditSopClass[] sopClasses)
		{
			InternalAddStudyParticipantObject(studyInstanceUid, mppsUid, accessionNumber, sopClasses);
		}
	}
}
