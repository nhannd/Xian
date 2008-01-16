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
    [ExtensionPoint]
    public class WorklistTokenExtensionPoint : ExtensionPoint<IWorklistTokens> { }

    public interface IWorklistTokens { }

    [ExtensionOf(typeof(WorklistTokenExtensionPoint))]
    public class WorklistTokens : IWorklistTokens
    {
        #region Registration Worklist Tokens

        [WorklistToken(Description = "Registration - Scheduled")]
        public const string RegistrationScheduledWorklist = "RegistrationScheduledWorklist";

        [WorklistToken(Description = "Registration - Checked In")]
        public const string RegistrationCheckedInWorklist = "RegistrationCheckedInWorklist";

        [WorklistToken(Description = "Registration - Cancelled")]
        public const string RegistrationCancelledWorklist = "RegistrationCancelledWorklist";

        [WorklistToken(Description = "Registration - Completed")]
        public const string RegistrationCompletedWorklist = "RegistrationCompletedWorklist";

        [WorklistToken(Description = "Registration - In Progress")]
        public const string RegistrationInProgressWorklist = "RegistrationInProgressWorklist";

        [WorklistToken(Description = "Registration - Completed Protocol")]
        public const string RegistrationCompletedProtocolWorklist = "RegistrationCompletedProtocolWorklist";

        [WorklistToken(Description = "Registration - Suspended Protocol")]
        public const string RegistrationSuspendedProtocolWorklist = "RegistrationSuspendedProtocolWorklist";

        [WorklistToken(Description = "Registration - Rejected Protocol")]
        public const string RegistrationRejectedProtocolWorklist = "RegistrationRejectedProtocolWorklist";

        [WorklistToken(Description = "Registration - Pending Protocol")]
        public const string RegistrationPendingProtocolWorklist = "RegistrationPendingProtocolWorklist";

        [WorklistToken(Description = "Registration - To Be Scheduled")]
        public const string RegistrationToBeScheduledWorklist = "RegistrationToBeScheduledWorklist";

        #endregion

        #region Technologist Worklist Tokens

        [WorklistToken(Description = "Technologist - Scheduled")]
        public const string TechnologistScheduledWorklist = "TechnologistScheduledWorklist";

        [WorklistToken(Description = "Technologist - Checked In")]
        public const string TechnologistCheckedInWorklist = "TechnologistCheckedInWorklist";

        [WorklistToken(Description = "Technologist - Cancelled")]
        public const string TechnologistCancelledWorklist = "TechnologistCancelledWorklist";

        [WorklistToken(Description = "Technologist - Completed")]
        public const string TechnologistCompletedWorklist = "TechnologistCompletedWorklist";

        [WorklistToken(Description = "Technologist - In Progress")]
        public const string TechnologistInProgressWorklist = "TechnologistInProgressWorklist";

        [WorklistToken(Description = "Technologist - Suspended")]
        public const string TechnologistSuspendedWorklist = "TechnologistSuspendedWorklist";

        [WorklistToken(Description = "Technologist - Undocumented")]
        public const string TechnologistUndocumentedWorklist = "TechnologistUndocumentedWorklist";

        #endregion

        #region Reporting Worklist Tokens

        [WorklistToken(Description = "Reporting - To Be Reported")]
        public const string ReportingToBeReportedWorklist = "ReportingToBeReportedWorklist";

        [WorklistToken(Description = "Reporting - To Be Protocolled")]
        public const string ReportingToBeProtocolledWorklist = "ReportingToBeProtocolledWorklist";

        [WorklistToken(Description = "Reporting - Draft")]
        public const string ReportingDraftWorklist = "ReportingDraftWorklist";
        
        [WorklistToken(Description = "Reporting - In Transcription")]
        public const string ReportingInTranscriptionWorklist = "ReportingInTranscriptionWorklist";

        [WorklistToken(Description = "Reporting - To Be Verified")]
        public const string ReportingToBeVerifiedWorklist = "ReportingToBeVerifiedWorklist";

        [WorklistToken(Description = "Reporting - Verified")]
        public const string ReportingVerifiedWorklist = "ReportingVerifiedWorklist";

        [WorklistToken(Description = "Reporting - Review Resident Report")]
        public const string ReportingReviewResidentReportWorklist = "ReportingReviewResidentReportWorklist";

        [WorklistToken(Description = "Reporting - To Be Approved")]
        public const string ReportingToBeApprovedWorklist = "ReportingToBeApprovedWorklist";

        [WorklistToken(Description = "Reporting - Draft Protocol")]
        public const string ReportingDraftProtocolWorklist = "ReportingDraftProtocolWorklist";

        [WorklistToken(Description = "Reporting - Completed Protocol")]
        public const string ReportingCompletedProtocolWorklist = "ReportingCompletedProtocolWorklist";

        [WorklistToken(Description = "Reporting - Suspended Protocol")]
        public const string ReportingSuspendedProtocolWorklist = "ReportingSuspendedProtocolWorklist";

        [WorklistToken(Description = "Reporting - Rejected Protocol")]
        public const string ReportingRejectedProtocolWorklist = "ReportingRejectedProtocolWorklist";

        #endregion
    }
}
