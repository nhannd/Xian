#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common
{
	/// <summary>
	/// Defines a default set of authority groups to be imported at deployment time.  This class
	/// is not used post-deployment.
	/// </summary>
	[ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint), FeatureToken = FeatureTokens.RIS.Core)]
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
                        AuthorityTokens.Workflow.Protocol.Accept,
                        AuthorityTokens.Workflow.Protocol.Create,
                        AuthorityTokens.Workflow.Protocol.Resubmit,
                        AuthorityTokens.Workflow.Protocol.Reassign,
                        AuthorityTokens.Workflow.PreliminaryDiagnosis.Create,

                        AuthorityTokens.Admin.Data.ProtocolGroups,

						AuthorityTokens.FolderSystems.Booking,
						AuthorityTokens.FolderSystems.Protocolling,
						AuthorityTokens.FolderSystems.Emergency,
						AuthorityTokens.FolderSystems.OrderNotes,
                    }),

                new AuthorityGroupDefinition(Clerical,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.Protocol.Resubmit,
  						AuthorityTokens.FolderSystems.Booking,
                   }),

                new AuthorityGroupDefinition(Technologists,
                    new string[] 
                    {
                    }),

                new AuthorityGroupDefinition(Radiologists,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.Protocol.Create,
                        AuthorityTokens.Workflow.Protocol.Accept,
                        AuthorityTokens.Workflow.Protocol.OmitSupervisor,
                        AuthorityTokens.Workflow.PreliminaryDiagnosis.Create,

						AuthorityTokens.FolderSystems.Protocolling,
						AuthorityTokens.FolderSystems.OrderNotes,
                   }),

                new AuthorityGroupDefinition(RadiologyResidents,
                    new string[] 
                    {
						AuthorityTokens.Workflow.Protocol.Create,
                        AuthorityTokens.Workflow.PreliminaryDiagnosis.Create,

						AuthorityTokens.FolderSystems.Protocolling,
						AuthorityTokens.FolderSystems.OrderNotes,
                  }),

                new AuthorityGroupDefinition(EmergencyPhysicians,
                    new string[] 
                    {
                        AuthorityTokens.Workflow.PreliminaryDiagnosis.Create,
						AuthorityTokens.FolderSystems.Emergency,
						AuthorityTokens.FolderSystems.OrderNotes,
                    }),

            };
		}

		#endregion
	}
}
