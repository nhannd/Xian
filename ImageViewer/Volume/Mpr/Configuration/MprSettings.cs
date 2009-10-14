using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Configuration
{
	[SettingsGroupDescription("MPR user settings.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	internal sealed partial class MprSettings
	{
	}
}