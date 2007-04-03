using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.Ris.Application.Common
{
    public static class AuthorityTokens
    {
        [AuthorityToken(Description="Allow administration of patient profiles")]
        public const string PatientProfileAdmin = "PatientProfileAdmin";

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

        [AuthorityToken(Description = "Allow administration of practitioners")]
        public const string PractitionerAdmin = "PractitionerAdmin";

        [AuthorityToken(Description = "Allow administration of staff")]
        public const string StaffAdmin = "StaffAdmin";

        [AuthorityToken(Description = "Allow administration of visit details")]
        public const string VisitAdmin = "VisitAdmin";

    }
}
