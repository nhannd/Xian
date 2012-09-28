#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common
{
	public class WorklistClassNames
	{
		#region Registration Protocoling Worklist Class Names

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
		public const string ProtocollingAdminAssignedWorklist = "ProtocollingAdminAssignedWorklist";

		#endregion
	}
}
