#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;

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

		public const string HealthcareAdmin = "Healthcare Administrators";
		public const string Clerical = "Clerical";
		public const string Technologists = "Technologists";
		public const string Radiologists = "Radiologists";
		public const string RadiologyResidents = "Radiology Residents";
		public const string EmergencyPhysicians = "Emergency Physicians";


		#region IDefineAuthorityGroups Members

		public AuthorityGroupDefinition[] GetAuthorityGroups()
		{
			return new AuthorityGroupDefinition[]
            {
                new AuthorityGroupDefinition(HealthcareAdmin,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
                        AuthorityTokens.Workflow.PatientBiography.View,
                        AuthorityTokens.Workflow.CannedText.Personal,
                        AuthorityTokens.Workflow.CannedText.Group,
                        AuthorityTokens.Workflow.Patient.Create,
                        AuthorityTokens.Workflow.Patient.Update,
                        AuthorityTokens.Workflow.Patient.Reconcile,
                        AuthorityTokens.Workflow.PatientProfile.Update,
                        AuthorityTokens.Workflow.Visit.Create,
                        AuthorityTokens.Workflow.Visit.Update,
                        AuthorityTokens.Workflow.Order.Create,
                        AuthorityTokens.Workflow.Order.Modify,
                        AuthorityTokens.Workflow.Order.Cancel,
                        AuthorityTokens.Workflow.Order.Replace,
                        AuthorityTokens.Workflow.Procedure.CheckIn,
                        AuthorityTokens.Workflow.Protocol.Accept,
                        AuthorityTokens.Workflow.Protocol.Create,
                        AuthorityTokens.Workflow.Protocol.Resubmit,
                        AuthorityTokens.Workflow.Protocol.Reassign,
                        AuthorityTokens.Workflow.Documentation.Create,
                        AuthorityTokens.Workflow.Documentation.Accept,
                        AuthorityTokens.Workflow.PreliminaryDiagnosis.Create,
                        AuthorityTokens.Workflow.Report.Create,
                        AuthorityTokens.Workflow.Report.Cancel,
                        AuthorityTokens.Workflow.Report.Verify,
                        AuthorityTokens.Workflow.Report.OmitSupervisor,
                        AuthorityTokens.Workflow.Report.Reassign,
                        AuthorityTokens.Workflow.ExternalPractitioner.Create,
                        AuthorityTokens.Workflow.ExternalPractitioner.Update,
                        AuthorityTokens.Workflow.ExternalPractitioner.Merge,

                        AuthorityTokens.Admin.Data.DiagnosticService,
                        AuthorityTokens.Admin.Data.Enumeration,
                        AuthorityTokens.Admin.Data.ExternalPractitioner,
                        AuthorityTokens.Admin.Data.Facility,
                        AuthorityTokens.Admin.Data.Location,
                        AuthorityTokens.Admin.Data.Modality,
                        AuthorityTokens.Admin.Data.PatientNoteCategory,
                        AuthorityTokens.Admin.Data.ProcedureType,
                        AuthorityTokens.Admin.Data.ProcedureTypeGroup,
                        AuthorityTokens.Admin.Data.ProtocolGroups,
                        AuthorityTokens.Admin.Data.Staff,
                        AuthorityTokens.Admin.Data.StaffGroup,
                        AuthorityTokens.Admin.Data.Worklist,

						AuthorityTokens.FolderSystems.Booking,
						AuthorityTokens.FolderSystems.Registration,
						AuthorityTokens.FolderSystems.Performing,
						AuthorityTokens.FolderSystems.Protocolling,
						AuthorityTokens.FolderSystems.Reporting,
						AuthorityTokens.FolderSystems.OrderNotes,
						AuthorityTokens.FolderSystems.Emergency,
						AuthorityTokens.FolderSystems.RadiologistAdmin,
                    }),

                new AuthorityGroupDefinition(Clerical,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
                        AuthorityTokens.Workflow.PatientBiography.View,
                        AuthorityTokens.Workflow.CannedText.Personal,
                        AuthorityTokens.Workflow.Patient.Create,
                        AuthorityTokens.Workflow.Patient.Update,
                        AuthorityTokens.Workflow.Patient.Reconcile,
                        AuthorityTokens.Workflow.PatientProfile.Update,
                        AuthorityTokens.Workflow.Visit.Create,
                        AuthorityTokens.Workflow.Visit.Update,
                        AuthorityTokens.Workflow.Order.Create,
                        AuthorityTokens.Workflow.Order.Modify,
                        AuthorityTokens.Workflow.Order.Cancel,
                        AuthorityTokens.Workflow.Order.Replace,
                        AuthorityTokens.Workflow.Procedure.CheckIn,
                        AuthorityTokens.Workflow.Protocol.Resubmit,
                        AuthorityTokens.Workflow.ExternalPractitioner.Create,
                        AuthorityTokens.Workflow.ExternalPractitioner.Update,

 						AuthorityTokens.FolderSystems.Booking,
						AuthorityTokens.FolderSystems.Registration,
                   }),

                new AuthorityGroupDefinition(Technologists,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
                        AuthorityTokens.Workflow.PatientBiography.View,
                        AuthorityTokens.Workflow.CannedText.Personal,
                        AuthorityTokens.Workflow.Order.Create,
                        AuthorityTokens.Workflow.Order.Modify,
                        AuthorityTokens.Workflow.Order.Cancel,
                        AuthorityTokens.Workflow.Order.Replace,
                        AuthorityTokens.Workflow.Procedure.CheckIn,
                        AuthorityTokens.Workflow.Documentation.Create,
                        AuthorityTokens.Workflow.Documentation.Accept,

						AuthorityTokens.FolderSystems.Registration,
						AuthorityTokens.FolderSystems.Performing,
                    }),

                new AuthorityGroupDefinition(Radiologists,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
                        AuthorityTokens.Workflow.PatientBiography.View,
                        AuthorityTokens.Workflow.CannedText.Personal,
                        AuthorityTokens.Workflow.Report.Create,
                        AuthorityTokens.Workflow.Report.Verify,
                        AuthorityTokens.Workflow.Report.OmitSupervisor,
                        AuthorityTokens.Workflow.Protocol.Create,
                        AuthorityTokens.Workflow.Protocol.Accept,
                        AuthorityTokens.Workflow.Protocol.OmitSupervisor,
                        AuthorityTokens.Workflow.PreliminaryDiagnosis.Create,

						AuthorityTokens.FolderSystems.Protocolling,
						AuthorityTokens.FolderSystems.Reporting,
						AuthorityTokens.FolderSystems.OrderNotes,
                   }),

                new AuthorityGroupDefinition(RadiologyResidents,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
						AuthorityTokens.Workflow.PatientBiography.View,
						AuthorityTokens.Workflow.CannedText.Personal,
						AuthorityTokens.Workflow.Report.Create,
						AuthorityTokens.Workflow.Protocol.Create,
                        AuthorityTokens.Workflow.PreliminaryDiagnosis.Create,

						AuthorityTokens.FolderSystems.Protocolling,
						AuthorityTokens.FolderSystems.Reporting,
						AuthorityTokens.FolderSystems.OrderNotes,
                   }),

                new AuthorityGroupDefinition(EmergencyPhysicians,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.HomePage.View,
                        AuthorityTokens.Workflow.PatientBiography.View,
                        AuthorityTokens.Workflow.CannedText.Personal,
                        AuthorityTokens.Workflow.PreliminaryDiagnosis.Create,

						AuthorityTokens.FolderSystems.OrderNotes,
						AuthorityTokens.FolderSystems.Emergency,
                    }),

            };
		}

		#endregion
	}
}
