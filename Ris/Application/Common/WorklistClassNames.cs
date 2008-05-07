#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Application.Common
{
	public class WorklistClassNames
	{
		#region Registration Worklist Class Names

		[WorklistClassName]
		public const string RegistrationScheduledWorklist = "RegistrationScheduledWorklist";

		[WorklistClassName]
		public const string RegistrationCheckedInWorklist = "RegistrationCheckedInWorklist";

		[WorklistClassName]
		public const string RegistrationCancelledWorklist = "RegistrationCancelledWorklist";

		[WorklistClassName]
		public const string RegistrationCompletedWorklist = "RegistrationCompletedWorklist";

		[WorklistClassName]
		public const string RegistrationInProgressWorklist = "RegistrationInProgressWorklist";

		[WorklistClassName]
		public const string RegistrationCompletedProtocolWorklist = "RegistrationCompletedProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationSuspendedProtocolWorklist = "RegistrationSuspendedProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationRejectedProtocolWorklist = "RegistrationRejectedProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationPendingProtocolWorklist = "RegistrationPendingProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationAsapPendingProtocolWorklist = "RegistrationAsapPendingProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationToBeScheduledWorklist = "RegistrationToBeScheduledWorklist";

		#endregion

		#region Technologist Worklist Class Names

		[WorklistClassName]
		public const string TechnologistScheduledWorklist = "TechnologistScheduledWorklist";

		[WorklistClassName]
		public const string TechnologistCheckedInWorklist = "TechnologistCheckedInWorklist";

		[WorklistClassName]
		public const string TechnologistCancelledWorklist = "TechnologistCancelledWorklist";

		[WorklistClassName]
		public const string TechnologistCompletedWorklist = "TechnologistCompletedWorklist";

		[WorklistClassName]
		public const string TechnologistInProgressWorklist = "TechnologistInProgressWorklist";

		[WorklistClassName]
		public const string TechnologistSuspendedWorklist = "TechnologistSuspendedWorklist";

		[WorklistClassName]
		public const string TechnologistUndocumentedWorklist = "TechnologistUndocumentedWorklist";

		#endregion

		#region Reporting Worklist Class Names

		[WorklistClassName]
		public const string ReportingToBeReportedWorklist = "ReportingToBeReportedWorklist";

		[WorklistClassName]
		public const string ReportingToBeProtocolledWorklist = "ReportingToBeProtocolledWorklist";

		[WorklistClassName]
		public const string ReportingAssignedWorklist = "ReportingAssignedWorklist";

		[WorklistClassName]
		public const string ReportingDraftWorklist = "ReportingDraftWorklist";

		[WorklistClassName]
		public const string ReportingInTranscriptionWorklist = "ReportingInTranscriptionWorklist";

		[WorklistClassName]
		public const string ReportingRadiologistToBeVerifiedWorklist = "ReportingRadiologistToBeVerifiedWorklist";

		[WorklistClassName]
		public const string ReportingResidentToBeVerifiedWorklist = "ReportingResidentToBeVerifiedWorklist";

		[WorklistClassName]
		public const string ReportingRadiologistVerifiedWorklist = "ReportingRadiologistVerifiedWorklist";

		[WorklistClassName]
		public const string ReportingResidentVerifiedWorklist = "ReportingResidentVerifiedWorklist";

		[WorklistClassName]
		public const string ReportingReviewResidentReportWorklist = "ReportingReviewResidentReportWorklist";

		[WorklistClassName]
		public const string ReportingDraftProtocolWorklist = "ReportingDraftProtocolWorklist";

		[WorklistClassName]
		public const string ReportingCompletedProtocolWorklist = "ReportingCompletedProtocolWorklist";

		[WorklistClassName]
		public const string ReportingSuspendedProtocolWorklist = "ReportingSuspendedProtocolWorklist";

		[WorklistClassName]
		public const string ReportingRejectedProtocolWorklist = "ReportingRejectedProtocolWorklist";

		#endregion

		#region Emergency Worklist Class Names

		[WorklistClassName]
		public const string EmergencyOrdersWorklist = "EmergencyOrdersWorklist";

		#endregion
	}
}
