using System;
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
        
        #endregion

        #region Reporting Worklist Tokens

        [WorklistToken(Description = "Reporting - To Be Reported")]
        public const string ReportingToBeReportedWorklist = "ReportingToBeReportedWorklist";

        #endregion
    }
}
