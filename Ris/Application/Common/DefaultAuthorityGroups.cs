#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

						AuthorityTokens.Management.HL7.QueueMonitor,

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
