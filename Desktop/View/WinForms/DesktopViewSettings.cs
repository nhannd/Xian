using System;
using System.Configuration;

namespace ClearCanvas.Desktop.View.WinForms
{

	// TODO add a description of the purpose of the settings group here
	[SettingsGroupDescription("")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class DesktopViewSettings
	{

		public DesktopViewSettings()
		{
		}
	}
}
