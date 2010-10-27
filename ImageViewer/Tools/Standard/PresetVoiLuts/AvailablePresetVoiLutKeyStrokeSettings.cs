#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using System.Collections.Generic;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	[SettingsGroupDescription("Stores the available keystrokes for LUT presets.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class AvailablePresetVoiLutKeyStrokeSettings
	{
		public AvailablePresetVoiLutKeyStrokeSettings()
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
