#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		public const string ReportingVerifiedWorklist = "ReportingVerifiedWorklist";

		[WorklistClassName]
		public const string ReportingToBeReviewedReportWorklist = "ReportingToBeReviewedReportWorklist";

		[WorklistClassName]
		public const string ReportingAssignedReviewWorklist = "ReportingAssignedReviewWorklist";

		[WorklistClassName]
		public const string ReportingAwaitingReviewWorklist = "ReportingAwaitingReviewWorklist";

		#endregion


		#region Reporting Tracking

		[WorklistClassName]
		public const string ReportingTrackingActiveWorklist = "ReportingTrackingActiveWorklist";

		[WorklistClassName]
		public const string ReportingTrackingDraftWorklist = "ReportingTrackingDraftWorklist";

		[WorklistClassName]
		public const string ReportingTrackingPreliminaryWorklist = "ReportingTrackingPreliminaryWorklist";

		[WorklistClassName]
		public const string ReportingTrackingFinalWorklist = "ReportingTrackingFinalWorklist";

		[WorklistClassName]
		public const string ReportingTrackingCorrectedWorklist = "ReportingTrackingCorrectedWorklist";

		#endregion

		#region Radiologist Admin Worklist Class Names

		[WorklistClassName]
		public const string ReportingAdminUnreportedWorklist = "ReportingAdminUnreportedWorklist";

		[WorklistClassName]
		public const string ReportingAdminAssignedWorklist = "ReportingAdminAssignedWorklist";

		[WorklistClassName]
		public const string ReportingAdminToBeTranscribedWorklist = "ReportingAdminToBeTranscribedWorklist";

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
