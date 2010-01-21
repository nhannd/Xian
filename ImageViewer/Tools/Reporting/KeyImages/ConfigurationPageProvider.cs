using System.Collections.Generic;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Reporting.KeyImages
{
	[ExtensionOf(typeof(ConfigurationPageProviderExtensionPoint))]
	public class ConfigurationPageProvider : IConfigurationPageProvider
	{
		#region IConfigurationPageProvider Members

		public IEnumerable<IConfigurationPage> GetPages()
		{
			if (PermissionsHelper.IsInRole(AuthorityTokens.KeyImageAdministration))
				yield return new ConfigurationPage("KeyImageConfigurationPath", new KeyImageConfigurationComponent());
		}

		#endregion
	}
}
