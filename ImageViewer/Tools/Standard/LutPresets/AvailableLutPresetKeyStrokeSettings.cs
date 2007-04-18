using System;
using System.Configuration;
using System.Collections.Generic;
using ClearCanvas.Desktop;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Tools.Standard.LutPresets
{
	public static class AvailableLutPresetKeyStrokes
	{
		public static IEnumerable<XKeys> GetAvailableKeyStrokes()
		{
			EnumConverter converter = new EnumConverter(typeof(XKeys));

			List<XKeys> keys = new List<XKeys>();
			foreach (string keyStrokeString in AvailableLutPresetKeyStrokeSettings.Default.AvailableKeyStrokes)
				keys.Add((XKeys)converter.ConvertFromInvariantString(keyStrokeString));

			return keys.AsReadOnly();
		}
	}

	[SettingsGroupDescription("Stores the available keystrokes for Lut Presets")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class AvailableLutPresetKeyStrokeSettings
	{
		public AvailableLutPresetKeyStrokeSettings()
		{
		}
	}
}
