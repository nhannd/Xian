using System.Configuration;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	[SettingsGroupDescription("Stores the available keystrokes for Lut Presets")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class AvailableLutKeyStrokeSettings
	{
		public AvailableLutKeyStrokeSettings()
		{
		}

		public IEnumerable<XKeys> GetAvailableKeyStrokes()
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof (XKeys));
			foreach (string keyStroke in AvailableKeyStrokes)
				yield return (XKeys)converter.ConvertFromString(keyStroke);
		}
	}
}
