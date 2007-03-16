using System;
using System.Configuration;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer.ServerTree
{

	// TODO add a description of the purpose of the settings group here
	[SettingsGroupDescription("")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class DicomServerSettings
	{

		public DicomServerSettings()
		{
		}
	}
}
