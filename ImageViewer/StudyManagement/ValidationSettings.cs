using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	//TODO: revert these settings?
	[SettingsGroupDescription("Configures validation settings.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	internal sealed partial class ValidationSettings
	{
		private ValidationSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}