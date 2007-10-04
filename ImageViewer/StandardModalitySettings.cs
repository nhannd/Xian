using System.Collections.Generic;
using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer
{
	[SettingsGroupDescription("Defines the standard list of modalities used primarily for (but not limited to) searching Dicom servers")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class StandardModalitySettings
	{
		private StandardModalitySettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public ICollection<string> ModalitiesAsArray
		{ 
			get
			{
				return CollectionUtils.Map<string, string>(this.Modalities.Split(','),
				   delegate(string s) { return s.Trim(); });
			}
		}
	}
}
