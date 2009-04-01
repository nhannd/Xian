using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters
{
	[SettingsGroupDescription("Stores settings for the Study Filter plugin.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	internal sealed partial class StudyFilterSettings
	{
		public StudyFilterSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}