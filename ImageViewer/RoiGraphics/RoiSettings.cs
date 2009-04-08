using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.RoiGraphics
{
	[SettingsGroupDescription("Configures ROI settings.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class RoiSettings
	{
		public RoiSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}