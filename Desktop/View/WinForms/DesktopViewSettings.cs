using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop.View.WinForms
{

	[SettingsGroupDescription("Stores window position for the application, so it can be restored the next time it runs")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class DesktopViewSettings
	{
		private DesktopViewSettings()
		{
			ApplicationSettingsRegister.Instance.RegisterInstance(this);
		}
	}
}
