using System.Configuration;

namespace ClearCanvas.ImageViewer.Externals.Config
{
	[SettingsGroupDescription("Settings for external applications.")]
	[SettingsProvider(typeof(LocalFileSettingsProvider))]
	internal sealed partial class ExternalsConfigurationSettings
	{
	}
}