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
	public enum ApplicationActivityType
	{
		ApplicationStarted,
		ApplicationStopped
	}

	/// <summary>
	/// Helper for Application Activity Audit Log
	/// </summary>
	public class ApplicationActivityAuditHelper : DicomAuditHelper
	{
		public ApplicationActivityAuditHelper(EventIdentificationTypeEventOutcomeIndicator outcome, ApplicationActivityType type)
		{
			AuditMessage.EventIdentification = new EventIdentificationType();
			AuditMessage.EventIdentification.EventID = CodedValueType.ApplicationActivity;
			AuditMessage.EventIdentification.EventActionCode = EventIdentificationTypeEventActionCode.E;
			AuditMessage.EventIdentification.EventDateTime = Platform.Time.ToUniversalTime();
			AuditMessage.EventIdentification.EventOutcomeIndicator = outcome;
			
			if (type == ApplicationActivityType.ApplicationStarted)
				AuditMessage.EventIdentification.EventTypeCode = new CodedValueType[] { CodedValueType.ApplicationStart };
			else
				AuditMessage.EventIdentification.EventTypeCode = new CodedValueType[] { CodedValueType.ApplicationStop };
		}
		/// <summary>
		/// Add the ID of the Application Started, should be called once.
		/// </summary>
		/// <param name="aeTitle">A comma delimited list of AE titles started</param>
		/// <remarks>
		/// Note that this routine should only be called once.
		/// </remarks>
		public void AddApplicationStartStopId(string aeTitle)
		{
			_participantList.Add(
				new AuditMessageActiveParticipant(CodedValueType.Application, ProcessId, aeTitle, null, null, null, false));
		}

		/// <summary>
		/// Add the ID of person or process that started or stopped the Application.  Can be called multiple times.
		/// </summary>
		/// <param name="userId">The user ID</param>
		/// <param name="userName">The user name</param>
		public void AddParticipant(string userId, string userName)
		{
			InternalAddActiveParticipant(null, userId, null, userName);
		}
	}
}
