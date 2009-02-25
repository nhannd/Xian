using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	[SettingsGroupDescription("Stores settings for standard tools.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class ToolSettings
	{
		public ToolSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}