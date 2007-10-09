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
                        AuthorityTokens.RequestedProcedureTypeGroupAdmin,
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
                        AuthorityTokens.RequestedProcedureTypeGroupAdmin,
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
                    })

            };
        }

        #endregion
    }
}
