using System.Collections.Specialized;
using ClearCanvas.ImageViewer.Services.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Services.Configuration
{
	public class DefaultServersConfigurationComponent : ServerTreeConfigurationComponent
	{
		public DefaultServersConfigurationComponent()
			:base(SR.DescriptionDefaultServers)
		{
		}

		public override void Save()
		{
			DefaultServerSettings.Default.DefaultServerPaths = new StringCollection();

			foreach (string path in SelectedServerPaths)
				DefaultServerSettings.Default.DefaultServerPaths.Add(path);
			
			DefaultServerSettings.Default.Save();
		}
	}
}
