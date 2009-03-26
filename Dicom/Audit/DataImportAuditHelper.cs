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

namespace ClearCanvas.Dicom.Audit
{
	public class DataImportAuditHelper : DicomAuditHelper
	{
			/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="outcome">The outcome (success or failure)</param>
		/// <param name="importDescriptor">Any machine readable identifications on the media, such as media serial number, volume label, 
		/// DICOMDIR SOP Instance UID.</param>
		public DataImportAuditHelper(EventIdentificationTypeEventOutcomeIndicator outcome, string importDescriptor)
		{
			AuditMessage.EventIdentification = new EventIdentificationType();
			AuditMessage.EventIdentification.EventID = CodedValueType.Import;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationTypeEventActionCode.C;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;

			// Add the Destination
			_participantList.Add(
				new AuditMessageActiveParticipant(CodedValueType.SourceMedia, ProcessName, importDescriptor, null, null, null, false));
		}

		/// <summary>
		/// Add an importer.
		/// </summary>
		/// <param name="userId">The identity of the local user or process importer the data. If both
		/// are known, then two active participants shall be included (both the
		/// person and the process).</param>
		/// <param name="userName">The name of the user</param>
		/// <param name="userIsRequestor">Flag telling if the exporter is a user (as opposed to a process)</param>
		public void AddImporter(string userId, string userName, bool userIsRequestor)
		{
			_participantList.Add(
			new AuditMessageActiveParticipant(CodedValueType.SourceMedia, userId, null, userName, ProcessId, NetworkAccessPointTypeEnum.IpAddress, userIsRequestor));
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
