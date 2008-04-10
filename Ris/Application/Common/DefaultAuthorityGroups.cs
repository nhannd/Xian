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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Common;

namespace ClearCanvas.Ris.Application.Common
{
    /// <summary>
    /// Defines a default set of authority groups to be imported at deployment time.  This class
    /// is not used post-deployment.
    /// </summary>
    [ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint))]
    public class DefaultAuthorityGroups : IDefineAuthorityGroups
    {
        // note: we do not define the sys admin ("Administrators") group here because it is defined 
        // automatically in ClearCanvas.Enterprise.Authentication

        private const string HealthcareAdmin = "Healthcare Administrators";
        private const string TechnicalSupport = "Technical Support";
        private const string Clerical = "Clerical";
        private const string Transcriptionists = "Transcriptionists";
        private const string Technologists = "Technologists";
        private const string Radiologists = "Radiologists";
        private const string RadiologyResidents = "Radiology Residents";
        private const string Engineers = "Engineers";


        #region IDefineAuthorityGroups Members

        public AuthorityGroupDefinition[] GetAuthorityGroups()
        {
            return new AuthorityGroupDefinition[]
            {
                new AuthorityGroupDefinition(HealthcareAdmin,
                    new string[] 
                    {
                        AuthorityTokens.PatientProfileAdmin,
                        AuthorityTokens.ReconcilePatients,
                        AuthorityTokens.DiagnosticServiceAdmin,
                        AuthorityTokens.FacilityAdmin,
                        AuthorityTokens.LocationAdmin,
                        AuthorityTokens.ModalityAdmin,
                        AuthorityTokens.HL7Admin,
                        AuthorityTokens.NoteAdmin,
                        AuthorityTokens.StaffAdmin,
                        AuthorityTokens.ExternalPractitionerAdmin,
                        AuthorityTokens.VisitAdmin,
                        AuthorityTokens.UserAdmin,
                        AuthorityTokens.AuthorityGroupAdmin,
                        AuthorityTokens.ProcedureTypeGroupAdmin,
                        AuthorityTokens.WorklistAdmin
                    }),

                new AuthorityGroupDefinition(TechnicalSupport,
                    new string[] 
                    {
                        AuthorityTokens.PatientProfileAdmin,
                        AuthorityTokens.ReconcilePatients,
                        AuthorityTokens.DiagnosticServiceAdmin,
                        AuthorityTokens.FacilityAdmin,
                        AuthorityTokens.LocationAdmin,
                        AuthorityTokens.ModalityAdmin,
                        AuthorityTokens.HL7Admin,
                        AuthorityTokens.NoteAdmin,
                        AuthorityTokens.StaffAdmin,
                        AuthorityTokens.ExternalPractitionerAdmin,
                        AuthorityTokens.VisitAdmin,
                        AuthorityTokens.UserAdmin,
                        AuthorityTokens.AuthorityGroupAdmin,
                        AuthorityTokens.ProcedureTypeGroupAdmin,
                        AuthorityTokens.WorklistAdmin
                   }),

                new AuthorityGroupDefinition(Clerical,
                    new string[] 
                    {
                        AuthorityTokens.PatientProfileAdmin,
                        AuthorityTokens.ExternalPractitionerAdmin,
                        AuthorityTokens.ReconcilePatients
                    }),

                new AuthorityGroupDefinition(Transcriptionists,
                    new string[] 
                    {
                    }),

                new AuthorityGroupDefinition(Technologists,
                    new string[] 
                    {
                    }),

                new AuthorityGroupDefinition(Radiologists,
                    new string[] 
                    {
                        AuthorityTokens.VerifyReport
                    }),

                new AuthorityGroupDefinition(RadiologyResidents,
                    new string[] 
                    {
                    }),

                new AuthorityGroupDefinition(Engineers,
                    new string[]
                    {
                        AuthorityTokens.PatientProfileAdmin,
                        AuthorityTokens.ReconcilePatients,
                        AuthorityTokens.DiagnosticServiceAdmin,
                        AuthorityTokens.FacilityAdmin,
                        AuthorityTokens.LocationAdmin,
                        AuthorityTokens.ModalityAdmin,
                        AuthorityTokens.HL7Admin,
                        AuthorityTokens.NoteAdmin,
                        AuthorityTokens.StaffAdmin,
                        AuthorityTokens.ExternalPractitionerAdmin,
                        AuthorityTokens.VisitAdmin,
                        AuthorityTokens.UserAdmin,
                        AuthorityTokens.AuthorityGroupAdmin,
                        AuthorityTokens.ProcedureTypeGroupAdmin,
                        AuthorityTokens.WorklistAdmin,
                        AuthorityTokens.ViewUnfilteredWorkflowFolders
                    }), 

            };
        }

        #endregion
    }
}
