using ClearCanvas.Common;
using ClearCanvas.Common.Authorization;

namespace ClearCanvas.ImageViewer.Services
{
	[ExtensionOf(typeof(DefineAuthorityGroupsExtensionPoint))]
	internal class DefaultAuthorityGroups : IDefineAuthorityGroups
	{
		//TODO: make these only exist in ImageViewer.Common once desktop refactoring is done and this assembly essentially moves to Common
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
						AuthorityTokens.Admin.System.DiskspaceManagement,
						AuthorityTokens.Admin.System.DataStore,
						AuthorityTokens.Admin.System.DicomServer,
						AuthorityTokens.Management.DataStore,
						AuthorityTokens.Management.DicomServer
                    })
			};
		}

		#endregion
	}

	public class AuthorityTokens
	{
		public class Admin
		{
			public class System
			{
				[AuthorityToken(Description = "Allows administrative configuration of Diskspace Management.")]
				public const string DiskspaceManagement = "Admin/System/Viewer/Diskspace Management";

				[AuthorityToken(Description = "Allows administrative configuration of the viewer Data Store.")]
				public const string DataStore = "Admin/System/Viewer/Data Store";

				[AuthorityToken(Description = "Allows administrative configuration of the viewer DICOM Server.")]
				public const string DicomServer = "Admin/System/Viewer/DICOM Server";
			}
		}

		public class Management
		{
			[AuthorityToken(Description = "Allow management of the Data Store (e.g. Reindexing).")]
			public const string DataStore = "Management/Viewer/Data Store";

			[AuthorityToken(Description = "Allow management of the viewer DICOM Server.")]
			public const string DicomServer = "Management/Viewer/DICOM Server";
		}
	}
}
