using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Ris.Application.Common
{
    /// <summary>
    /// Defines constants for all RIS authority tokens.
    /// </summary>
    public static class AuthorityTokens
    {
        [AuthorityToken(Description="Allow administration of patient profiles")]
        public const string PatientProfileAdmin = "PatientProfileAdmin";

        [AuthorityToken(Description = "Allow reconciliation of patient profiles")]
        public const string ReconcilePatients = "ReconcilePatients";

        [AuthorityToken(Description = "Allow administration of diagnostic services")]
        public const string DiagnosticServiceAdmin = "DiagnosticServiceAdmin";

        [AuthorityToken(Description = "Allow administration of facilities")]
        public const string FacilityAdmin = "FacilityAdmin";

        [AuthorityToken(Description = "Allow administration of HL7 messages")]
        public const string HL7Admin = "HL7Admin";

        [AuthorityToken(Description = "Allow administration of locations")]
        public const string LocationAdmin = "LocationAdmin";

        [AuthorityToken(Description = "Allow administration of modalities")]
        public const string ModalityAdmin = "ModalityAdmin";

        [AuthorityToken(Description = "Allow administration of notes")]
        public const string NoteAdmin = "NoteAdmin";

        [AuthorityToken(Description = "Allow administration of staff")]
        public const string StaffAdmin = "StaffAdmin";

        [AuthorityToken(Description = "Allow administration of visit details")]
        public const string VisitAdmin = "VisitAdmin";

        [AuthorityToken(Description = "Allow administration of user accounts")]
        public const string UserAdmin = "UserAdmin";

        [AuthorityToken(Description = "Allow administration of authority groups")]
        public const string AuthorityGroupAdmin = "AuthorityGroupAdmin";

        [AuthorityToken(Description = "Allow administration of requested procedure type groups")]
        public const string RequestedProcedureTypeGroupAdmin = "RequestedProcedureTypeGroupAdmin";

        [AuthorityToken(Description = "Allow administration of worklists")]
        public const string WorklistAdmin = "WorklistAdmin";

        [AuthorityToken(Description = "Allow access to the Demo components")]
        public const string DemoAdmin = "DemoAdmin";  
    }
}
