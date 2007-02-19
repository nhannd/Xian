using System;
using System.Configuration;
using System.Collections.Generic;
using System.Collections;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer
{
	[SettingsGroupDescription("Defines the standard list of modalities used primarily for (but not limited to) searching Dicom servers")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class StandardModalitySettings
	{
		private StandardModalitySettings()
		{
			ApplicationSettingsRegister.Instance.RegisterInstance(this);
		}

		~StandardModalitySettings()
		{
			ApplicationSettingsRegister.Instance.UnregisterInstance(this);
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
