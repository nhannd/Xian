using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageViewer.Common
{
	[ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint))]
	public class DefaultAuthorityGroups : IDefineAuthorityGroups
	{
		public const string Administrators = "Administrators";
		public const string HealthcareAdministrators = "Healthcare Administrators";
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
				new AuthorityGroupDefinition(Administrators,
                    new string[] 
                    {
						AuthorityTokens.Workflow.Study.Search,
						AuthorityTokens.Workflow.Study.View,
						AuthorityTokens.Workflow.Study.Export,
						AuthorityTokens.Workflow.Study.Create,
						AuthorityTokens.Workflow.Study.Modify,
						AuthorityTokens.Workflow.Study.Delete
                    }),

				new AuthorityGroupDefinition(Clerical,
				    new string[] 
				    {
						AuthorityTokens.Workflow.Study.Search
				   }),

                new AuthorityGroupDefinition(Technologists,
                    new string[] 
                    {
						AuthorityTokens.Workflow.Study.Search,
						AuthorityTokens.Workflow.Study.View,
						AuthorityTokens.Workflow.Study.Export,
						AuthorityTokens.Workflow.Study.Create,
						AuthorityTokens.Workflow.Study.Modify,
						AuthorityTokens.Workflow.Study.Delete
                    }),

                new AuthorityGroupDefinition(Radiologists,
                    new string[] 
                    {
						AuthorityTokens.Workflow.Study.Search,
						AuthorityTokens.Workflow.Study.View,
						AuthorityTokens.Workflow.Study.Export,
						AuthorityTokens.Workflow.Study.Create,
						AuthorityTokens.Workflow.Study.Modify,
						AuthorityTokens.Workflow.Study.Delete
                   }),

                new AuthorityGroupDefinition(RadiologyResidents,
                    new string[] 
                    {
						AuthorityTokens.Workflow.Study.Search,
						AuthorityTokens.Workflow.Study.View,
						AuthorityTokens.Workflow.Study.Export,
						AuthorityTokens.Workflow.Study.Create,
						AuthorityTokens.Workflow.Study.Modify,
						AuthorityTokens.Workflow.Study.Delete
                   }),

                new AuthorityGroupDefinition(EmergencyPhysicians,
                    new string[] 
                    {
						AuthorityTokens.Workflow.Study.Search,
						AuthorityTokens.Workflow.Study.View,
						AuthorityTokens.Workflow.Study.Export
                    }),
            };
		}

		#endregion
	}

	public static class AuthorityTokens
	{
		public class Workflow
		{
			public class Study
			{
				[AuthorityToken(Description = "Generic user permission to search the study data in the viewer.")]
				public const string Search = "Workflow/Viewer/Study/Search";

				[AuthorityToken(Description = "Generic user permission to export study data from the viewer.")]
				public const string Export = "Workflow/Viewer/Study/Export";

				[AuthorityToken(Description = "Generic user permission to view study data in the viewer.")]
				public const string View = "Workflow/Viewer/Study/View";

				[AuthorityToken(Description = "Generic user permission to create a new study in the viewer.")]
				public const string Create = "Workflow/Viewer/Study/Create";

				[AuthorityToken(Description = "Generic user permission to modify study data in the viewer.")]
				public const string Modify = "Workflow/Viewer/Study/Modify";

				[AuthorityToken(Description = "Generic user permission to delete a study from the viewer.")]
				public const string Delete = "Workflow/Viewer/Study/Delete";
			}
		}
	}
}
