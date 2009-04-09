using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Measurement
{
	[SettingsGroupDescription("Stores settings for measurement tools.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class Settings
	{
		public Settings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}