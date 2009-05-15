using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	[SettingsGroupDescription("Stores settings for synchronization tools.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class SynchronizationToolSettings
	{
		public SynchronizationToolSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}