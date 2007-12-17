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

        [AuthorityToken(Description = "Allow administration of external practitioners")]
        public const string ExternalPractitionerAdmin = "ExternalPractitionerAdmin";

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

        [AuthorityToken(Description = "Allow verification of reports")]
        public const string VerifyReport = "VerifyReport";

        [AuthorityToken(Description = "Enable Transcription workflow features")]
        public const string UseTranscriptionWorkflow = "UseTranscriptionWorkflow";

        [AuthorityToken(Description = "Allow administration of protocol groups and codes")]
        public const string ProtocolGroupAdmin = "ProtocolGroupAdmin";
    }
}
