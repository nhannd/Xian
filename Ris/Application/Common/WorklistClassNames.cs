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
		public const string RegistrationPerformedWorklist = "RegistrationPerformedWorklist";

		[WorklistClassName]
		public const string RegistrationInProgressWorklist = "RegistrationInProgressWorklist";

		[WorklistClassName]
		public const string RegistrationCompletedProtocolWorklist = "RegistrationCompletedProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationRejectedProtocolWorklist = "RegistrationRejectedProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationPendingProtocolWorklist = "RegistrationPendingProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationAsapPendingProtocolWorklist = "RegistrationAsapPendingProtocolWorklist";

		[WorklistClassName]
		public const string RegistrationToBeScheduledWorklist = "RegistrationToBeScheduledWorklist";

		#endregion

		#region Performing Worklist Class Names

		[WorklistClassName]
		public const string PerformingScheduledWorklist = "PerformingScheduledWorklist";

		[WorklistClassName]
		public const string PerformingCheckedInWorklist = "PerformingCheckedInWorklist";

		[WorklistClassName]
		public const string PerformingCancelledWorklist = "PerformingCancelledWorklist";

		[WorklistClassName]
		public const string PerformingPerformedWorklist = "PerformingPerformedWorklist";

		[WorklistClassName]
		public const string PerformingInProgressWorklist = "PerformingInProgressWorklist";

		[WorklistClassName]
		public const string PerformingSuspendedWorklist = "PerformingSuspendedWorklist";

		[WorklistClassName]
		public const string PerformingUndocumentedWorklist = "PerformingUndocumentedWorklist";

		#endregion

		#region Reporting Worklist Class Names

		[WorklistClassName]
		public const string ReportingToBeReportedWorklist = "ReportingToBeReportedWorklist";

		[WorklistClassName]
		public const string ReportingAssignedWorklist = "ReportingAssignedWorklist";

		[WorklistClassName]
		public const string ReportingDraftWorklist = "ReportingDraftWorklist";

		[WorklistClassName]
		public const string ReportingInTranscriptionWorklist = "ReportingInTranscriptionWorklist";

		[WorklistClassName]
		public const string ReportingReviewTranscriptionWorklist = "ReportingReviewTranscriptionWorklist";

		[WorklistClassName]
		public const string ReportingToBeReviewedReportWorklist = "ReportingToBeReviewedReportWorklist";

		[WorklistClassName]
		public const string ReportingAssignedReviewWorklist = "ReportingAssignedReviewWorklist";

		[WorklistClassName]
		public const string ReportingAwaitingReviewWorklist = "ReportingAwaitingReviewWorklist";

		[WorklistClassName]
		public const string ReportingVerifiedWorklist = "ReportingVerifiedWorklist";

		#region Protocolling

		[WorklistClassName]
		public const string ReportingToBeProtocolledWorklist = "ReportingToBeProtocolledWorklist";

		[WorklistClassName]
		public const string ReportingAssignedProtocolWorklist = "ReportingAssignedProtocolWorklist";

		[WorklistClassName]
		public const string ReportingToBeReviewedProtocolWorklist = "ReportingToBeReviewedProtocolWorklist";

		[WorklistClassName]
		public const string ReportingAssignedReviewProtocolWorklist = "ReportingAssignedReviewProtocolWorklist";

		[WorklistClassName]
		public const string ReportingDraftProtocolWorklist = "ReportingDraftProtocolWorklist";

		[WorklistClassName]
		public const string ReportingCompletedProtocolWorklist = "ReportingCompletedProtocolWorklist";

		[WorklistClassName]
		public const string ReportingRejectedProtocolWorklist = "ReportingRejectedProtocolWorklist";

		[WorklistClassName]
		public const string ReportingAwaitingApprovalProtocolWorklist = "ReportingAwaitingApprovalProtocolWorklist";

		#endregion

		#endregion

		#region Emergency Worklist Class Names

		[WorklistClassName]
		public const string EmergencyScheduledWorklist = "EmergencyScheduledWorklist";

		[WorklistClassName]
		public const string EmergencyInProgressWorklist = "EmergencyInProgressWorklist";

		[WorklistClassName]
		public const string EmergencyPerformedWorklist = "EmergencyPerformedWorklist";

		[WorklistClassName]
		public const string EmergencyCancelledWorklist = "EmergencyCancelledWorklist";
		
		#endregion

		#region Radiologist Admin Worklist Class Names

		[WorklistClassName]
		public const string ReportingAdminUnreportedWorklist = "ReportingAdminUnreportedWorklist";

		[WorklistClassName]
		public const string ReportingAdminAssignedWorklist = "ReportingAdminAssignedWorklist";

		[WorklistClassName]
		public const string ProtocollingAdminAssignedWorklist = "ProtocollingAdminAssignedWorklist";

		#endregion

		#region Transcription Worklist Class Names

		[WorklistClassName]
		public const string TranscriptionToBeTranscribedWorklist = "TranscriptionToBeTranscribedWorklist";

		[WorklistClassName]
		public const string TranscriptionDraftWorklist = "TranscriptionDraftWorklist";

		[WorklistClassName]
		public const string TranscriptionCompletedWorklist = "TranscriptionCompletedWorklist";

		[WorklistClassName]
		public const string TranscriptionToBeReviewedWorklist = "TranscriptionToBeReviewedWorklist";

		[WorklistClassName]
		public const string TranscriptionAwaitingReviewWorklist = "TranscriptionAwaitingReviewWorklist";

		#endregion
	}
}
