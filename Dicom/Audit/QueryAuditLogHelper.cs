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

using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;

namespace ClearCanvas.Dicom.Audit
{
	public class QueryAuditLogHelper : DicomAuditHelper
	{
		public QueryAuditLogHelper(EventIdentificationTypeEventOutcomeIndicator outcome, 
			AssociationParameters parms,
			string enterpriseId,
			string applicationId,
			AuditSourceTypeCodeEnum sourceCode)
		{
			AuditMessage.EventIdentification = new EventIdentificationType();
			AuditMessage.EventIdentification.EventID = CodedValueType.Query;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationTypeEventActionCode.E;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddActiveDicomParticipant(parms);

			InternalAddAuditSource(enterpriseId, applicationId, sourceCode);
		}

		public QueryAuditLogHelper(EventIdentificationTypeEventOutcomeIndicator outcome,
			AssociationParameters parms,
			string applicationId)
		{
			AuditMessage.EventIdentification = new EventIdentificationType();
			AuditMessage.EventIdentification.EventID = CodedValueType.Query;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationTypeEventActionCode.E;
			AuditMessage.EventIdentification.EventDateTime = DateTime.Now.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			InternalAddActiveDicomParticipant(parms);

			InternalAddAuditSource(applicationId);
		}

		/// <summary>
		/// Add the ID of person or process that started or stopped the Application.  Can be called multiple times.
		/// </summary>
		/// <param name="userId">The user ID</param>
		/// <param name="alternateUserId">Alternate User Id, can be null</param>
		/// <param name="userName">The user name</param>
		public void AddParticipant(string userId, string alternateUserId, string userName)
		{
			InternalAddActiveParticipant(null, userId, alternateUserId, userName);
		}

		public void AddPatientParticipant(string patientId, string patientName)
		{
			InternalAddPatientParticipantObject(patientId, patientName);
		}

		public void AddStudyParticipant(string studyInstanceUid)
		{
			InternalAddStudyParticipantObject(studyInstanceUid);
		}

		public void AddStudyParticipant(string studyInstanceUid, string mppsUid, string accessionNumber, AuditSopClass[] sopClasses)
		{
			InternalAddStudyParticipantObject(studyInstanceUid, mppsUid, accessionNumber, sopClasses);
		}
	
	}
}
