using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	public sealed class LutPresetSettings : ExtensionSettings
	{
		public LutPresetSettings()
		{ 
		}

		[UserScopedSetting]
		public List<LutPresetGroup> LutPresetGroups
		{
			get { return (List<LutPresetGroup>)this["LutPresetGroups"]; }
			set { this["LutPresetGroups"] = value; }
		}
	}
}
