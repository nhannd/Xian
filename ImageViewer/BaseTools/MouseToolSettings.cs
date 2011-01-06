#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	[SettingsGroupDescription("Settings for mouse tools in the ImageViewer.")]
	[SettingsProvider(typeof (StandardSettingsProvider))]
	//TODO: make "universal" and re-enable for next release.
	[UserSettingsMigrationDisabled]
	[SharedSettingsMigrationDisabled]
	internal partial class MouseToolSettings
	{
		public MouseToolSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}